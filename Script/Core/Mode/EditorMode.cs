using System;
using System.Linq;
using Godot;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex.Storage;
using TribesOfDust.Hex.Neighborhood;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Modes;

public partial class EditorMode : Node2D, IUnique<EditorMode>
{
    public static EditorMode? Instance { get; private set; }

    public Rect2 GetMapExtents()
    {
        Vector2 minimum = Vector2.Inf;
        Vector2 maximum = -Vector2.Inf;
        foreach (var tile in _context.Map.Tiles)
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
        _context = new EditorContext(Context.Instance);
        
        // Register tiles

        foreach (var tile in _context.Map.Tiles)
            AddChild(tile.Value.Sprite);
        
        // Register units
        var @class = _context.Classes.GetAsset();
        var unit1 = new Unit(new(-2, -4), @class);
        var unit2 = new Unit(new( 2,  1), @class);
        var unit3 = new Unit(new( 1, -2), @class);
        
        _context.Map.Units.Add(unit1, unit1.Coordinates);
        _context.Map.Units.Add(unit2, unit2.Coordinates);
        _context.Map.Units.Add(unit3, unit3.Coordinates);
        
        AddChild(unit1.Sprite);
        AddChild(unit2.Sprite);
        AddChild(unit3.Sprite);

        // Register overlays with context   
        _context.Display.AddOverlay(_activeTileOverlay);
        _context.Display.AddOverlay(_activeTypeOverlay);
        _context.Display.AddOverlay(_neighborhoodOverlay);
        _context.Display.AddOverlay(_lineOverlay);

        _context.Map.Tiles.Added += (_, _, _) => UpdateTypeOverlay();
        _neighborhood = new ConnectedNeighborhood(3, _context.Map.Tiles);

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
        
        _context.Maps.TrySave(_context.Map);
        
        base._ExitTree();
    }

    public override void _Input(InputEvent inputEvent)
    {
        var tiles = _context.Map.Tiles;
        var repo = _context.Terrains;

        // Update the active tile and color it accordingly.
        // The active tile is the tile the mouse cursor is currently hovering over.
        // The active tile is colored for highlighting purposes only, this will be removed later on.
        if (inputEvent is InputEventMouseMotion)
        {
            var world = GetGlobalMousePosition();
            var hex = HexConversions.UnitToHex(world / HexConstants.DefaultSize);

            if (_activeTileCoordinates != hex)
            {
                _activeTileCoordinates = hex;
                _activeTileOverlay.Clear();
                _activeTileOverlay.Add(Colors.Aqua, hex);
            }

            _lineOverlay.Clear();
            foreach (var coordinate in Intersections.Line(Vector2.Zero, world / HexConstants.DefaultSize))
            {
                _lineOverlay.Add(Colors.YellowGreen, coordinate);
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
                var world = GetGlobalMousePosition();
                var hex = HexConversions.UnitToHex(world / HexConstants.DefaultSize);
                try
                {
                    var hexTile = new Tile(hex, repo.GetAsset(_activeTileType));
                    var tile = tiles.Get(hex);

                    tiles.Remove(hex);

                    if (tile is not null)
                        RemoveChild(tile.Sprite);

                    tiles.Add(hexTile, hexTile.Coordinates);
                    AddChild(hexTile.Sprite);
                }
                catch (ArgumentException exception)
                {
                    GD.PrintErr(exception.Message);
                }
            }

            // Remove open tiles on right mouse click.
            // Remove other tiles on right mouse click and replace them with an open tile.

            else if (mouseButton is { Pressed: true, ButtonIndex: MouseButton.Right })
            {
                var world = GetGlobalMousePosition();
                var hex = HexConversions.UnitToHex(world / HexConstants.DefaultSize);
                var tile = tiles.Get(hex);

                tiles.Remove(hex);

                if (tile is not null)
                    RemoveChild(tile.Sprite);

                if (tile is null || tile.Key != TileType.Open)
                {
                    try
                    {
                        var hexTile = new Tile(hex, repo.GetAsset(TileType.Open));
                        tiles.Add(hexTile, hexTile.Coordinates);
                        AddChild(hexTile.Sprite);
                    }
                    catch (ArgumentException exception)
                    {
                        GD.PrintErr(exception.Message);
                    }
                }
            }

            // Display neighborhood overlay on the tile that has been clicked.

            else if (mouseButton is { Pressed: true, ButtonIndex: MouseButton.Middle })
            {
                _neighborhoodOverlay.Clear();

                var world = GetGlobalMousePosition();
                var hex = HexConversions.UnitToHex(world / HexConstants.DefaultSize);
                var tile = tiles.Get(hex);

                if (tile is not null)
                {
                    foreach (var neighbor in _neighborhood.GetNeighbors(tile.Coordinates))
                    {
                        _neighborhoodOverlay.Add(Colors.SaddleBrown, neighbor);
                    }
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

        var tiles = _context.Map.Tiles;
        var overlay = tiles.Where(tile => tile.Value.Key == _activeTileType);

        foreach (var tile in overlay)
            _activeTypeOverlay.Add(Colors.LightBlue, tile.Key);
    }

    private AxialCoordinate? _activeTileCoordinates;
    private TileType _activeTileType = TileType.Tundra;

    private EditorContext _context = null!;
    private INeighborhood _neighborhood = null!;

    private readonly ITileLayer<Color> _activeTileOverlay = new TileLayer<Color>();
    private readonly ITileLayer<Color> _activeTypeOverlay = new TileLayer<Color>();
    private readonly ITileLayer<Color> _neighborhoodOverlay = new TileLayer<Color>();
    private readonly ITileLayer<Color> _lineOverlay = new TileLayer<Color>();
}