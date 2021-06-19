using System.Collections.Generic;
using TribesOfDust.Hex;

namespace TribesOfDust.Map
{
    public class MapTemplate
    {
        private Dictionary<AxialCoordinate<int>, TileType> _tiles = new();
        private Dictionary<TileType, int> _tilePool = new();
        private List<AxialCoordinate<int>> _startCoordinates = new();
        private List<AxialCoordinate<int>> _fountainCoordinates = new();
    }
}