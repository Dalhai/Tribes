using static System.Diagnostics.Debug;

using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;

namespace TribesOfDust.Core.Entities;

public class Building : IEntity<BuildingConfiguration>
{
    public Building(BuildingConfiguration configuration, IPlacement placement, IController? owner)
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
    public BuildingConfiguration Configuration { get; }

    /// <summary>
    /// The location of the entity.
    /// </summary>
    public AxialCoordinate? Location => _placement.Location;

    /// <summary>
    /// The owner of the entity.
    /// </summary>
    public IController? Owner { get; }

    private readonly IPlacement _placement;
}