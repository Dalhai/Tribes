using System;
using Godot;
using TribesOfDust.Core.Controllers;
using TribesOfDust.Core.Entities;
using TribesOfDust.Core.Entities.Buildings;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Modes;

public partial class GameMode : Node2D, IUnique<GameMode>
{
    public static GameMode? Instance { get; private set; }

    public Rect2 GetMapExtents()
    {
        Vector2 minimum = Vector2.Inf;
        Vector2 maximum = -Vector2.Inf;
        foreach (var tile in _context.Map.Hexes)
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
        
        // Register tiles

        foreach (var tile in _context.Map.Hexes)
            AddChild(tile.Value.Sprite);
        
        // Register buildings
        var buildingClass = _context.Repos.Buildings.GetAsset();
        var camp1 = new Camp(new(-2, -3), buildingClass, _player1);
        var camp2 = new Camp(new( 5,  4), buildingClass, _player2);
        
        AddChild(camp1.Sprite);
        AddChild(camp2.Sprite);
        
        // Register units
        var unitClass = _context.Repos.Units.GetAsset();
        
        if (camp1.Owner != null)
        {
            var unit1 = new Unit(camp1.Coordinates.N, unitClass, camp1.Owner);
            var unit2 = new Unit(camp1.Coordinates.NE, unitClass, camp1.Owner);
            var unit3 = new Unit(camp1.Coordinates.SE, unitClass, camp1.Owner);
        
            _context.Map.Units.Add(unit1, unit1.Coordinates);
            _context.Map.Units.Add(unit2, unit2.Coordinates);
            _context.Map.Units.Add(unit3, unit3.Coordinates);
        
            AddChild(unit1.Sprite);
            AddChild(unit2.Sprite);
            AddChild(unit3.Sprite);
        }
        
        if (camp2.Owner != null)
        {
            var unit1 = new Unit(camp2.Coordinates.N, unitClass, camp2.Owner);
            var unit2 = new Unit(camp2.Coordinates.NE, unitClass, camp2.Owner);
            var unit3 = new Unit(camp2.Coordinates.SE, unitClass, camp2.Owner);
        
            _context.Map.Units.Add(unit1, unit1.Coordinates);
            _context.Map.Units.Add(unit2, unit2.Coordinates);
            _context.Map.Units.Add(unit3, unit3.Coordinates);
        
            AddChild(unit1.Sprite);
            AddChild(unit2.Sprite);
            AddChild(unit3.Sprite);
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
        base._ExitTree();
    }

    private MapContext _context = null!;
    private Player _player1 = new("Player 1", Colors.Red);
    private Player _player2 = new("Player 2", Colors.Blue);
}