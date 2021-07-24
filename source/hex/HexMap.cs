using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Godot;


namespace TribesOfDust.Hex
{
    public class HexMap : Node2D, IEnumerable<HexTile>
    {
        /// <summary>
        /// Initializes an empty <see cref="HexMap"/>.
        /// </summary>
        public HexMap()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="HexMap"/>.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when there are multiple tiles with the same coordinates in the initial tile list.
        /// </exception>
        ///
        /// <param name="tiles">The initial tiles of the map.</param>
        public HexMap(IEnumerable<HexTile> tiles)
        {
            // Build an index of tile coordinates to tiles.
            // Automatically performs validity checks for successively added tiles.

            foreach (var tile in tiles)
            {
                AddTile(tile);
            }
        }

        #region Tile Queries

        /// <summary>
        /// Gets a <see cref="HexTile"/> at a specific <see cref="AxialCoordinate"/>.
        /// </summary>
        ///
        /// <returns>The tile at the specified coordinates.</returns>
        public HexTile? this[AxialCoordinate coordinate] => GetTileAt(coordinate);

        public bool HasTileAt(AxialCoordinate coordinates) => tiles.ContainsKey(coordinates);
        public bool IsOpenAt(AxialCoordinate coordinates) => GetTileTypeAt(coordinates) == TileType.Open;
        public bool IsBlockedAt(AxialCoordinate coordinates) => GetTileTypeAt(coordinates) == TileType.Blocked;

        public HexTile? GetTileAt(AxialCoordinate coordinates) => tiles.ContainsKey(coordinates) ? tiles[coordinates] : null;
        public TileType GetTileTypeAt(AxialCoordinate coordinates) => GetTileAt(coordinates)?.Type ?? TileType.Unknown;

        #endregion
        #region Manipulation

        /// <summary>
        /// Adds a <see cref="HexTile"/> to the map.
        /// </summary>
        ///
        /// <remarks>
        /// If a tile has already been added at the same location, replaces the existing tile
        /// with this new one silently.
        /// </remarks>
        ///
        /// <param name="tile">The tile to add.</param>
        public void AddOrOverwriteTile(HexTile tile)
        {
            if (tiles.ContainsKey(tile.Coordinates))
            {
                RemoveTileAt(tile.Coordinates);
            }

            AddTile(tile);
        }

        /// <summary>
        /// Adds a <see cref="HexTile"/> to the map.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// A tile at the specified coordinates has already been added.
        /// </exception>
        ///
        /// <param name="tile">The tile to add.</param>
        public void AddTile(HexTile tile)
        {
            if (tiles.ContainsKey(tile.Coordinates))
            {
                throw new ArgumentException("Duplicate tile coordinates in tile map.", "tile");
            }

            tiles.Add(tile.Coordinates, tile);
            AddChild(tile);
        }

        /// <summary>
        /// Removes a <see cref="HexTile"/> from the map.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when the tile that is being removed is not part of the map.
        /// </exception>
        ///
        /// <param name="tile">The tile to remove.</param>
        public void RemoveTile(HexTile tile)
        {
            HexTile? target = GetTileAt(tile.Coordinates);

            // Note that even though we might receive a valid tile, the tile stored at
            // the coordinates of the provided tile might not be the same, in which
            // case we need to throw an exception.

            if (target is null || target != tile)
            {
                throw new ArgumentException("Trying to remove tile that is not part of the hex map.", "tile");
            }

            tiles.Remove(target.Coordinates);
            RemoveChild(target);
        }

        /// <summary>
        /// Removes a <see cref="HexTile"/> from the map.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when there is no tile at the specified coordinates.
        /// </exception>
        ///
        /// <param name="coordinates">The coordinates of the tile to remove.</param>
        public void RemoveTileAt(AxialCoordinate coordinates)
        {
            HexTile? removed = GetTileAt(coordinates);

            // Note that the dictionary won't throw an exception if the coordinates are
            // not registered. Since this is exactly what we want, no additional checks
            // are necessary.

            if (removed is null)
            {
                throw new ArgumentException("Trying to remove tile that is not part of the hex map.", "coordinates");
            }

            tiles.Remove(coordinates);
            RemoveChild(removed);
        }

        #endregion

        #region IEnumerable Implementation

        public IEnumerator<HexTile> GetEnumerator() => tiles.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => tiles.Values.GetEnumerator();

        #endregion

        private readonly Dictionary<AxialCoordinate, HexTile> tiles = new();
    }
}