using System.Collections.Generic;
using System.Linq;
using Godot;
using TribesOfDust.Hex;

using JsonDict = Godot.Collections.Dictionary<string, object>;
using JsonArray = Godot.Collections.Array<object>;


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

        public void Save()
        {
            string json = ToJson();
            var target = new File();
            target.Open("user://map.template", File.ModeFlags.Write);
            target.StoreLine(json);
            target.Close();
        }

        private string ToJson()
        {
            JsonDict dictionary = new();

            JsonArray tilesArray = new();
            foreach (var tile in _tiles)
            {
                JsonDict tileDict = new();
                tileDict.Add("q", tile.Key.Q);
                tileDict.Add("r",tile.Key.R);
                tileDict.Add("type",tile.Value.ToString());
                tilesArray.Add(tileDict);
            }

            JsonDict tilePoolDict = new();
            foreach (var tileCount in _tilePool)
            {
                tilePoolDict.Add(tileCount.Key.ToString(),tileCount.Value);
            }

            JsonArray startCoordinateArray = new();

            foreach (var coordinate in _startCoordinates)
            {
                JsonDict tileDict = new();
                tileDict.Add("q", coordinate.Q);
                tileDict.Add("r",coordinate.R);
                startCoordinateArray.Add(tileDict);
            }

            JsonArray fountainCoordinateArray = new();
            foreach (var coordinate in _fountainCoordinates)
            {
                JsonDict tileDict = new();
                tileDict.Add("q", coordinate.Q);
                tileDict.Add("r",coordinate.R);
                fountainCoordinateArray.Add(tileDict);
            }


            dictionary.Add("tiles",tilesArray);
            dictionary.Add("tilePool",tilePoolDict);
            dictionary.Add("startCoordinates",startCoordinateArray);
            dictionary.Add("fountainCoordinates",fountainCoordinateArray);

            return JSON.Print(dictionary);
        }

        private readonly Dictionary<AxialCoordinate<int>, TileType> _tiles;
        private readonly Dictionary<TileType, int> _tilePool = new();
        private readonly List<AxialCoordinate<int>> _startCoordinates = new();
        private readonly List<AxialCoordinate<int>> _fountainCoordinates = new();
    }
}