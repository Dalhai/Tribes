using TribesOfDust.Core.Controllers;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex;

namespace TribesOfDust.Core;

public class Tile : Entity<TileConfiguration>
{
    /// <summary>
    /// Creates a new tile at the specified location.
    /// </summary>
    /// 
    /// <param name="configuration">The kind of tile to create.</param>
    /// <param name="location">The location the tile will initially be situated at.</param>
    /// <param name="owner">The owning controller of the tile, either player or non-player.</param>
    public Tile(TileConfiguration configuration, AxialCoordinate location, IController? owner = null)
        : base(configuration, location, owner)
    {
    }
}