using System;
using System.Collections.Generic;

using Godot;
using GodotJson = Godot.Collections.Dictionary;

using TribesOfDust.Utils;

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

        public HexTile(AxialCoordinate<int> coordinates, TileType type, Texture texture, Vector2 scale)
        {
            Coordinates = coordinates;

            // Initialize tile with properties that would normally be provided by a tile asset

            Type =  type;
            Texture = texture;

            // Initialize the tile scale explicitly

            Scale = scale;

            // Popsition tile according to specified coordinates

            Centered = true;
            Position = HexConversions.HexToWorld(coordinates, TileAsset.ExpectedSize);
        }

        public float Size => Width / 2.0f;
        public float Width => Texture.GetWidth();
        public float Height => Texture.GetHeight();

        public TileType Type { get; }
        public AxialCoordinate<int> Coordinates { get; }
        public IEnumerable<TileEffect> Effects => _effects;

        /// <summary>
        /// Serialize the tile into a godot JSON dictionary.
        /// </summary>
        /// <returns>The tile as a dictionary.</returns>
        public GodotJson Serialize()
        {
            var serializedCoordinates = Json.Serialize(Coordinates);
            var serializedScale = Json.Serialize(Scale);
            var serializedTexture = Texture.ResourcePath;
            var serializedType = Type.ToString();

            return new()
            {
                {nameof(Coordinates).ToLower(), serializedCoordinates},
                {nameof(Scale).ToLower(), serializedScale},
                {nameof(Texture).ToLower(), serializedTexture},
                {nameof(Type).ToLower(), serializedType}
            };
        }

        /// <summary>
        /// Try to deserialize a <see cref="HexTile"/> from a godot JSON dictionary.
        /// </summary>
        /// <param name="json">The input JSON dictionary.</param>
        /// <param name="tile">The output tile, or null, if unparseable.</param>
        /// <returns>True, if the tile could be deserialized, false otherwise.</returns>
        public static bool TryDeserialize(GodotJson json, out HexTile? tile)
        {
            tile = null;

            string keyCoordinates = nameof(Coordinates).ToLower();
            string keyScale = nameof(Scale).ToLower();
            string keyTexture = nameof(Texture).ToLower();
            string keyType = nameof(Type).ToLower();

            // Check if all necessary keys exist

            if (!json.Contains(keyCoordinates) || !json.Contains(keyScale) || !json.Contains(keyTexture) || !json.Contains(keyType))
            {
                return false;
            }

            // Check if the texture can be loaded

            Texture texture = GD.Load<Texture>((string) json[keyTexture]);

            if (texture == null)
            {
                tile = null;
                return false;
            }

            // Check if the scale and coordinates can be deserialized

            if (json[keyCoordinates] is not GodotJson coordinatesJson || !Json.TryDeserialize(coordinatesJson, out AxialCoordinate<int> coordinates))
            {
                tile = null;
                return false;
            }

            if (json[keyScale] is not GodotJson scaleJson || !Json.TryDeserialize(scaleJson, out Vector2 scale))
            {
                tile = null;
                return false;
            }

            // Check if the tile type can be deserialized

            if (json[keyType] is not string typeJson || !Enum.TryParse(typeJson, out TileType type))
            {
                tile = null;
                return false;
            }

            tile = new (coordinates, type, texture, scale);
            return true;
        }

        private readonly List<TileEffect> _effects = new();
    }
}