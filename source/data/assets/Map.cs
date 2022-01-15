using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using TribesOfDust.Data.Repositories;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Storage;

namespace TribesOfDust.Data.Assets
{
    [DataContract]
    public class Map : IAsset<string>
    {
        public Map(string name, string path)
        {
            Name = name;
            ResourcePath = path;

            foreach (var type in Enum.GetValues(typeof(TileType)))
            {
                _tilePool.Add((TileType)type, 0);
            }
        }

        public override string ToString() => $"Map: {Name}, {ResourcePath}";

        #region Asset

        public string Key => Name;

        [DataMember]
        public string Name { get; init; }
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
        /// Fills a <see cref="TileStorage"/> with tiles from the map template.
        /// </summary>
        ///
        /// <param name="repository">The tile asset repository providing assets for the template.</param>
        /// <param name="storage">The tile storage to fill with new tiles.</param>
        public void Generate(TerrainRepository repository, ITileStorage<Tile> storage)
        {
            var tiles = _tiles.Select(tile => Tile.Create(tile.Key, repository.GetAsset(tile.Value, 0)));

            foreach (var tile in tiles)
            {
                storage.Add(tile.Coordinates, tile);
            }
        }

        /// <summary>
        /// Generates a new <see cref="TileStorage"/> from the map template.
        /// </summary>
        ///
        /// <param name="repository">The tile asset repository providing assets for the template.</param>
        /// <returns>A new runtime map based on the map template.</returns>
        public TileStorage<Tile> Generate(TerrainRepository repository)
        {
            var storage = new TileStorage<Tile>();

            Generate(repository, storage);

            return storage;
        }

        #endregion

        [DataMember] private readonly Dictionary<AxialCoordinate, TileType> _tiles = new();
        [DataMember] private readonly Dictionary<TileType, int> _tilePool = new();
        [DataMember] private readonly List<AxialCoordinate> _startCoordinates = new();
        [DataMember] private readonly List<AxialCoordinate> _fountainCoordinates = new();
    }
}