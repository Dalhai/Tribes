using System;
using System.Linq;
using Godot;
using TribesOfDust.Core.Controllers;
using TribesOfDust.Core.Entities;
using TribesOfDust.Core.Entities.Buildings;
using TribesOfDust.Gen;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;

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
        
        // Generate tiles
        HexMapGenerator generator = new(new(-100, -100), new(100, 100), Context.Repos.Tiles);
        generator.Generate(Context.Map.Tiles);

        // Register tiles
        foreach (var (coordinate, tile) in Context.Map.Tiles)
            CreateSprite(coordinate, tile);
        
        // Register buildings
        var campClass = Context.Repos.Buildings.GetAsset("Camp");
        var camp1 = new Camp(campClass, _player1);
        var camp2 = new Camp(campClass, _player2);

        AxialCoordinate camp1Coordinate = new(-2, -3);
        AxialCoordinate camp2Coordinate = new(5, 4);

        CreateSprite(camp1Coordinate, camp1);
        CreateSprite(camp2Coordinate, camp2);

        var fountainClass = Context.Repos.Buildings.GetAsset("Fountain");
        var fountain1 = new Fountain(fountainClass);
        var fountain2 = new Fountain(fountainClass);

        AxialCoordinate fountain1Coordinate = new(1, -1);
        AxialCoordinate fountain2Coordinate = new(5,  1);

        CreateSprite(fountain1Coordinate, fountain1);
        CreateSprite(fountain2Coordinate, fountain2);

        // Register units
        UnitConfiguration GetUnitConfiguration() => Context.Repos.Units.GetAsset();

        if (camp1.Owner != null)
        {
            var unit1 = new Unit(GetUnitConfiguration(), camp1.Owner);
            var unit2 = new Unit(GetUnitConfiguration(), camp1.Owner);
            var unit3 = new Unit(GetUnitConfiguration(), camp1.Owner);

            AxialCoordinate unit1Coordinate = camp1Coordinate.N;
            AxialCoordinate unit2Coordinate = camp1Coordinate.NE;
            AxialCoordinate unit3Coordinate = camp1Coordinate.NW;

            CreateSprite(unit1Coordinate, unit1);
            CreateSprite(unit2Coordinate, unit2);
            CreateSprite(unit3Coordinate, unit3);
        }

        if (camp2.Owner != null)
        {
            var unit1 = new Unit(GetUnitConfiguration(), camp2.Owner);
            var unit2 = new Unit(GetUnitConfiguration(), camp2.Owner);
            var unit3 = new Unit(GetUnitConfiguration(), camp2.Owner);

            AxialCoordinate unit1Coordinate = camp2Coordinate.N;
            AxialCoordinate unit2Coordinate = camp2Coordinate.NE;
            AxialCoordinate unit3Coordinate = camp2Coordinate.NW;

            CreateSprite(unit1Coordinate, unit1);
            CreateSprite(unit2Coordinate, unit2);
            CreateSprite(unit3Coordinate, unit3);
        }

        base._Ready();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            var position = GetGlobalMousePosition();
            var coordinates = HexConversions.UnitToHex(position / HexConstants.DefaultSize);

            bool hasUnit = Context.Map.Units.Contains(coordinates);

            _selectionOverlay.Clear();
            _selectionOverlay.Add(coordinates, hasUnit
                ? Colors.Blue.Lightened(0.9f)
                : Colors.Red.Lightened(0.9f));
        }
        else if (@event is InputEventMouseButton mouseButton)
        {
            var position = GetGlobalMousePosition();
            var coordinates = HexConversions.UnitToHex(position / HexConstants.DefaultSize);

            // Select a unit

            if (mouseButton.ButtonIndex == MouseButton.Left && Context.Map.Units.Get(coordinates) is { } unit)
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

    public MapContext Context { get; private set; } = null!;
    private readonly Player _player1 = new("Player 1", Colors.Red);
    private readonly Player _player2 = new("Player 2", Colors.Blue);
    private readonly IHexLayer<Color> _selectionOverlay = new HexLayer<Color>();
    private readonly IHexLayer<Color> _movementOverlay = new HexLayer<Color>();
}