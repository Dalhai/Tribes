using System;
using System.Collections.Generic;
using Godot;

namespace TribesOfDust.Hex
{
    public class HexTile : Sprite
    {
        public HexTile(AxialCoordinate<int> coordinates, TileType type, List<TileEffect> effects)
        {
            Coordinates = coordinates;
            Type = type;
            Effects = effects;
        }

        public AxialCoordinate<int> Coordinates { get; }
        public TileType Type { get; }
        public List<TileEffect> Effects { get; }
    }
}