using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities;

public class Unit(AxialCoordinate location, UnitConfiguration configuration, IController owner)
    : IEntity<UnitConfiguration>
{
    public ulong Identity { get; } = Identities.GetNextIdentity();
    public IController? Owner { get; } = owner;
    public UnitConfiguration Configuration => configuration;
    public AxialCoordinate Location { get; } = location;

    public double Health { get; set; } = 10;
    public double MaxHealth { get; } = 10;
    public double Water { get; set; } = 10;
    public double MaxWater { get; } = 10;
    public double Speed { get; } = 5;
}