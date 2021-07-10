using System.Collections.Generic;
using System.Linq;
using TribesOfDust.Hex;

namespace TribesOfDust.Map
{
    public class MapTemplate
    {
        public MapTemplate(Dictionary<AxialCoordinate<int>, TileType> tiles)
        {
            _tiles = tiles;
        }

        /// <summary>
        /// Gets all available player start coordinates on the map.
        /// </summary>
        public IEnumerable<AxialCoordinate<int>> StartCoordinates => _startCoordinates;

        /// <summary>
        /// Gets all available fountain coordinates on the map.
        /// </summary>
        public IEnumerable<AxialCoordinate<int>> FountainCoordinates => _fountainCoordinates;

        /// <summary>
        /// Gets the available number of tiles per tile type.
        /// </summary>
        public IDictionary<TileType, int> TilePool => _tilePool;

        public Dictionary<AxialCoordinate<int>, HexTile> Generate(Dictionary<TileType, TileAsset> assets) =>
            _tiles.ToDictionary(
                tile => tile.Key,
                tile => new HexTile(tile.Key, assets[tile.Value])
            );

        private readonly Dictionary<AxialCoordinate<int>, TileType> _tiles;
        private readonly Dictionary<TileType, int> _tilePool = new();
        private readonly List<AxialCoordinate<int>> _startCoordinates = new();
        private readonly List<AxialCoordinate<int>> _fountainCoordinates = new();
    }
}