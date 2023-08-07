using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities;

public abstract class Building(AxialCoordinate location, BuildingConfiguration configuration, IController? owner)
    : IEntity<BuildingConfiguration>
{
    public ulong Identity { get; } = Identities.GetNextIdentity();
    public IController? Owner { get; } = owner;
    public BuildingConfiguration Configuration => configuration;
    public AxialCoordinate Location { get; } = location;
}