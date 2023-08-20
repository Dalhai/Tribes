using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities;

public class Unit : IEntity<UnitConfiguration>
{
    public Unit(UnitConfiguration configuration, IPlacement placement, IController owner)
    {
        Identity      = Identities.GetNextIdentity();
        Configuration = configuration;
        Owner         = owner;
        
        // Keep track of the placement of the unit.
        // Can only be set from the outside.
        _placement = placement;
    }

    /// <summary>
    /// The unique identity of the entity.
    /// </summary>
    public ulong Identity { get; }

    /// <summary>
    /// The configuration of the entity.
    /// </summary>
    public UnitConfiguration Configuration { get; }

    /// <summary>
    /// The location of the entity.
    /// </summary>
    public AxialCoordinate? Location => _placement.Location;

    /// <summary>
    /// The owner of the entity.
    /// </summary>
    public IController? Owner { get; }
    
    public double Health { get; set; } = 10;
    public double MaxHealth { get; } = 10;
    public double Water { get; set; } = 10;
    public double MaxWater { get; } = 10;
    public double Speed { get; } = 5;
    
    private readonly IPlacement _placement;
}