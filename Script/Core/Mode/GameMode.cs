using System;
using Godot;
using TribesOfDust.Core.Controllers;
using TribesOfDust.Core.Entities;
using TribesOfDust.Gen;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;
using TribesOfDust.Utils;

namespace TribesOfDust.Core.Modes;

public partial class GameMode : Node2D, IUnique<GameMode>
{
    [Export] public NodePath? HealthPath;
    [Export] public NodePath? WaterPath;

    public static GameMode? Instance { get; private set; }

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
        Context.Display.AddOverlay(_selectionOverlay);
        Context.Display.AddOverlay(_movementOverlay);
        
        var map = Context.Map;
        var repo = Context.Repos;
        
        // Generate tiles
        HexMapGenerator generator = new(new(-100, -100), new(100, 100), Context.Repos.Tiles);
        map.Generate(generator);

        // Register tiles
        foreach (var (_, tile) in Context.Map.Tiles)
            this.CreateSpriteForEntity(Context, tile);
        
        // Register buildings
        var campClass = repo.Buildings.GetAsset("Camp");
        var camp1 = new Building(campClass, new(-2, -3), _player1);
        var camp2 = new Building(campClass, new(5, 4), _player2);

        map.TryAddEntity(camp1);
        map.TryAddEntity(camp2);

        this.CreateSpriteForEntity(Context, camp1);
        this.CreateSpriteForEntity(Context, camp2);

        var fountainClass = repo.Buildings.GetAsset("Fountain");
        var fountain1 = new Building(fountainClass, new(1, -1));
        var fountain2 = new Building(fountainClass, new(5, 1));

        map.TryAddEntity(fountain1);
        map.TryAddEntity(fountain2);

        this.CreateSpriteForEntity(Context, fountain1);
        this.CreateSpriteForEntity(Context, fountain2);

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

            this.CreateSpriteForEntity(Context, unit1);
            this.CreateSpriteForEntity(Context, unit2);
            this.CreateSpriteForEntity(Context, unit3);
        }

        if (camp2.Owner != null)
        {
            var unit1 = new Unit(GetUnitConfiguration(), camp2.Location.N, camp2.Owner);
            var unit2 = new Unit(GetUnitConfiguration(), camp2.Location.NE, camp2.Owner);
            var unit3 = new Unit(GetUnitConfiguration(), camp2.Location.NW, camp2.Owner);

            map.TryAddEntity(unit1);
            map.TryAddEntity(unit2);
            map.TryAddEntity(unit3);

            this.CreateSpriteForEntity(Context, unit1);
            this.CreateSpriteForEntity(Context, unit2);
            this.CreateSpriteForEntity(Context, unit3);
        }

        base._Ready();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            var mousePosition = GetGlobalMousePosition();
            var clickedLocation = HexConversions.UnitToHex(mousePosition / HexConstants.DefaultSize);

            bool hasUnit = Context.Map.Units.Contains(clickedLocation);

            _selectionOverlay.Clear();
            _selectionOverlay.TryAdd(clickedLocation, hasUnit
                ? Colors.Blue.Lightened(0.9f)
                : Colors.Red.Lightened(0.9f));
        }
        else if (@event is InputEventMouseButton mouseButton)
        {
            var mousePosition = GetGlobalMousePosition();
            var clickedLocation = HexConversions.UnitToHex(mousePosition / HexConstants.DefaultSize);

            // Select a unit

            if (mouseButton.ButtonIndex == MouseButton.Left && Context.Map.Units.Get(clickedLocation) is { } unit)
            {
                if (Context.Selected is Unit previousUnit)
                    Context.Display.Sprites[previousUnit.Identity].Modulate = previousUnit.Owner?.Color ?? Colors.White;

                Context.Selected = unit;
                Context.Display.Sprites[unit.Identity].Modulate = Colors.Yellow;

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
}