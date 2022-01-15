using TribesOfDust.Hex;
using System.Collections.Generic;

namespace TribesOfDust.Hex.Neighborhood
{
    public interface INeighborhood
    {
        /// <summary>
        /// Get all coordinates of the specified tile in this neighborhood.
        /// </summary>
        /// 
        /// <param name="axialCoordinate">The target tile.</param>
        /// <returns>A list of all neighbors in this neighborhood around the tile.</returns>
        List<AxialCoordinate> GetNeighbors(AxialCoordinate axialCoordinate);
    }
}