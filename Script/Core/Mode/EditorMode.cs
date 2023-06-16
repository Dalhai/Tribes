using System;
using System.Linq;
using Godot;

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
        foreach (var tile in _context.Level.Tiles)
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
        _context.Level.Map = Load(_context.Repositories);

        foreach (var tile in _context.Level.Tiles)
        {
            AddChild(tile.Value.Sprite);
        }

        // Register overlays with context   
        _context.Display.AddOverlay(_activeTileOverlay);
        _context.Display.AddOverlay(_activeTypeOverlay);
        _context.Display.AddOverlay(_neighborhoodOverlay);
        _context.Display.AddOverlay(_lineOverlay);

        _context.Level.Tiles.Added += (_, _) => UpdateTypeOverlay();
        _neighborhood = new ConnectedNeighborhood(3, _context.Level.Tiles);

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
        
        Save(_context.Level);
        base._ExitTree();
    }

    private void Save(Level level)
    {
        level.Map ??= new("World");
        level.Map.Tiles.Clear();

        foreach (var tile in level.Tiles)
        {
            level.Map.Tiles[tile.Key] = tile.Value.Key;
        }

        _context.Repositories.Maps.TrySave(level.Map);
    }

    private static Map Load(Repositories repositories) => repositories.Maps.First();

    public override void _Input(InputEvent inputEvent)
    {
        var tiles = _context.Level.Tiles;
        var repo = _context.Repositories.Terrains;

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
                _activeTileOverlay.Add(hex, Colors.Aqua);
            }

            _lineOverlay.Clear();
            foreach (var coordinate in Intersections.Line(Vector2.Zero, world / HexConstants.DefaultSize))
            {
                _lineOverlay.Add(coordinate, Colors.YellowGreen);
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
                    var hexTile = Tile.Create(hex, repo.GetAsset(_activeTileType));
                    var tile = tiles.Get(hex);

                    tiles.Remove(hex);

                    if (tile is not null)
                        RemoveChild(tile.Sprite);

                    tiles.Add(hexTile.Coordinates, hexTile);
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
                        var hexTile = Tile.Create(hex, repo.GetAsset(TileType.Open));
                        tiles.Add(hexTile.Coordinates, hexTile);
                        AddChild(hexTile.Sprite);
                    }
                    catch (ArgumentException exception)
                    {
                        GD.PrintErr(exception.Message);
                    }
                }
            }

            // Display neighborhood overlay on the tile that has been clicked.

            else if (mouseButton is { Pressed: true, ButtonIndex: MouseButton.Middle } && _neighborhood is not null)
            {
                _neighborhoodOverlay.Clear();

                var world = GetGlobalMousePosition();
                var hex = HexConversions.UnitToHex(world / HexConstants.DefaultSize);
                var tile = tiles.Get(hex);

                if (tile is not null)
                {
                    foreach (var neighbor in _neighborhood.GetNeighbors(tile.Coordinates))
                    {
                        _neighborhoodOverlay.Add(neighbor, Colors.SaddleBrown);
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

        var tiles = _context.Level.Tiles;
        var overlay = tiles.Where(tile => tile.Value.Key == _activeTileType);

        foreach (var tile in overlay)
            _activeTypeOverlay.Add(tile.Key, Colors.LightBlue);
    }

    private AxialCoordinate? _activeTileCoordinates;
    private TileType _activeTileType = TileType.Tundra;

    private EditorContext _context = null!;
    private INeighborhood _neighborhood = null!;

    private readonly ITileStorage<Color> _activeTileOverlay = new TileStorage<Color>();
    private readonly ITileStorage<Color> _activeTypeOverlay = new TileStorage<Color>();
    private readonly ITileStorage<Color> _neighborhoodOverlay = new TileStorage<Color>();
    private readonly ITileStorage<Color> _lineOverlay = new TileStorage<Color>();
}