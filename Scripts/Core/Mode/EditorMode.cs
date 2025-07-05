using System.Collections.Generic;
using System.Linq;
using Godot;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;
using TribesOfDust.Interface;
using TribesOfDust.Interface.Menu;

namespace TribesOfDust.Core.Modes;

public partial class EditorMode : Node2D, IUnique<EditorMode>
{
    [Export] public NodePath EditorMenuPath { get; set; } = "Canvas/CanvasLayer/EditorMenu";
    
    public static EditorMode? Instance { get; private set; }

    private HexMap?     _hexMap;
    private EditorMenu? _editorMenu;
    
    /// <summary>
    /// The HexMap responsible for rendering terrain tiles.
    /// </summary>
    public HexMap HexMap => _hexMap ??= GetHexMap();
        
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
        foreach (var (coordinate, building) in Context.Map.Buildings)
            CreateSpriteForNonTileEntity(coordinate, building);
        foreach (var (coordinate, unit) in Context.Map.Units)
            CreateSpriteForNonTileEntity(coordinate, unit);

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

    public override void _EnterTree()
    {
        Instance = this;
        base._EnterTree();
    }

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
                _hoveredOverlay.TryAdd(hoveredLocation, Colors.Aqua);
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

            // Remove open tiles on right mouse click.
            // Remove other tiles on right mouse click and replace them with an open tile.

            else if (mouseButton is { Pressed: true, ButtonIndex: MouseButton.Right })
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
        }

        UpdateActiveType();
    }

    #region Editor Menu Integration
    
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
    
    private void OnTileAdded(IHexLayerView<Tile> layer, Tile tile, AxialCoordinate coordinate)
    {
        UpdateTileCount(tile.Configuration.Key);
    }
    
    private void OnTileRemoved(IHexLayerView<Tile> layer, Tile tile, AxialCoordinate coordinate)
    {
        UpdateTileCount(tile.Configuration.Key);
    }
    
    private void UpdateTileCount(TileType tileType)
    {
        if (_editorMenu == null) return;
        
        var count = Context.Map.Tiles.Count(tile => tile.Value.Configuration.Key == tileType);
        _editorMenu.UpdateTileCount(tileType, count);
    }
    
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
    
    public TileType GetActiveTileType()
    {
        return _activeTileType;
    }
    
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

    private void UpdateTypeOverlay()
    {
        _activeTypeOverlay.Clear();

        var tiles = Context.Map.Tiles;
        var overlay = tiles.Where(tile => tile.Value.Configuration.Key == _activeTileType);

        foreach (var tile in overlay)
            _activeTypeOverlay.TryAdd(tile.Key, Colors.LightBlue);
    }

    /// <summary>
    /// Creates a sprite for non-tile entities (buildings, units).
    /// Tiles are handled by the HexMap.
    /// </summary>
    private void CreateSpriteForNonTileEntity(AxialCoordinate coordinate, IEntity<IConfiguration> entity)
    {
        // Skip tiles - they are handled by HexMap
        if (entity is Tile)
            return;

        Sprite2D sprite = new();

        sprite.Scale = Vector2.One;
        sprite.Centered = true;
        sprite.Position = HexMap.HexToWorldPosition(coordinate);
        sprite.Texture = entity.Configuration.Texture;
        sprite.Modulate = entity.Owner?.Color ?? Colors.White;

        switch (entity)
        {
            case Building:
                sprite.Scale *= 0.8f;
                sprite.ZIndex = 10;
                break;
            case Unit:
                sprite.Scale *= 0.8f;
                sprite.ZIndex = 10;
                break;
        }

        // Note: In EditorMode, we don't need to track sprites in a dictionary
        // since we don't use them for selection like in GameMode
        AddChild(sprite);
    }

    /// <summary>
    /// Gets or creates the HexMap for this editor.
    /// </summary>
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

    private AxialCoordinate _hoveredLocation = AxialCoordinate.Zero;
    private TileType _activeTileType = TileType.Tundra;

    public MapContext Context { get; private set; } = null!;
    private readonly IHexLayer<Color> _hoveredOverlay = new HexLayer<Color>();
    private readonly IHexLayer<Color> _activeTypeOverlay = new HexLayer<Color>();
}