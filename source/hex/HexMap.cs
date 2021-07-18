using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace TribesOfDust.Hex
{
    public class HexMap : IEnumerable<HexTile>
    {
        public HexMap()
        {
            tiles = new();
        }

        public HexMap(IEnumerable<HexTile> tiles)
        {

            // Verify the tiles are valid

            bool hasDuplicateCoordinates = tiles
                .GroupBy(tile => tile.Coordinates)
                .Select(group => group.Count())
                .Any(count => count > 1);

            if (hasDuplicateCoordinates)
            {
                throw new ArgumentException("Duplicate tile coordinates in tile map.", "tiles");
            }

            // Build an index of tile coordinates to tiles

            this.tiles = tiles.ToDictionary(tile => tile.Coordinates);
        }

        #region Tile Queries

        public bool HasTileAt(AxialCoordinate<int> coordinates) => tiles.ContainsKey(coordinates);
        public bool IsOpenAt(AxialCoordinate<int> coordinates) => GetTileTypeAt(coordinates) == TileType.Open;
        public bool IsBlockedAt(AxialCoordinate<int> coordinates) => GetTileTypeAt(coordinates) == TileType.Blocked;

        public HexTile? GetTileAt(AxialCoordinate<int> coordinates) => tiles.ContainsKey(coordinates) ? tiles[coordinates] : null;
        public TileType GetTileTypeAt(AxialCoordinate<int> coordinates) => GetTileAt(coordinates)?.Type ?? TileType.Unknown;

        #endregion
        #region Manipulation

        public void RemoveTileAt(AxialCoordinate<int> coordinates)
        {
            // Note that the dictionary won't throw an exception if the coordinates are
            // not registered. Since this is exactly what we want, no additional checks
            // are necessary.

            tiles.Remove(coordinates);
        }

        #endregion

        #region IEnumerable Implementation

        public IEnumerator<HexTile> GetEnumerator() => tiles.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => tiles.Values.GetEnumerator();

        #endregion

        private readonly Dictionary<AxialCoordinate<int>, HexTile> tiles;
    }
}