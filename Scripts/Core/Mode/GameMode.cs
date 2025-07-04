using System.Collections.Generic;
using Godot;
using TribesOfDust.Core.Controllers;
using TribesOfDust.Core.Entities;
using TribesOfDust.Gen;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;
using TribesOfDust.Utils;

namespace TribesOfDust.Core.Modes;

struct FakeGenerator : IHexLayerGenerator<Tile>
{
    public TileConfigurationRepository Repository { get; init; }
    
    public bool Generate(IHexLayer<Tile> layer)
    {
        var config = Repository.GetAsset();
        
        GD.Print($"tile1: {AxialCoordinate.Zero}");
        var tile1 = new Tile(config, AxialCoordinate.Zero);
        layer.TryAdd(tile1.Location, tile1);

        GD.Print($"tile2: {AxialCoordinate.Zero.NW}");
        var tile2 = new Tile(config, AxialCoordinate.Zero.NW);
        layer.TryAdd(tile2.Location, tile2);

        GD.Print($"tile3: {AxialCoordinate.Zero.SE}");
        var tile3 = new Tile(config, AxialCoordinate.Zero.SE);
        layer.TryAdd(tile3.Location, tile3);
        
        return true;
    }
}

public partial class GameMode : Node2D, IUnique<GameMode>
{
    [Export] public NodePath? HealthPath;
    [Export] public NodePath? WaterPath;

    public static GameMode? Instance { get; private set; }
    
    private HexMap _hexMap = null!;
    
    /// <summary>
    /// The HexMap responsible for rendering terrain tiles.
    /// </summary>
    public HexMap HexMap => _hexMap ??= GetHexMap();

