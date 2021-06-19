using System.Collections.Generic;
using System.Linq;
using TribesOfDust.Hex;

namespace TribesOfDust.Map
{
    public class MapTemplate
    {
        private Dictionary<AxialCoordinate<int>, TileType> _tiles;
        private Dictionary<TileType, int> _tilePool = new();
        private List<AxialCoordinate<int>> _startCoordinates = new();
        private List<AxialCoordinate<int>> _fountainCoordinates = new();

        public MapTemplate(Dictionary<AxialCoordinate<int>, TileType> tiles)
        {
            _tiles = tiles;
        }

        public Dictionary<AxialCoordinate<int>, HexTile> Generate() =>
            _tiles.ToDictionary(
                tile => tile.Key,
                tile => new HexTile(tile.Key, tile.Value)
            );
    }
}