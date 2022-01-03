using Godot;

using System.Linq;
using System.Collections.Generic;

using TribesOfDust.Data.Assets;
using TribesOfDust.Data.Config;

namespace TribesOfDust.Hex
{
    public class Tile : Sprite
    {
        #region Factory

        public static Tile Create(AxialCoordinate coordinates, Terrain terrain)
        {
            return new(TileConfig.Default, coordinates, terrain);
        }

        #endregion
        #region Constructors

        private Tile()
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

        private Tile(TileConfig config, AxialCoordinate coordinates, Terrain terrain)
        {
            // Initialize tile with tile config shared properties

            Material = config.Material;

            // Initialize tile with proper coordinates

            Coordinates = coordinates;

            // Initialize tile with properties from tile asset

            Key = terrain.Key;
            Texture = terrain.Texture;

            _connections = (TileDirection)terrain.Connections;
            _direction = terrain.Direction;

            // Scale tile according to specified texture

            Scale = new Vector2(terrain.WidthScaleToExpected, terrain.HeightScaleToExpected);

            // Position tile according to specified coordinates

            Centered = true;
            Position = HexConversions.HexToWorld(coordinates, Terrain.ExpectedSize);
        }

        #endregion
        #region Queries

        public float Size => Width / 2.0f;
        public float Width => Texture.GetWidth();
        public float Height => Texture.GetHeight();

        public TileType Key { get; private set; }
        public bool IsBlocked => Key == TileType.Blocked;
        public bool IsOpen => Key == TileType.Open;

        public AxialCoordinate Coordinates { get; private set; }

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
        #region Display

        public void AddOverlayColor(Color color) 
        {
            _overlayColors.Add(color);
            UpdateOverlayColor();
        }

        public void RemoveOverlayColor(Color color)
        {
            _overlayColors.Remove(color);
            UpdateOverlayColor();
        }

        public void ClearOverlayColor()
        {
            _overlayColors.Clear();
            UpdateOverlayColor();
        }

        private void UpdateOverlayColor() 
        {
            if (_overlayColors.Count == 0)
            {
                Modulate = Colors.White;
            }
            else
            {
                // Figure out the color mix and assign it to the modulation color
                Modulate = _overlayColors.Aggregate((Next, Current) => Next + Current);
                Modulate /= _overlayColors.Count;
            }
        }

        #endregion

        private TileDirection _connections = TileDirection.None;
        private TileDirection _direction = TileDirection.None;

        private readonly List<Color> _overlayColors = new();
    }
}