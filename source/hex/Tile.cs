using System;

using Godot;
using GodotJson = Godot.Collections.Dictionary;

using TribesOfDust.Data.Assets;

namespace TribesOfDust.Hex
{
    public class Tile : Sprite
    {
        public Tile()
        {
            // Default Zero Coordinates

            Coordinates = AxialCoordinate.Zero;

            // Setup empty defaults for asset values

            Key = TileType.Unknown;
            Scale = Vector2.Zero;

            // Position tile according to specified coordinates

            Centered = true;
            Position = HexConversions.HexToWorld(Coordinates, Terrain.ExpectedSize);
        }

        public Tile(AxialCoordinate coordinates, Terrain asset)
        {
            Coordinates = coordinates;

            // Initialize tile with properties from tile asset

            Key = asset.Key;
            Texture = asset.Texture;

            _connections = (TileDirection) asset.Connections;
            _direction = asset.Direction;

            // Scale tile according to specified texture

            Scale = new Vector2(asset.WidthScaleToExpected, asset.HeightScaleToExpected);

            // Position tile according to specified coordinates

            Centered = true;
            Position = HexConversions.HexToWorld(coordinates, Terrain.ExpectedSize);
        }

        private Tile(AxialCoordinate coordinates, TileType type, Texture texture, Vector2 scale)
        {
            Coordinates = coordinates;

            // Initialize tile with properties that would normally be provided by a tile asset

            Key =  type;
            Texture = texture;

            _connections = TileDirection.None;
            _direction = TileDirection.None;

            // Initialize the tile scale explicitly

            Scale = scale;

            // Popsition tile according to specified coordinates

            Centered = true;
            Position = HexConversions.HexToWorld(coordinates, Terrain.ExpectedSize);
        }

        #region Queries

        public float Size => Width / 2.0f;
        public float Width => Texture.GetWidth();
        public float Height => Texture.GetHeight();

        public TileType Key { get; }
        public bool IsBlocked => Key == TileType.Blocked;
        public bool IsOpen => Key == TileType.Open;

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

        private TileDirection _connections = TileDirection.None;
        private TileDirection _direction = TileDirection.None;
    }
}