using System.Collections.Generic;
using System.Linq;

using TribesOfDust.Hex.Storage;

namespace TribesOfDust.Hex.Neighborhood
{
    public class Neighborhood : INeighborhood
    {
        public Neighborhood(ITileStorageView storage) => Storage = storage;

        public ITileStorageView Storage { get; set; }

        /// <summary>
        /// Get all coordinates of the specified tile in this neighborhood.
        /// </summary>
        /// 
        /// <param name="axialCoordinate">The target tile.</param>
        /// <returns>A list of all neighbors in this neighborhood around the tile.</returns>
        public List<AxialCoordinate> GetNeighbors(AxialCoordinate axialCoordinate)
        {
            List<AxialCoordinate> neighbors = new();

            neighbors.Add(axialCoordinate + AxialCoordinate.N);
            neighbors.Add(axialCoordinate + AxialCoordinate.NE);
            neighbors.Add(axialCoordinate + AxialCoordinate.NW);
            neighbors.Add(axialCoordinate + AxialCoordinate.S);
            neighbors.Add(axialCoordinate + AxialCoordinate.SE);
            neighbors.Add(axialCoordinate + AxialCoordinate.SW);

            return neighbors.Where(Storage.Contains).ToList();
        }
    }
}