using System;
using System.Collections.Generic;
using Godot;

namespace TribesOfDust.Hex
{
    public class HexTile : Sprite
    {
        private static readonly Lazy<Texture> MountainTexture = new(() => GD.Load<Texture>("res://assets/textures/tile_mountain_1.png"));
        private static readonly Lazy<Texture> DefaultTexture = new(() => GD.Load<Texture>("res://assets/textures/tile_mountain_1.png"));

        /// <summary>
        /// The radius of one hex tile.
        /// </summary>
        /// <remarks>
        /// Currently defaults to the dimension of the default texture.
        /// Is due to change in the future.
        /// </remarks>
        public static float Size => DefaultTexture.Value.GetWidth() / 2.0f;

        public HexTile(AxialCoordinate<int> coordinates, TileType type)
        {
            Coordinates = coordinates;
            Type = type;
            Centered = true;
            Position = HexConversions.HexToWorld(coordinates, Size);

            switch (Type)
            {
                case TileType.Rocks:
                    Texture = MountainTexture.Value;
                    break;
                default:
                    Texture = DefaultTexture.Value;
                    Modulate = Colors.Fuchsia;
                    break;
            }
        }

        public TileType Type { get; }
        public AxialCoordinate<int> Coordinates { get; }
        public IEnumerable<TileEffect> Effects => _effects;

        private readonly List<TileEffect> _effects = new();
    }
}