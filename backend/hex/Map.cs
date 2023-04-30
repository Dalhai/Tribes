using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using TribesOfDust.Hex.Storage;
using TribesOfDust.Utils;

namespace TribesOfDust.Hex
{
    [DataContract]
    public class Map : IVariant<string>
    {
        public Map(string name) { Name = name; }
        public override string ToString() => $"Map: {Name}";

        #region Asset

        [DataMember]
        public string Name { get; init; }
        string IVariant<string>.Key => Name;

        #endregion
        #region Access

        /// <summary>
        /// Gets all preplaced tiles on the map.
        /// </summary>
        public IDictionary<AxialCoordinate, TileType> Tiles => _tiles;
        
        #endregion
        #region Generation

        /// <summary>
        /// Fills a <see cref="ITileStorage{Tile}"/> with tiles from the map template.
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

        #endregion

        [DataMember] private readonly Dictionary<AxialCoordinate, TileType> _tiles = new();
    }
}