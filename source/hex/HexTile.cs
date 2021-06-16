using System;
using Godot;

namespace TribesOfDust.Hex
{
    public class HexTile : Sprite
    {
        public AxialCoordinate<int> Coordinates { get; init; }
        public TileType Type { get; init; }
    }
}