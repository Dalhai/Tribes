using static System.Diagnostics.Debug;

using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;

namespace TribesOfDust.Core.Entities;

public abstract class Building : IEntity<BuildingConfiguration>
{
    protected Building(BuildingConfiguration configuration, IController? owner)
    {
        Identity      = Identities.GetNextIdentity();
        Configuration = configuration;
        Owner         = owner;
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
    /// The owner of the entity.
    /// </summary>
    public IController? Owner { get; }
}