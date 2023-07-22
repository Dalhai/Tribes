using Godot;
using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities;

public abstract class Building : IEntity
{
    #region Constructors
    
    protected Building(AxialCoordinate coordinates, BuildingClass @class, IController owner)
    {
        Coordinates = coordinates;
        Owner = owner;
        
        Identity = Identities.GetNextIdentity();
        
        Sprite = new();
        Sprite.Texture = @class.Texture2D;
        Sprite.Modulate = owner.Color;
        Sprite.ZIndex = 10;
        
        // Initialize and reduce scale so the building sprite fits

        float widthScaleToExpected = @class.Texture2D != null ? HexConstants.DefaultWidth / @class.Texture2D.GetWidth() : 1.0f;
        float heightScaleToExpected = @class.Texture2D != null ? HexConstants.DefaultHeight / @class.Texture2D.GetHeight() : 1.0f;
        
        Sprite.Scale = new Vector2(widthScaleToExpected, heightScaleToExpected);
        Sprite.Scale *= 0.5f;
        
        // Position unit according to specified coordinates

        Sprite.Centered = true;
        Sprite.Position = HexConversions.HexToUnit(coordinates) * HexConstants.DefaultSize;
    }
    
    #endregion
    #region Queries
    
    public ulong Identity { get; }
    public IController? Owner { get; }
    
    public AxialCoordinate Coordinates { get; }
    public Sprite2D Sprite { get; }
    
    #endregion
}