    public override void _Ready()
    {
        Context = new MapContext(Core.Context.Instance);
        Context.Display.AddOverlay(_selectionOverlay);
        Context.Display.AddOverlay(_movementOverlay);
        
        var map = Context.Map;
        var repo = Context.Repos;
        
        // Generate tiles
        // HexMapGenerator generator = new(new(-100, -100), new(100, 100), Context.Repos.Tiles);
        // map.Generate(generator);
        FakeGenerator fake = new() { Repository = repo.Tiles };
        map.Generate(fake);

        // Initialize the HexMap and sync tiles
        _hexMap = GetHexMap();
        _hexMap.ConnectToMap(Context.Map);
        
        // Connect HexMap to Display for overlay support
        Context.Display.HexMap = _hexMap;
        
        // Get tile size for sprite positioning and scaling
        var tileSize = _hexMap.TerrainLayer.TileSet.GetTileSize();
        
        // Register buildings
        var campClass = repo.Buildings.GetAsset("Camp");
        var camp1 = new Building(campClass, new(0, 0), _player1);
        var camp2 = new Building(campClass, new(5, 4), _player2);

        map.TryAddEntity(camp1);
        map.TryAddEntity(camp2);

        // Create sprites for camp buildings
        var camp1Sprite = this.CreateSpriteForEntity(camp1, tileSize);
        if (camp1Sprite != null) _sprites.Add(camp1.Identity, camp1Sprite);
        
        var camp2Sprite = this.CreateSpriteForEntity(camp2, tileSize);
        if (camp2Sprite != null) _sprites.Add(camp2.Identity, camp2Sprite);

        var fountainClass = repo.Buildings.GetAsset("Fountain");
        var fountain1 = new Building(fountainClass, new(1, -3));
        var fountain2 = new Building(fountainClass, new(5, 1));

        map.TryAddEntity(fountain1);
        map.TryAddEntity(fountain2);

        // Create sprites for fountain buildings
        var fountain1Sprite = this.CreateSpriteForEntity(fountain1, tileSize);
        if (fountain1Sprite != null) _sprites.Add(fountain1.Identity, fountain1Sprite);
        
        var fountain2Sprite = this.CreateSpriteForEntity(fountain2, tileSize);
        if (fountain2Sprite != null) _sprites.Add(fountain2.Identity, fountain2Sprite);

        // Register units
        UnitConfiguration GetUnitConfiguration() => Context.Repos.Units.GetAsset();

        if (camp1.Owner != null)
        {
            var unit1 = new Unit(GetUnitConfiguration(), camp1.Location.N, camp1.Owner);
            var unit2 = new Unit(GetUnitConfiguration(), camp1.Location.NE, camp1.Owner);
            var unit3 = new Unit(GetUnitConfiguration(), camp1.Location.NW, camp1.Owner);

            map.TryAddEntity(unit1);
            map.TryAddEntity(unit2);
            map.TryAddEntity(unit3);

            // Create sprites for units
            var unit1Sprite = this.CreateSpriteForEntity(unit1, tileSize);
            if (unit1Sprite != null) _sprites.Add(unit1.Identity, unit1Sprite);
            
            var unit2Sprite = this.CreateSpriteForEntity(unit2, tileSize);
            if (unit2Sprite != null) _sprites.Add(unit2.Identity, unit2Sprite);
            
            var unit3Sprite = this.CreateSpriteForEntity(unit3, tileSize);
            if (unit3Sprite != null) _sprites.Add(unit3.Identity, unit3Sprite);
        }

        if (camp2.Owner != null)
        {
            var unit1 = new Unit(GetUnitConfiguration(), camp2.Location.N, camp2.Owner);
            var unit2 = new Unit(GetUnitConfiguration(), camp2.Location.NE, camp2.Owner);
            var unit3 = new Unit(GetUnitConfiguration(), camp2.Location.NW, camp2.Owner);

            map.TryAddEntity(unit1);
            map.TryAddEntity(unit2);
            map.TryAddEntity(unit3);

            // Create sprites for units
            var unit1Sprite = this.CreateSpriteForEntity(unit1, tileSize);
            if (unit1Sprite != null) _sprites.Add(unit1.Identity, unit1Sprite);
            
            var unit2Sprite = this.CreateSpriteForEntity(unit2, tileSize);
            if (unit2Sprite != null) _sprites.Add(unit2.Identity, unit2Sprite);
            
            var unit3Sprite = this.CreateSpriteForEntity(unit3, tileSize);
            if (unit3Sprite != null) _sprites.Add(unit3.Identity, unit3Sprite);
        }

        base._Ready();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            var mousePosition = GetGlobalMousePosition();
            var hoveredLocation = HexMap.WorldToHexCoordinate(mousePosition);

            bool hasUnit = Context.Map.Units.Contains(hoveredLocation);

            _selectionOverlay.Clear();
            _selectionOverlay.TryAdd(hoveredLocation, hasUnit
                ? Colors.Blue.Lightened(0.9f)
                : Colors.Red.Lightened(0.9f));
        }
        else if (@event is InputEventMouseButton mouseButton)
        {
            var mousePosition = GetGlobalMousePosition();
            var clickedLocation = HexMap.WorldToHexCoordinate(mousePosition);

            // Select a unit

            if (mouseButton.ButtonIndex == MouseButton.Left && Context.Map.Units.Get(clickedLocation) is { } unit)
            {
                if (Context.Selected is Unit previousUnit)
                    _sprites[previousUnit.Identity].Modulate = previousUnit.Owner?.Color ?? Colors.White;

                Context.Selected = unit;
                _sprites[unit.Identity].Modulate = Colors.Yellow;

                Label? healthLabel = GetNode<Label>(HealthPath);
                Label? waterLabel = GetNode<Label>(WaterPath);

                if (healthLabel is { } && waterLabel is { })
                {
                    healthLabel.Text = $"{unit.Health} / {unit.MaxHealth}";
                    waterLabel.Text = $"{unit.Water} / {unit.MaxWater}";
                }
            }
        }
    }

    public override void _EnterTree()
    {
        Instance = this;
        base._EnterTree();
    }

    public override void _ExitTree()
    {
        Instance = null;
        base._ExitTree();
    }

    public MapContext Context { get; private set; } = null!;
    private readonly Player _player1 = new("Player 1", Colors.Red);
    private readonly Player _player2 = new("Player 2", Colors.Blue);
    private readonly IHexLayer<Color> _selectionOverlay = new HexLayer<Color>();
    private readonly IHexLayer<Color> _movementOverlay = new HexLayer<Color>();
    
    /// <summary>
    /// Dictionary of sprites for non-tile entities (buildings, units) used for selection and visual effects.
    /// </summary>
    private readonly Dictionary<ulong, Sprite2D> _sprites = new();
    
    /// <summary>
    /// Gets or creates the HexMap for this game.
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
}