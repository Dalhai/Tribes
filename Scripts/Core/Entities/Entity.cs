using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities;

public abstract class Entity<TConfiguration> : IEntity<TConfiguration>
{
    protected Entity(TConfiguration configuration, AxialCoordinate location, IController? owner = null)
    {
        Identity      = Identities.GetNextIdentity();
        
        Owner         = owner;
        Configuration = configuration;
        Location      = location;
    }
    
    /// <summary>
    /// The unique identity of the entity.
    /// </summary>
    public ulong Identity { get; }

    /// <summary>
    /// The owner of the entity.
    /// </summary>
    public IController? Owner { get; }

    /// <summary>
    /// The configuration of the entity.
    /// </summary>
    public TConfiguration Configuration { get; }

    /// <summary>
    /// The location of the entity.
    /// </summary>
    public AxialCoordinate Location { get; set; }
}