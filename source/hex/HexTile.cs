using System;
using System.Collections.Generic;
using Godot;

namespace TribesOfDust.Hex
{
    public class HexTile : Sprite
    {
        public HexTile(AxialCoordinate<int> coordinates, TileAsset asset)
        {
            Coordinates = coordinates;

            // Initialize tile with properties from tile asset

            Type = asset.Type;
            Texture = asset.Texture;

            // Scale tile according to specified texture

            Scale = new Vector2(asset.WidthScaleToExpected, asset.HeightScaleToExpected);

            // Position tile according to specified coordinates

            Centered = true;
            Position = HexConversions.HexToWorld(coordinates, TileAsset.ExpectedSize);
        }

        public float Size => Width / 2.0f;
        public float Width => Texture.GetWidth();
        public float Height => Texture.GetHeight();

        public TileType Type { get; }
        public AxialCoordinate<int> Coordinates { get; }
        public IEnumerable<TileEffect> Effects => _effects;

        private readonly List<TileEffect> _effects = new();
    }
}