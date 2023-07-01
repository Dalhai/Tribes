using Godot;

using System.Linq;
using System.Collections.Generic;

namespace TribesOfDust.Hex
{
    public class Tile 
    {
        #region Constructors

        public Tile()
        {
            // Default Zero Coordinates

            Coordinates = AxialCoordinate.Zero;

            // Setup empty defaults for asset values

            Key = TileType.Unknown;

            // Position tile according to specified coordinates

            _sprite.Scale = Vector2.Zero;
            _sprite.Centered = true;
            _sprite.Position = HexConversions.HexToUnit(Coordinates) * HexConstants.DefaultSize;
        }

        public Tile(AxialCoordinate coordinates, Terrain terrain)
        {
            // Initialize tile with proper coordinates

            Coordinates = coordinates;

            // Initialize tile with properties from tile asset

            Key = terrain.Key;

            _connections = (TileDirection)terrain.Connections;
            _direction = terrain.Direction;

            // Scale tile according to specified texture

            _sprite.Texture = terrain.Texture2D;
            _sprite.Scale = new Vector2(terrain.WidthScaleToExpected, terrain.HeightScaleToExpected);

            // Position tile according to specified coordinates

            _sprite.Centered = true;
            _sprite.Position = HexConversions.HexToUnit(coordinates) * HexConstants.DefaultSize;
        }

        #endregion
        #region Queries

        public float Size => Width / 2.0f;
        public float Width => _sprite.Texture.GetWidth();
        public float Height => _sprite.Texture.GetHeight();

        public TileType Key { get; private set; }
        public bool IsBlocked => Key == TileType.Blocked;
        public bool IsOpen => Key == TileType.Open;

        public AxialCoordinate Coordinates { get; private set; }
        public Sprite2D Sprite => _sprite;

        #endregion
        #region Connectivity

        public static bool AreConnected(Tile first, Tile second) 
        {
            var firstToSecond = TileDirections.FromOffset(second.Coordinates - first.Coordinates);
            var secondToFirst = TileDirections.FromOffset(first.Coordinates - second.Coordinates);

            return first.IsConnected(firstToSecond) && second.IsConnected(secondToFirst);
        }

        /// <summary>
        /// Checks if the tile is connected in the specified direction.
        /// </summary>
        ///
        /// <param name="direction">The direction to look at.</param>
        /// <returns>True, if there is a connection, false otherwise.</returns>
        public bool IsConnected(TileDirection direction) => _connections.HasFlag(direction);

        /// <summary>
        /// Connect the tile in the specified <see cref="TileDirection"/>.
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
                _sprite.Modulate = Colors.White;
            }
            else
            {
                // Figure out the color mix and assign it to the modulation color
                _sprite.Modulate = _overlayColors.Aggregate((next, current) => next + current);
                _sprite.Modulate /= _overlayColors.Count;
            }
        }

        #endregion

        private TileDirection _connections = TileDirection.None;
        private TileDirection _direction = TileDirection.None;

        private readonly List<Color> _overlayColors = new();
        private readonly Sprite2D _sprite = new();
    }
}