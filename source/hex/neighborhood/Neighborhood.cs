using System.Collections.Generic;
using System.Linq;

using TribesOfDust.Hex.Storage;

namespace TribesOfDust.Hex.Neighborhood
{
    public abstract class Neighborhood : INeighborhood
    {
        /// <summary>
        /// Get all coordinates of the specified tile in this neighborhood.
        /// </summary>
        /// 
        /// <param name="axialCoordinate">The target tile.</param>
        /// <returns>A list of all neighbors in this neighborhood around the tile.</returns>
        public virtual List<AxialCoordinate> GetNeighbors(AxialCoordinate axialCoordinate)
        {
            List<AxialCoordinate> neighbors = new();
            List<AxialCoordinate> current = new();
            List<AxialCoordinate> open = new();

            current.Add(axialCoordinate);

            while (current.Count > 0)
            {
                foreach (var coordinate in current)
                {
                    neighbors.Add(coordinate);
                    open.AddRange(GetNext(coordinate));
                }

                // Move new open coordinates to working set
                current = open
                    .Where(coordinate => !neighbors.Contains(coordinate))
                    .Distinct()
                    .ToList();

                open.Clear();
            }

            return neighbors;
        }

        protected abstract IEnumerable<AxialCoordinate> GetNext(AxialCoordinate coordinate);
    }

    public partial class ConnectedNeighborhood : Neighborhood
    {

        public ConnectedNeighborhood(int distance, ITileStorage<Tile> tiles)
        {
            Tiles = tiles;
            Distance = distance;
            Distances = new TileStorage<int>();
        }

        public ITileStorage<Tile> Tiles { get; init; }
        public int Distance { get; init; }
        public ITileStorage<int> Distances { get; init; }

        public override List<AxialCoordinate> GetNeighbors(AxialCoordinate axialCoordinate)
        {
            Distances.Clear();
            return base.GetNeighbors(axialCoordinate);
        }

        protected override IEnumerable<AxialCoordinate> GetNext(AxialCoordinate coordinate)
        {
            List<AxialCoordinate> next = new();
            
            // Get distance of tile or zero distance, if it does not exist.
            var tile = Tiles.Get(coordinate);
            int distance = Distances.Get(coordinate);

            if (!(tile is null) && distance < Distance)
            {
                var tileNW = Tiles.Get(tile.Coordinates.NW);
                if (tileNW is not null && Tile.AreConnected(tile, tileNW))
                {
                    next.Add(coordinate.NW);
                    Distances.Add(coordinate.NW, distance + 1);
                }

                var tileN = Tiles.Get(tile.Coordinates.N);
                if (tileN is not null && Tile.AreConnected(tile, tileN)) 
                {
                    next.Add(coordinate.N);
                    Distances.Add(coordinate.N, distance + 1);
                }
                var tileNE = Tiles.Get(tile.Coordinates.NE);
                if (tileNE is not null && Tile.AreConnected(tile, tileNE))
                {
                    next.Add(coordinate.NE);
                    Distances.Add(coordinate.NE, distance + 1);
                }

                var tileSE = Tiles.Get(tile.Coordinates.SE);
                if (tileSE is not null && Tile.AreConnected(tile, tileSE))
                {
                    next.Add(coordinate.SE);
                    Distances.Add(coordinate.SE, distance + 1);
                }

                var tileS = Tiles.Get(tile.Coordinates.S);
                if (tileS is not null && Tile.AreConnected(tile, tileS))
                {
                    next.Add(coordinate.S);
                    Distances.Add(coordinate.S, distance + 1);
                }

                var tileSW = Tiles.Get(tile.Coordinates.SW);
                if (tileSW is not null && Tile.AreConnected(tile, tileSW))
                {
                    next.Add(coordinate.SW);
                    Distances.Add(coordinate.SW, distance + 1);
                }
            }

            return next;
        }
    }
}