using TribesOfDust.Core.Controllers;
using TribesOfDust.Core.Entities;

namespace TribesOfDust.Core;

public class Tile : IEntity<TileConfiguration>
{
    public Tile(TileConfiguration configuration)
    {
        Identity      = Identities.GetNextIdentity();
        Configuration = configuration;
    }

    /// <summary>
    /// The unique identity of the entity.
    /// </summary>
    public ulong Identity { get; }

    /// <summary>
    /// The configuration of the entity.
    /// </summary>
    public TileConfiguration Configuration { get; }

    /// <summary>
    /// The owner of the entity.
    /// </summary>
    public IController? Owner { get; } = null;
}