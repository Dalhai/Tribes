using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities;

public class Building : Entity<BuildingConfiguration>
{
    /// <summary>
    /// Creates a new building at the specified location.
    /// </summary>
    /// 
    /// <param name="configuration">The kind of building to create.</param>
    /// <param name="location">The location the building will initially be situated at.</param>
    /// <param name="owner">The owning controller of the building, either player or non-player.</param>
    public Building(BuildingConfiguration configuration, AxialCoordinate location, IController? owner = null)
        : base(configuration, location, owner)
    {
    }
}