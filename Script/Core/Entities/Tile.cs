using TribesOfDust.Core.Controllers;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex;

namespace TribesOfDust.Core;

public class Tile : IEntity<TileConfiguration>
{
    public Tile(TileConfiguration configuration, IPlacement placement)
    {
        Identity      = Identities.GetNextIdentity();
        Configuration = configuration;
        
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
    public TileConfiguration Configuration { get; }

    /// <summary>
    /// The location of the entity.
    /// </summary>
    public AxialCoordinate? Location => _placement.Location;

    /// <summary>
    /// The owner of the entity.
    /// </summary>
    public IController? Owner { get; } = null;

    private readonly IPlacement _placement;
}