using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities;

public abstract class Building(AxialCoordinate coordinates, BuildingConfiguration configuration, IController? owner)
    : IEntity
{
    #region Queries
    
    public ulong Identity { get; } = Identities.GetNextIdentity();
    public IController? Owner { get; } = owner;
    public AxialCoordinate Coordinates { get; } = coordinates;
    public readonly BuildingConfiguration Configuration = configuration;

    #endregion
}