using System;
using System.Linq;
using Godot;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;
using TribesOfDust.Utils.Extensions;

namespace TribesOfDust.Core.Modes;

public partial class EditorMode : Node2D, IUnique<EditorMode>
{
    public static EditorMode? Instance { get; private set; }
    
    private TileMapNode _tileMapNode;
    
    /// <summary>
    /// The TileMapNode responsible for rendering terrain tiles.
    /// </summary>
    public TileMapNode TileMapNode => _tileMapNode ??= GetTileMapNode();
        
    public override void _Ready()
    {
        Context = new MapContext(Core.Context.Instance);
        
        // Initialize the TileMapNode
        _tileMapNode = GetTileMapNode();
        
        // Sync tiles with the TileMapNode
        _tileMapNode.SyncWithMap(Context.Map);
        
        // Register non-tile entities (buildings, units) with sprite rendering
        foreach (var (coordinate, building) in Context.Map.Buildings)
            CreateSpriteForNonTileEntity(coordinate, building);
        foreach (var (coordinate, unit) in Context.Map.Units)
            CreateSpriteForNonTileEntity(coordinate, unit);

        // Register overlays with context   
        Context.Display.AddOverlay(_hoveredOverlay);
        Context.Display.AddOverlay(_activeTypeOverlay);
        Context.Display.AddOverlay(_neighborhoodOverlay);

        Context.Map.Tiles.Added += (_, _, _) => UpdateTypeOverlay();

        // Initialize render state
        UpdateActiveType();
        UpdateTypeOverlay();

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
            var hoveredLocation = TileMapNode.WorldToHexCoordinate(world);

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
                var clickedLocation = TileMapNode.WorldToHexCoordinate(mousePosition);
                
                // Remove the existing tile from the map data
                if (Context.Map.Tiles.Get(clickedLocation) is { } existingTile)
                {
                    Context.Map.TryRemoveEntity(existingTile);
                }
                
                // Create a new tile with the selected tile type and register it with the context
                var newTile = new Tile(Context.Repos.Tiles.GetAsset(_activeTileType), clickedLocation);
                Context.Map.TryAddEntity(newTile);
                
                // Update the TileMapNode
                TileMapNode.SetTile(clickedLocation, newTile);
            }

            // Remove open tiles on right mouse click.
            // Remove other tiles on right mouse click and replace them with an open tile.

            else if (mouseButton is { Pressed: true, ButtonIndex: MouseButton.Right })
            {
                var mousePosition = GetGlobalMousePosition();
                var clickedLocation = TileMapNode.WorldToHexCoordinate(mousePosition);
                var clickedTile = Context.Map.Tiles.Get(clickedLocation);

                // Remove the existing tile
                if (clickedTile is { } tile)
                {
                    Context.Map.TryRemoveEntity(tile);
                    TileMapNode.RemoveTile(clickedLocation);
                }

                // Create a new tile with the open tile type and register it with the context
                if (clickedTile is null || clickedTile.Configuration.Key != TileType.Open)
                {
                    var newTile = new Tile(Context.Repos.Tiles.GetAsset(TileType.Open), clickedLocation);
                    Context.Map.TryAddEntity(newTile);
                    TileMapNode.SetTile(clickedLocation, newTile);
                }
            }
        }

        UpdateActiveType();
    }

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
            UpdateTypeOverlay();
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
    /// Tiles are handled by the TileMapNode.
    /// </summary>
    private void CreateSpriteForNonTileEntity(AxialCoordinate coordinate, IEntity<IConfiguration> entity)
    {
        // Skip tiles - they are handled by TileMapNode
        if (entity is Tile)
            return;

        Sprite2D sprite = new();

        // Since textures should match expected size, scale should be 1.0
        var scale = new Vector2(1.0f, 1.0f);

        sprite.Scale = scale;
        sprite.Centered = true;
        sprite.Position = TileMapNode.HexToWorldPosition(coordinate);
        sprite.Texture = entity.Configuration.Texture;
        sprite.Modulate = entity.Owner?.Color ?? Colors.White;

        switch (entity)
        {
            case Building building:
                sprite.Scale *= 0.8f;
                sprite.ZIndex = 10;
                break;
            case Unit unit:
                sprite.Scale *= 0.8f;
                sprite.ZIndex = 10;
                break;
        }

        Context.Display.Sprites.Add(entity.Identity, sprite);
        AddChild(sprite);
    }

    /// <summary>
    /// Gets or creates the TileMapNode for this editor.
    /// </summary>
    private TileMapNode GetTileMapNode()
    {
        if (_tileMapNode != null)
            return _tileMapNode;
            
        // Look for existing TileMapNode
        foreach (Node child in GetChildren())
        {
            if (child is TileMapNode tileMapNode)
            {
                _tileMapNode = tileMapNode;
                return _tileMapNode;
            }
        }
        
        // Create new TileMapNode if none exists
        _tileMapNode = new TileMapNode
        {
            Name = "TileMapNode"
        };
        
        AddChild(_tileMapNode);
        return _tileMapNode;
    }

    private AxialCoordinate _hoveredLocation;
    private TileType _activeTileType = TileType.Tundra;

    public MapContext Context { get; private set; } = null!;
    private readonly IHexLayer<Color> _hoveredOverlay = new HexLayer<Color>();
    private readonly IHexLayer<Color> _activeTypeOverlay = new HexLayer<Color>();
    private readonly IHexLayer<Color> _neighborhoodOverlay = new HexLayer<Color>();
}