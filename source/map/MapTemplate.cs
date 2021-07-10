using System.Collections.Generic;
using System.Linq;
using System;

using GodotJson = Godot.Collections.Dictionary;
using GodotArray = Godot.Collections.Array;

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

        public static bool TryDeserialize(GodotJson json, out MapTemplate? map)
        {
            map = null;

            string keyTiles = nameof(Tiles).ToLower();
            string keyPool = nameof(TilePool).ToLower();
            string keyStarts = nameof(StartCoordinates).ToLower();
            string keyFountains = nameof(FountainCoordinates).ToLower();

            if (!json.Contains(keyTiles) || !json.Contains(keyPool) || !json.Contains(keyStarts) || !json.Contains(keyFountains))
            {
                return false;
            }

            // Try to deserialize the pre-placed tiles into this dictionary
            Dictionary<AxialCoordinate<int>, TileType> tiles = new ();

            // First need to check whether the are even tiles available
            if (json[keyTiles] is GodotArray jsonTiles)
            {
                string keyCoordinates = "coordinates";
                string keyType = "type";

                foreach (var jsonTile in jsonTiles)
                {
                    if (jsonTile is not GodotJson tileObject)
                        continue;

                    if (!tileObject.Contains(keyCoordinates) || !tileObject.Contains(keyType))
                    {
                        return false;
                    }

                    // Try to get the coordinates

                    if (tileObject[keyCoordinates] is not GodotJson jsonCoordinates || !Json.TryDeserialize(jsonCoordinates, out AxialCoordinate<int> coordinates))
                    {
                        return false;
                    }

                    // Try to get the tile type

                    if (tileObject[keyType] is not string jsonType || !Enum.TryParse(jsonType, out TileType type))
                    {
                        return false;
                    }

                    // Append the tile information to the output
                    tiles.Add(coordinates, type);
                }
            }

            map = new(tiles);
            return true;
        }

        private readonly Dictionary<AxialCoordinate<int>, TileType> _tiles;
        private readonly Dictionary<TileType, int> _tilePool = new();
        private readonly List<AxialCoordinate<int>> _startCoordinates = new();
        private readonly List<AxialCoordinate<int>> _fountainCoordinates = new();
    }
}