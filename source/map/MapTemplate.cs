using System.Collections.Generic;
using System.Linq;

using Godot;
using GodotJson = Godot.Collections.Dictionary<string, object>;
using GodotArray = Godot.Collections.Array<object>;

using TribesOfDust.Hex;
using TribesOfDust.Utils;

namespace TribesOfDust.Map
{
    public class MapTemplate
    {
        public MapTemplate(Dictionary<AxialCoordinate<int>, TileType> tiles)
        {
            _tiles = tiles;
            _tilePool.Add(TileType.Canyon,2);
            _startCoordinates.Add(new AxialCoordinate<int>(1,0));
            _fountainCoordinates.Add(new AxialCoordinate<int>(0,0));
        }

        /// <summary>
        /// Gets all preplaced tiles on the map.
        /// </summary>
        public IDictionary<AxialCoordinate<int>, TileType> Tiles => _tiles;

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

        public GodotJson Serialize()
        {
            GodotJson result = new();

            // Serialize the initial tile placements

            GodotArray serializedTiles = new();
            foreach (var tile in _tiles)
            {
                var serializedCoordinates = tile.Key.Serialize();
                var serializedType = tile.Value.ToString();

                GodotJson serializedTile = new()
                {
                    {"coordinates", serializedCoordinates},
                    {"type", serializedType}
                };

                serializedTiles.Add(serializedTile);
            }

            // Serialize the tile pool

            GodotJson serializedPool = new();
            foreach (var tilePool in _tilePool)
            {
                var serializedType = tilePool.Key.ToString();
                var serializedCount = tilePool.Value;

                serializedPool.Add(serializedType, serializedCount);
            }

            // Serialize the player start coordinates

            GodotArray serializedStarts = new();
            foreach (var startCoordinates in _startCoordinates)
            {
                serializedStarts.Add(startCoordinates.Serialize());
            }

            // Serialize the fountain coordinates

            GodotArray serializedFountains = new();
            foreach (var fountainCoordinates in _fountainCoordinates)
            {
                serializedFountains.Add(fountainCoordinates.Serialize());
            }

            // Add all serialized values to the final output

            result.Add(nameof(Tiles).ToLower(), serializedTiles);
            result.Add(nameof(TilePool).ToLower(), serializedPool);
            result.Add(nameof(StartCoordinates).ToLower(), serializedStarts);
            result.Add(nameof(FountainCoordinates).ToLower(), serializedFountains);

            return result;
        }

        private readonly Dictionary<AxialCoordinate<int>, TileType> _tiles;
        private readonly Dictionary<TileType, int> _tilePool = new();
        private readonly List<AxialCoordinate<int>> _startCoordinates = new();
        private readonly List<AxialCoordinate<int>> _fountainCoordinates = new();
    }
}