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
    }
    
    #endregion
    #region Queries
    
    public ulong Identity { get; }
    
    public AxialCoordinate Coordinates { get; }
    public Sprite2D Sprite { get; }
    
    #endregion
}