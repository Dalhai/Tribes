using System;
using System.Collections.Generic;
using System.Linq;

using TribesOfDust.Data.Repositories;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Storage;
using TribesOfDust.Utils.IO;

using GodotArray = Godot.Collections.Array;
using GodotJson = Godot.Collections.Dictionary;

namespace TribesOfDust.Data.Assets
{
    public class Map : IAsset<string>
    {
        /// <summary>
        /// Initializes a new <see cref="Map"/>.
        /// </summary>
        ///
        /// <param name="name">The name of the map.</param>
        /// <param name="path">The path to the map file.</param>
        /// <param name="tiles">The tiles forming the initial map.</param>
        /// <param name="tilePool">The number of tiles available to players of each type.</param>
        /// <param name="startCoordinates">The possible start coordinates for players.</param>
        /// <param name="fountainCoordinates">The possible fountain coordinates.</param>
        private Map(
            string name,
            string path,
            Dictionary<AxialCoordinate, TileType> tiles,
            Dictionary<TileType, int> tilePool,
            List<AxialCoordinate> startCoordinates,
            List<AxialCoordinate> fountainCoordinates)
        {
            Key = name;
            ResourcePath = path;

            _tiles = tiles;
            _tilePool = tilePool;
            _startCoordinates = startCoordinates;
            _fountainCoordinates = fountainCoordinates;
        }

        #region Asset

        public string Key { get; init; }
        public string ResourcePath { get; init; }

        #endregion
        #region Access

        /// <summary>
        /// Gets all preplaced tiles on the map.
        /// </summary>
        public IDictionary<AxialCoordinate, TileType> Tiles => _tiles;

        /// <summary>
        /// Gets the available number of tiles per tile type.
        /// </summary>
        public IDictionary<TileType, int> TilePool => _tilePool;

        /// <summary>
        /// Gets all available player start coordinates on the map.
        /// </summary>
        public IEnumerable<AxialCoordinate> StartCoordinates => _startCoordinates;

        /// <summary>
        /// Gets all available fountain coordinates on the map.
        /// </summary>
        public IEnumerable<AxialCoordinate> FountainCoordinates => _fountainCoordinates;

        #endregion
        #region Generation

        /// <summary>
        /// Generates a new <see cref="TileStorage"/> from the map template.
        /// </summary>
        ///
        /// <param name="repository">The tile asset repository providing assets for the template.</param>
        /// <returns>A new runtime map based on the map template.</returns>
        public TileStorage<Tile> Generate(TerrainRepository repository)
        {
            var tiles = _tiles.Select(tile => new Tile(tile.Key, repository.GetAsset(tile.Value)));
            var storage = new TileStorage<Tile>();

            foreach (var tile in tiles)
            {
                storage.Add(tile.Coordinates, tile);
            }

            return storage;
        }

        #endregion
        #region Serialization

        /// <summary>
        /// Serializes the map template into a JSON dictionary.
        /// </summary>
        ///
        /// <remarks>
        /// The JSON dictioanry is simply a base-type Godot Dictionary mapping objects to objects.
        /// The map template is encoded into this dictionary format, which can then be further processed.
        /// </remarks>
        ///
        /// <returns>A Godot dictionary representation of the map template.</returns>
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
                    { "coordinates", serializedCoordinates },
                    { "type", serializedType }
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

        /// <summary>
        /// Tries to deserialize a Godot JSON dictionary into a map template.
        /// </summary>
        /// <param name="json">The godot JSON dictionary representing the map.</param>
        /// <param name="map">The output map template that is filled with information on success.</param>
        /// <returns>True, if deserializing succeeded, false otherwise.</returns>
        public static bool TryDeserialize(GodotJson json, out Map? map)
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
            Dictionary<AxialCoordinate, TileType> tiles = new();

            // First need to check whether the are even tiles available
            if (json[keyTiles] is not GodotArray jsonTiles)
            {
                return false;
            }

            // Iterate over all json tiles and deserialize them individually

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

                if (tileObject[keyCoordinates] is not GodotJson jsonCoordinates || !Json.TryDeserialize(jsonCoordinates, out AxialCoordinate coordinates))
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

            // Try to deserialize the tile pool
            Dictionary<TileType, int> tilePool = new();

            // First need to check whether the tile pool is even available
            if (json[keyPool] is not GodotJson jsonPool)
            {
                return false;
            }

            // Iterate over all json tiles and deserialize them individually

            foreach (var poolKey in jsonPool.Keys)
            {
                // Try to get the tile type

                if (poolKey is not string jsonType || !Enum.TryParse(jsonType, out TileType type))
                {
                    return false;
                }

                // Try to get the tile count

                try
                {
                    tilePool.Add(type, Convert.ToInt32(jsonPool[poolKey]));
                }
                catch (Exception)
                {
                    return false;
                }
            }

            static bool exctractAxialCoordinates(object json, List<AxialCoordinate> result)
            {
                // First need to check whether the coordinates are even available
                if (json is not GodotArray jsonCoordinates)
                {
                    return false;
                }

                // Iterate over all positions and deserialize them individually

                foreach (var coordinate in jsonCoordinates)
                {
                    if (coordinate is not GodotJson jsonCoordinate || !Json.TryDeserialize(jsonCoordinate, out AxialCoordinate coordinates))
                    {
                        return false;
                    }

                    result.Add(coordinates);
                }

                return true;
            };

            List<AxialCoordinate> startCoordinates = new();
            List<AxialCoordinate> fountainCoordinates = new();

            exctractAxialCoordinates(json[keyStarts], startCoordinates);
            exctractAxialCoordinates(json[keyFountains], fountainCoordinates);

            map = new(String.Empty, String.Empty, tiles, tilePool, startCoordinates, fountainCoordinates);
            return true;
        }

        #endregion

        private readonly Dictionary<AxialCoordinate, TileType> _tiles;
        private readonly Dictionary<TileType, int> _tilePool = new();
        private readonly List<AxialCoordinate> _startCoordinates = new();
        private readonly List<AxialCoordinate> _fountainCoordinates = new();
    }
}