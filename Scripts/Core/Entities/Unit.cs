using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities;

public class Unit : Entity<UnitConfiguration>
{
    /// <summary>
    /// Creates a new unit instance.
    /// </summary>
    /// 
    /// <param name="configuration">The configuration used for the unit.</param>
    /// <param name="location">The location the unit is initially placed at.</param>
    /// <param name="owner">The owner controlling the unit within the world.</param>
    public Unit(UnitConfiguration configuration, AxialCoordinate location, IController owner)
        : base(configuration, location, owner)
    {
    }
    
    public double Health { get; set; } = 10;
    public double MaxHealth { get; } = 10;
    public double Water { get; set; } = 10;
    public double MaxWater { get; } = 10;
    public double Speed { get; } = 5;
}