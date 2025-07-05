using System.Collections.Generic;
using System.Linq;
using Godot;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;
using TribesOfDust.Interface;
using TribesOfDust.Interface.Menu;
using TribesOfDust.Utils;

namespace TribesOfDust.Core.Modes;

/// <summary>
/// Editor mode for creating and modifying game maps.
/// Provides tile placement/removal, overlay visualization, and integration with the editor menu.
/// </summary>
public partial class EditorMode : Node2D, IUnique<EditorMode>
{
    #region Properties

    /// <summary>
    /// Path to the editor menu node in the scene tree.
    /// </summary>
    [Export] public NodePath EditorMenuPath { get; set; } = "Canvas/CanvasLayer/EditorMenu";
    
    /// <summary>
    /// Singleton instance of the EditorMode.
    /// </summary>
    public static EditorMode? Instance { get; private set; }

    /// <summary>
    /// The HexMap responsible for rendering terrain tiles.
    /// </summary>
    public HexMap HexMap => _hexMap ??= GetHexMap();

    /// <summary>
    /// The map context containing the map data and display settings.
    /// </summary>
    public MapContext Context { get; private set; } = null!;

    #endregion

    #region Godot Lifecycle

    /// <summary>
    /// Initializes the editor mode when the node enters the scene tree.
    /// Sets up the map context, HexMap, overlays, and editor menu integration.
    /// </summary>
    public override void _Ready()
    {
        Context = new MapContext(Core.Context.Instance);
        
        // Initialize the HexMap
        _hexMap = GetHexMap();
        
        // Connect HexMap to Display for overlay support
        Context.Display.HexMap = _hexMap;
        
        // Sync tiles with the HexMap
        _hexMap.ConnectToMap(Context.Map);
        
        // Register non-tile entities (buildings, units) with sprite rendering
        foreach (var (_, building) in Context.Map.Buildings)
            this.CreateSpriteForEntity(building, _hexMap);
        foreach (var (_, unit) in Context.Map.Units)
            this.CreateSpriteForEntity(unit, _hexMap);

        // Register overlays with context   
        Context.Display.AddOverlay(_hoveredOverlay);
        Context.Display.AddOverlay(_activeTypeOverlay);

        Context.Map.Tiles.Added += (_, _, _) => UpdateTypeOverlay();
        Context.Map.Tiles.Removed += (_, _, _) => UpdateTypeOverlay();

        // Initialize render state
        UpdateActiveType();
        UpdateTypeOverlay();
        
        // Set up editor menu
        SetupEditorMenu();

        // Position camera to fit the map
        var camera = GetNode<Camera>("Canvas/Camera2D");
        if (camera != null)
        {
            camera.FitToMap(Context.Map, _hexMap);
        }

        base._Ready();
    }

    /// <summary>
    /// Sets the singleton instance when entering the scene tree.
    /// </summary>
    public override void _EnterTree()
    {
        Instance = this;
        base._EnterTree();
    }

    /// <summary>
    /// Cleans up resources and saves the map when exiting the scene tree.
    /// </summary>
    public override void _ExitTree()
    {
        Instance = null;
        
        // Clean up event subscriptions
        if (_editorMenu != null)
        {
            Context.Map.Tiles.Added -= OnTileAdded;
            Context.Map.Tiles.Removed -= OnTileRemoved;
        }
        
        Context.Repos.Maps.TrySave(Context.Map);
        base._ExitTree();
    }

    #endregion

    #region Input Handling

    /// <summary>
    /// Handles input events for tile placement, removal, and overlay updates.
    /// Processes mouse movements for hover effects and mouse clicks for tile editing.
    /// </summary>
    /// <param name="inputEvent">The input event to process</param>
    public override void _Input(InputEvent inputEvent)
    {
        // Update the active tile and color it accordingly.
        // The active tile is the tile the mouse cursor is currently hovering over.
        // The active tile is colored for highlighting purposes only, this will be removed later on.
        if (inputEvent is InputEventMouseMotion)
        {
            var world = GetGlobalMousePosition();
            var hoveredLocation = HexMap.WorldToHexCoordinate(world);

            if (_hoveredLocation != hoveredLocation)
            {
                _hoveredLocation = hoveredLocation;
                _hoveredOverlay.Clear();
                _hoveredOverlay.TryAdd(hoveredLocation, CoreColor.Cyan);
            }
        }

        // Add and remove tiles on mouse clicks.

        if (inputEvent is InputEventMouseButton mouseButton)
        {
            // Add new tiles or replace existing ones on left mouse click.
            // Existing tiles of a type are replaced with new types.
            // Existing tiles of a type are replaced with a new variation of the same type.

            if (mouseButton is { Pressed: true, ButtonIndex: MouseButton.Left })
            {
                HandleLeftClick();
            }

            // Remove open tiles on right mouse click.
            // Remove other tiles on right mouse click and replace them with an open tile.

            else if (mouseButton is { Pressed: true, ButtonIndex: MouseButton.Right })
            {
                HandleRightClick();
            }
        }

        UpdateActiveType();
    }

