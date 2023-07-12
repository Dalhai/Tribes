using Godot;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities;

public class Unit : IEntity
{
    #region Constructors
    
    public Unit(AxialCoordinate coordinates, UnitClass @class)
    {
        Coordinates = coordinates;
        
        Identity = Identities.GetNextIdentity();
        
        Sprite = new();
        Sprite.Texture = @class.Texture2D;
        Sprite.Modulate = Colors.Beige;
        Sprite.ZIndex = 10;
        
        // Initialize and reduce scale so the unit sprite fits
        
        Sprite.Scale = new Vector2(@class.WidthScaleToExpected, @class.HeightScaleToExpected);
        Sprite.Scale *= 0.5f;
        
        // Position unit according to specified coordinates

        Sprite.Centered = true;
        Sprite.Position = HexConversions.HexToUnit(coordinates) * HexConstants.DefaultSize;
    }
    
    #endregion
    #region Queries
    
    public ulong Identity { get; }
    
    public AxialCoordinate Coordinates { get; }
    public Sprite2D Sprite { get; }
    
    #endregion
}