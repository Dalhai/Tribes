using System;
using Godot;
using TribesOfDust.Core.Controllers;
using TribesOfDust.Core.Entities;
using TribesOfDust.Core.Entities.Buildings;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Storage;

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
        _context = new MapContext(Context.Instance);
        _context.Display.AddOverlay(_selectionOverlay);
        _context.Display.AddOverlay(_movementOverlay);
        
        // Register tiles

        foreach (var tile in _context.Map.Tiles)
            AddChild(tile.Value.Sprite);
        
        // Register buildings
        var campClass = _context.Repos.Buildings.GetAsset("Camp");
        var camp1 = new Camp(new(-2, -3), campClass, _player1);
        var camp2 = new Camp(new(5, 4), campClass, _player2);

        AddChild(camp1.Sprite);
        AddChild(camp2.Sprite);

        var fountainClass = _context.Repos.Buildings.GetAsset("Fountain");
        var fountain1 = new Fountain(new (1, -1), fountainClass);
        var fountain2 = new Fountain(new (5,  1), fountainClass);
        
        AddChild(fountain1.Sprite);
        AddChild(fountain2.Sprite);
        
        // Register units
        UnitClass GetUnitClass() => _context.Repos.Units.GetAsset();

        if (camp1.Owner != null)
        {
            var unit1 = new Unit(camp1.Coordinates.N, GetUnitClass(), camp1.Owner);
            var unit2 = new Unit(camp1.Coordinates.NE, GetUnitClass(), camp1.Owner);
            var unit3 = new Unit(camp1.Coordinates.SE, GetUnitClass(), camp1.Owner);
        
            _context.Map.Units.Add(unit1, unit1.Coordinates);
            _context.Map.Units.Add(unit2, unit2.Coordinates);
            _context.Map.Units.Add(unit3, unit3.Coordinates);
        
            AddChild(unit1.Sprite);
            AddChild(unit2.Sprite);
            AddChild(unit3.Sprite);
        }
        
        if (camp2.Owner != null)
        {
            var unit1 = new Unit(camp2.Coordinates.N, GetUnitClass(), camp2.Owner);
            var unit2 = new Unit(camp2.Coordinates.NE, GetUnitClass(), camp2.Owner);
            var unit3 = new Unit(camp2.Coordinates.SE, GetUnitClass(), camp2.Owner);
        
            _context.Map.Units.Add(unit1, unit1.Coordinates);
            _context.Map.Units.Add(unit2, unit2.Coordinates);
            _context.Map.Units.Add(unit3, unit3.Coordinates);
        
            AddChild(unit1.Sprite);
            AddChild(unit2.Sprite);
            AddChild(unit3.Sprite);
        }

        base._Ready();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            var position = GetGlobalMousePosition();
            var coordinates = HexConversions.UnitToHex(position / HexConstants.DefaultSize);
            
            bool hasUnit = _context.Map.Units.Contains(coordinates);
            
            _selectionOverlay.Clear();
            _selectionOverlay.Add(
                hasUnit
                    ? Colors.Blue.Lightened(0.9f) 
                    : Colors.Red.Lightened(0.9f),
                coordinates);
        }
        else if (@event is InputEventMouseButton mouseButton)
        {
            var position = GetGlobalMousePosition();
            var coordinates = HexConversions.UnitToHex(position / HexConstants.DefaultSize);

            if (mouseButton.ButtonIndex == MouseButton.Left && _context.Map.Units.Get(coordinates) is {} unit)
            {
                if (_context.Selected is Unit previousUnit)
                    previousUnit.Sprite.Modulate = previousUnit.Owner?.Color ?? Colors.White;
                
                _context.Selected = unit;
                unit.Sprite.Modulate = Colors.Yellow;
                
                Label? healthLabel = GetNode<Label>(HealthPath);
                Label? waterLabel = GetNode<Label>(WaterPath);

                if (healthLabel is { } && waterLabel is { })
                {
                    healthLabel.Text = $"{unit.Health} / {unit.MaxHealth}";
                    waterLabel.Text = $"{unit.Water} / {unit.MaxWater}";
                }
                
                // Update movement overlay
                _movementOverlay.Clear();
                foreach (var (coordinate, cost) in unit.ComputerReachable(_context.Map.Tiles))
                    _movementOverlay.Add(Colors.Aqua.Lightened((float)(cost / unit.Water)), coordinate);
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

    private MapContext _context = null!;
    private readonly Player _player1 = new("Player 1", Colors.Red);
    private readonly Player _player2 = new("Player 2", Colors.Blue);
    private readonly IHexLayer<Color> _selectionOverlay = new HexLayer<Color>();
    private readonly IHexLayer<Color> _movementOverlay = new HexLayer<Color>();
}