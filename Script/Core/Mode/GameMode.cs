using System;
using Godot;
using TribesOfDust.Core.Entities;
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
}