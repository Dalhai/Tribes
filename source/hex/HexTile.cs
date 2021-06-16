using System;
using System.Collections.Generic;
using Godot;

namespace TribesOfDust.Hex
{
    public class HexTile : Sprite
    {
        public AxialCoordinate<int> Coordinates { get; init; }
        public TileType Type { get; init; }
        public List<TileEffect> Effects { get; init; }
    }
}