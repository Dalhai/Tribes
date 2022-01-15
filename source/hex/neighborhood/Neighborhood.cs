using System.Collections.Generic;
using System.Linq;

using TribesOfDust.Hex.Storage;

namespace TribesOfDust.Hex.Neighborhood
{
    public class Neighborhood : INeighborhood
    {
        public Neighborhood(ITileStorageView storage, uint distance) 
        {
            Storage = storage;
            Distance = distance;
        }

        public ITileStorageView Storage { get; init; }
        public uint Distance { get; init; }

        /// <summary>
        /// Get all coordinates of the specified tile in this neighborhood.
        /// </summary>
        /// 
        /// <param name="axialCoordinate">The target tile.</param>
        /// <returns>A list of all neighbors in this neighborhood around the tile.</returns>
        public List<AxialCoordinate> GetNeighbors(AxialCoordinate axialCoordinate)
        {
            List<AxialCoordinate> neighbors = new();
            List<AxialCoordinate> current = new();
            List<AxialCoordinate> previous = new();

            previous.Add(axialCoordinate);
            neighbors.Add(axialCoordinate);

            for(int distance = 0; distance < Distance; distance++)
            {
                foreach(var coordinate in previous)
                {
                    current.Add(coordinate + AxialCoordinate.N);
                    current.Add(coordinate + AxialCoordinate.NE);
                    current.Add(coordinate + AxialCoordinate.NW);
                    current.Add(coordinate + AxialCoordinate.S);
                    current.Add(coordinate + AxialCoordinate.SE);
                    current.Add(coordinate + AxialCoordinate.SW);
                }

                previous = current.Where(coordinate => !neighbors.Contains(coordinate)).ToList();
                current = new ();

                neighbors.AddRange(previous);
            }

            neighbors.Remove(axialCoordinate);

            return neighbors.Where(Storage.Contains).ToList();
        }
    }
}