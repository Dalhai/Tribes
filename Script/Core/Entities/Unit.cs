using Godot;
using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities;

public class Unit : IEntity
{
    #region Constructors
    
    public Unit(AxialCoordinate coordinates, UnitClass @class, IController owner)
    {
        _class = @class;
        
        // Initialize position and look
        
        Coordinates = coordinates;
        Owner = owner;
        
        Identity = Identities.GetNextIdentity();
        
        Sprite = new();
        Sprite.Texture = @class.Texture2D;
        Sprite.Modulate = owner.Color;
        Sprite.ZIndex = 10;
        
        // Initialize and reduce scale so the unit sprite fits

        float widthScaleToExpected = @class.Texture2D != null ? HexConstants.DefaultWidth / @class.Texture2D.GetWidth() : 1.0f;
        float heightScaleToExpected = @class.Texture2D != null ? HexConstants.DefaultHeight / @class.Texture2D.GetHeight() : 1.0f;
        
        Sprite.Scale = new Vector2(widthScaleToExpected, heightScaleToExpected);
        Sprite.Scale *= 0.5f;
        
        // Position unit according to specified coordinates

        Sprite.Centered = true;
        Sprite.Position = HexConversions.HexToUnit(coordinates) * HexConstants.DefaultSize;
        
        // Initialize stats

        Health = 10;
        MaxHealth = 10;

        Water = 10;
        MaxWater = 10;

        Speed = 5;
    }
    
    #endregion
    #region Queries
    
    public ulong Identity { get; }
    public IController? Owner { get; }

    public AxialCoordinate Coordinates { get; }
    public Sprite2D Sprite { get; }
    
    #endregion
    #region Stats
    
    public double Health { get; set; }
    public double MaxHealth { get; }
    
    public double Water { get; set; }
    public double MaxWater { get; }
    
    public double Speed { get; }
    
    #endregion
    #region Class

    private readonly UnitClass _class;

    #endregion
}