    /// <summary>
    /// Handles left mouse click for tile placement.
    /// Places the currently selected tile type at the clicked location.
    /// </summary>
    private void HandleLeftClick()
    {
        var mousePosition = GetGlobalMousePosition();
        var clickedLocation = HexMap.WorldToHexCoordinate(mousePosition);
        
        // Remove the existing tile from the map data
        if (Context.Map.Tiles.Get(clickedLocation) is { } existingTile)
        {
            Context.Map.TryRemoveEntity(existingTile);
        }
        
        // Create a new tile with the selected tile type and register it with the context
        var tiles = Context.Repos.Tiles;
        if (tiles.HasVariations(_activeTileType))
        {
            var config  = tiles.GetAsset(_activeTileType);
            var newTile = new Tile(config, clickedLocation);
            Context.Map.TryAddEntity(newTile);
        }
    }

    /// <summary>
    /// Handles right mouse click for tile removal.
    /// Removes tiles or replaces non-Open tiles with Open tiles.
    /// </summary>
    private void HandleRightClick()
    {
        var mousePosition = GetGlobalMousePosition();
        var clickedLocation = HexMap.WorldToHexCoordinate(mousePosition);
        var clickedTile = Context.Map.Tiles.Get(clickedLocation);

        if (clickedTile is { } tile)
        {
            // Remove the existing tile
            Context.Map.TryRemoveEntity(tile);

            // If it was not an Open tile, replace it with an Open tile
            if (tile.Configuration.Key != TileType.Open)
            {
                var tiles = Context.Repos.Tiles;
                if (tiles.HasVariations(TileType.Open))
                {
                    var config = tiles.GetAsset(TileType.Open);
                    var newTile = new Tile(config, clickedLocation);
                    Context.Map.TryAddEntity(newTile);
                }
            }
        }
        // Do nothing if there's no tile at the clicked location
    }

    /// <summary>
    /// Updates the active tile type based on keyboard input (1-4 keys).
    /// </summary>
    private void UpdateActiveType()
    {
        TileType previousTileType = _activeTileType;

        if (Input.IsKeyPressed(Key.Key1))
            _activeTileType = TileType.Tundra;
        else if (Input.IsKeyPressed(Key.Key2))
            _activeTileType = TileType.Rocks;
        else if (Input.IsKeyPressed(Key.Key3))
            _activeTileType = TileType.Dunes;
        else if (Input.IsKeyPressed(Key.Key4))
            _activeTileType = TileType.Canyon;

        if (_activeTileType != previousTileType)
        {
            UpdateTypeOverlay();
        }
    }

    #endregion

    #region Editor Menu Integration
    
    /// <summary>
    /// Sets up the editor menu and connects event handlers for tile count updates.
    /// </summary>
    private void SetupEditorMenu()
    {
        // Find the editor menu in the scene tree using exported path
        _editorMenu = GetNode<EditorMenu>(EditorMenuPath);
        
        if (_editorMenu != null)
        {
            // Register for map events to update tile counts
            Context.Map.Tiles.Added += OnTileAdded;
            Context.Map.Tiles.Removed += OnTileRemoved;
            
            // Initial update of tile counts
            UpdateMenu();
        }
    }
    
    /// <summary>
    /// Handles tile addition events from the map to update the editor menu.
    /// </summary>
    /// <param name="layer">The tile layer (unused)</param>
    /// <param name="tile">The tile that was added</param>
    /// <param name="coordinate">The coordinate where the tile was added</param>
    private void OnTileAdded(IHexLayerView<Tile> layer, Tile tile, AxialCoordinate coordinate)
    {
        UpdateTileCount(tile.Configuration.Key);
    }
    
