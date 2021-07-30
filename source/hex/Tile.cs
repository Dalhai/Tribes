using System;
using System.Collections.Generic;

using Godot;
using GodotJson = Godot.Collections.Dictionary;

using TribesOfDust.Utils.IO;
using System.Security;
using System.Diagnostics;

namespace TribesOfDust.Hex
{
    public class Tile : Sprite
    {
        public Tile(AxialCoordinate coordinates, TileAsset asset)
        {
            Coordinates = coordinates;

            // Initialize tile with properties from tile asset

            Type = asset.Type;
            Texture = asset.Texture;

            _connections = (TileDirection) asset.Connections;
            _direction = asset.Direction;

            // Scale tile according to specified texture

            Scale = new Vector2(asset.WidthScaleToExpected, asset.HeightScaleToExpected);

            // Position tile according to specified coordinates

            Centered = true;
            Position = HexConversions.HexToWorld(coordinates, TileAsset.ExpectedSize);
        }

        private Tile(AxialCoordinate coordinates, TileType type, Texture texture, Vector2 scale)
        {
            Coordinates = coordinates;

            // Initialize tile with properties that would normally be provided by a tile asset

            Type =  type;
            Texture = texture;

            _connections = TileDirection.None;
            _direction = TileDirection.None;

            // Initialize the tile scale explicitly

            Scale = scale;

            // Popsition tile according to specified coordinates

            Centered = true;
            Position = HexConversions.HexToWorld(coordinates, TileAsset.ExpectedSize);
        }

        #region Queries

        public float Size => Width / 2.0f;
        public float Width => Texture.GetWidth();
        public float Height => Texture.GetHeight();

        public TileType Type { get; }
        public bool IsBlocked => Type == TileType.Blocked;
        public bool IsOpen => Type == TileType.Open;

        public AxialCoordinate Coordinates { get; }

        #endregion
        #region Connectivity

        /// <summary>
        /// Checks if the tile is connected in the specified direction.
        /// </summary>
        ///
        /// <param name="direction">The direction to look at.</param>
        /// <returns>True, if there is a connection, false otherwise.</returns>
        public bool IsConnected(TileDirection direction) => _connections.HasFlag(direction);

        /// <summary>
        /// Connect the tile in the specified <see cref="TileDirection">.
        /// </summary>
        ///
        /// <param name="direction">The direction to connect.</param>
        public void Connect(TileDirection direction)
        {
            _connections |= direction;
        }

        /// <summary>
        /// Disconnect the tile in the specified <see cref="TileDirection"/>.
        /// </summary>
        ///
        /// <param name="direction">The direction to disconnected.</param>
        public void Disconnect(TileDirection direction)
        {
            _connections &= ~direction;
        }

        #endregion
        #region Serialization

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
        /// Try to deserialize a <see cref="Tile"/> from a godot JSON dictionary.
        /// </summary>
        /// <param name="json">The input JSON dictionary.</param>
        /// <param name="tile">The output tile, or null, if unparseable.</param>
        /// <returns>True, if the tile could be deserialized, false otherwise.</returns>
        public static bool TryDeserialize(GodotJson json, out Tile? tile)
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
                return false;
            }

            // Check if the scale and coordinates can be deserialized

            if (json[keyCoordinates] is not GodotJson coordinatesJson || !Json.TryDeserialize(coordinatesJson, out AxialCoordinate coordinates))
            {
                return false;
            }

            if (json[keyScale] is not GodotJson scaleJson || !Json.TryDeserialize(scaleJson, out Vector2 scale))
            {
                return false;
            }

            // Check if the tile type can be deserialized

            if (json[keyType] is not string typeJson || !Enum.TryParse(typeJson, out TileType type))
            {
                return false;
            }

            tile = new (coordinates, type, texture, scale);
            return true;
        }

        #endregion

        private TileDirection _connections = TileDirection.None;
        private TileDirection _direction = TileDirection.None;
    }
}