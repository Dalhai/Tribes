using Godot;
using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities;

public class Unit : IEntity
{
    #region Constructors
    
    public Unit(AxialCoordinate coordinates, UnitConfiguration configuration, IController owner)
    {
        Configuration = configuration;
        
        // Initialize position and look
        
        Identity = Identities.GetNextIdentity();
        
        Sprite = new();
        Sprite.Texture = configuration.Texture;
        Sprite.Modulate = owner.Color;
        Sprite.ZIndex = 10;
        
        // Initialize and reduce scale so the unit sprite fits

        float widthScaleToExpected = configuration.Texture != null ? HexConstants.DefaultWidth / configuration.Texture.GetWidth() : 1.0f;
        float heightScaleToExpected = configuration.Texture != null ? HexConstants.DefaultHeight / configuration.Texture.GetHeight() : 1.0f;
        
        Sprite.Scale = new Vector2(widthScaleToExpected, heightScaleToExpected);
        Sprite.Scale *= 0.5f;
        
        // Position unit according to specified coordinates

        Sprite.Centered = true;
        
        Coordinates = coordinates;
        Owner = owner;
        
        // Initialize stats

        Health = 10;
        MaxHealth = 10;

        Water = 10;
        MaxWater = 10;

        Speed = 5;
    }
    
    #endregion
    #region Data
    
    public ulong Identity { get; }
    public IController? Owner { get; }

    private AxialCoordinate _coordinates;
    public AxialCoordinate Coordinates
    {
        get => _coordinates;
        set
        {
            _coordinates = value;
            Sprite.Position = HexConversions.HexToUnit(value) * HexConstants.DefaultSize;
        }
    }
    
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

    public readonly UnitConfiguration Configuration;

    #endregion
}