    /// <summary>
    /// Handles tile removal events from the map to update the editor menu.
    /// </summary>
    /// <param name="layer">The tile layer (unused)</param>
    /// <param name="tile">The tile that was removed</param>
    /// <param name="coordinate">The coordinate where the tile was removed</param>
    private void OnTileRemoved(IHexLayerView<Tile> layer, Tile tile, AxialCoordinate coordinate)
    {
        UpdateTileCount(tile.Configuration.Key);
    }
    
    /// <summary>
    /// Updates the tile count display for a specific tile type in the editor menu.
    /// </summary>
    /// <param name="tileType">The tile type to update the count for</param>
    private void UpdateTileCount(TileType tileType)
    {
        if (_editorMenu == null) return;
        
        var count = Context.Map.Tiles.Count(tile => tile.Value.Configuration.Key == tileType);
        _editorMenu.UpdateTileCount(tileType, count);
    }
    
    /// <summary>
    /// Updates all tile counts in the editor menu.
    /// Called during initialization to set initial state.
    /// </summary>
    private void UpdateMenu()
    {
        if (_editorMenu == null) return;
        
        var tileCounts = new Dictionary<TileType, int>();
        
        // Initialize all counts to 0
        tileCounts[TileType.Tundra] = 0;
        tileCounts[TileType.Rocks] = 0;
        tileCounts[TileType.Dunes] = 0;
        tileCounts[TileType.Canyon] = 0;
        tileCounts[TileType.Open] = 0;
        
        // Count actual tiles
        foreach (var tile in Context.Map.Tiles)
        {
            var tileType = tile.Value.Configuration.Key;
            if (tileCounts.ContainsKey(tileType))
            {
                tileCounts[tileType]++;
            }
        }
        
        // Update menu
        foreach (var (tileType, count) in tileCounts)
        {
            _editorMenu.UpdateTileCount(tileType, count);
        }
    }
    
    /// <summary>
    /// Gets the currently active tile type.
    /// </summary>
    /// <returns>The currently selected tile type</returns>
    public TileType GetActiveTileType()
    {
        return _activeTileType;
    }
    
    /// <summary>
    /// Sets the active tile type and updates overlays if it changed.
    /// </summary>
    /// <param name="tileType">The tile type to set as active</param>
    public void SetActiveTileType(TileType tileType)
    {
        var previousTileType = _activeTileType;
        _activeTileType = tileType;
        
        if (_activeTileType != previousTileType)
        {
            UpdateTypeOverlay();
        }
    }
    
    #endregion

    #region Overlay Management

    /// <summary>
    /// Updates the type overlay to highlight all tiles of the currently active type.
    /// </summary>
    private void UpdateTypeOverlay()
    {
        _activeTypeOverlay.Clear();

        var tiles = Context.Map.Tiles;
        var overlay = tiles.Where(tile => tile.Value.Configuration.Key == _activeTileType);

        foreach (var tile in overlay)
            _activeTypeOverlay.TryAdd(tile.Key, CoreColor.SkyBlue);
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Gets or creates the HexMap for this editor.
    /// </summary>
    /// <returns>The HexMap instance for this editor</returns>
    private HexMap GetHexMap()
    {
        if (_hexMap != null)
            return _hexMap;
            
        // Look for existing HexMap
        foreach (Node child in GetChildren())
        {
            if (child is HexMap hexMap)
            {
                _hexMap = hexMap;
                return _hexMap;
            }
        }
        
        // Create new HexMap if none exists
        _hexMap = new HexMap
        {
            Name = "HexMap"
        };
        
        AddChild(_hexMap);
        return _hexMap;
    }

    #endregion

    #region Private Fields

    /// <summary>
    /// The HexMap instance used for tile rendering.
    /// </summary>
    private HexMap? _hexMap;
    
    /// <summary>
    /// The editor menu for UI interaction.
    /// </summary>
    private EditorMenu? _editorMenu;

    /// <summary>
    /// The currently hovered coordinate for highlighting.
    /// </summary>
    private AxialCoordinate _hoveredLocation = AxialCoordinate.Zero;
    
    /// <summary>
    /// The currently active/selected tile type for placement.
    /// </summary>
    private TileType _activeTileType = TileType.Tundra;

    /// <summary>
    /// Overlay layer for highlighting the currently hovered tile.
    /// </summary>
    private readonly IHexLayer<CoreColor> _hoveredOverlay = new HexLayer<CoreColor>();
    
    /// <summary>
    /// Overlay layer for highlighting all tiles of the active type.
    /// </summary>
    private readonly IHexLayer<CoreColor> _activeTypeOverlay = new HexLayer<CoreColor>();

    #endregion
}