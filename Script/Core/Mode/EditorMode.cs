using System;
using System.Linq;
using Godot;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;

namespace TribesOfDust.Core.Modes;

public partial class EditorMode : Node2D, IUnique<EditorMode>
{
    public static EditorMode? Instance { get; private set; }

    public Rect2 GetMapExtents()
    {
        Vector2 minimum = Vector2.Inf;
        Vector2 maximum = -Vector2.Inf;
        foreach (var tile in Context.Map.Tiles)
        {
            var unitPosition = HexConversions.HexToUnit(tile.Key);
            var x = unitPosition.X * HexConstants.DefaultWidth;
            var y = unitPosition.Y * HexConstants.DefaultHeight;

            minimum.X = Math.Min(minimum.X, x);
            maximum.X = Math.Max(maximum.X, x);
            minimum.Y = Math.Min(minimum.Y, y);
            maximum.Y = Math.Max(maximum.Y, y);
        }

        return new(minimum, maximum - minimum);
    }
        
    public override void _Ready()
    {
        Context = new MapContext(Core.Context.Instance);
        
        // Register tiles
        foreach (var (coordinate, tile) in Context.Map.Tiles)
            CreateSprite(coordinate, tile);

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
            var hoveredCoordinate = HexConversions.UnitToHex(world / HexConstants.DefaultSize);

            if (_hoveredCoordinate != hoveredCoordinate)
            {
                _hoveredCoordinate = hoveredCoordinate;
                _hoveredOverlay.Clear();
                _hoveredOverlay.Add(hoveredCoordinate, Colors.Aqua);
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
                var clickedCoordinate = HexConversions.UnitToHex(mousePosition / HexConstants.DefaultSize);
                
                // Remove the tile that has been clicked from the tiles list
                if (Context.Map.Tiles.Get(clickedCoordinate) is { Identity: var identity })
                {
                    Sprite2D sprite = Context.Display.Sprites[identity];
                    Context.Display.Sprites.Remove(identity);
                    Context.Map.Tiles.Remove(clickedCoordinate);
                    RemoveChild(sprite);
                }
                
                // Create a new tile with the selected tile type and register it with the context
                var newCoordinate = clickedCoordinate;
                var newTile = new Tile(Context.Repos.Tiles.GetAsset(_activeTileType));
                
                Context.Map.Tiles.Add(newCoordinate, newTile);
                CreateSprite(newCoordinate, newTile);
            }

            // Remove open tiles on right mouse click.
            // Remove other tiles on right mouse click and replace them with an open tile.

            else if (mouseButton is { Pressed: true, ButtonIndex: MouseButton.Right })
            {
                var mousePosition = GetGlobalMousePosition();
                var clickedCoordinate = HexConversions.UnitToHex(mousePosition / HexConstants.DefaultSize);
                var clickedTile = Context.Map.Tiles.Get(clickedCoordinate);

                // Remove the tile that has been clicked from the tiles list
                if (clickedTile is { Identity: var identity })
                {
                    Sprite2D sprite = Context.Display.Sprites[identity];
                    Context.Display.Sprites.Remove(identity);
                    Context.Map.Tiles.Remove(clickedCoordinate);
                    RemoveChild(sprite);
                }

                // Create a new tile with the open tile type and register it with the context
                if (clickedTile is null || clickedTile.Configuration.Key != TileType.Open)
                {
                    var newCoordinate = clickedCoordinate;
                    var newTile = new Tile(Context.Repos.Tiles.GetAsset(TileType.Open));

                    Context.Map.Tiles.Add(newCoordinate, newTile);
                    CreateSprite(newCoordinate, newTile);
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
            _activeTypeOverlay.Add(tile.Key, Colors.LightBlue);
    }

    private void CreateSprite(AxialCoordinate coordinate, IEntity<IConfiguration> entity)
    {
        Sprite2D sprite = new();

        float widthScaleToExpected = entity.Configuration.Texture != null
            ? HexConstants.DefaultWidth / entity.Configuration.Texture.GetWidth()
            : 1.0f;
        float heightScaleToExpected = entity.Configuration.Texture != null
            ? HexConstants.DefaultHeight / entity.Configuration.Texture.GetHeight()
            : 1.0f;

        sprite.Scale = new Vector2(widthScaleToExpected, heightScaleToExpected);
        sprite.Centered = true;
        sprite.Position = HexConversions.HexToUnit(coordinate) * HexConstants.DefaultSize;
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
            case Tile tile:
                sprite.Scale *= 1.0f;
                sprite.ZIndex = 1;
                break;
        }

        Context.Display.Sprites.Add(entity.Identity, sprite);
        
        AddChild(sprite);
    }

    private AxialCoordinate? _hoveredCoordinate;
    private TileType _activeTileType = TileType.Tundra;

    public MapContext Context { get; private set; } = null!;
    private readonly IHexLayer<Color> _hoveredOverlay = new HexLayer<Color>();
    private readonly IHexLayer<Color> _activeTypeOverlay = new HexLayer<Color>();
    private readonly IHexLayer<Color> _neighborhoodOverlay = new HexLayer<Color>();
}