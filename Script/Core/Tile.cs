using System.Collections.Generic;
using System.Linq;
using Godot;
using TribesOfDust.Core.Controllers;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex;

namespace TribesOfDust.Core;

public class Tile : IEntity
{
    #region Constructors

    public Tile(AxialCoordinate coordinates, TileClass @class)
    {
        Coordinates = coordinates;
        Owner = null;

        // Initialize tile with properties from tile asset

        Key = @class.Key;
        Identity = Identities.GetNextIdentity();

        _connections = (HexDirection)@class.Connections;
        _direction = @class.Direction;

        // Scale tile according to specified texture

        Sprite = new();
        Sprite.Texture = @class.Texture2D;
        Sprite.Scale = new Vector2(@class.WidthScaleToExpected, @class.HeightScaleToExpected);

        // Position tile according to specified coordinates

        Sprite.Centered = true;
        Sprite.Position = HexConversions.HexToUnit(coordinates) * HexConstants.DefaultSize;
    }

    #endregion
    #region Queries

    public ulong Identity { get; }
    public TileType Key { get; }
    public IController? Owner { get; }

    public float Size => Width / 2.0f;
    public float Width => Sprite.Texture.GetWidth();
    public float Height => Sprite.Texture.GetHeight();
    
    public bool IsBlocked => Key == TileType.Blocked;
    public bool IsOpen => Key == TileType.Open;

    public AxialCoordinate Coordinates { get; }
    public Sprite2D Sprite { get; }

    #endregion
    #region Connectivity

    public static bool AreConnected(Tile first, Tile second) 
    {
        var firstToSecond = HexDirections.FromOffset(second.Coordinates - first.Coordinates);
        var secondToFirst = HexDirections.FromOffset(first.Coordinates - second.Coordinates);

        return first.IsConnected(firstToSecond) && second.IsConnected(secondToFirst);
    }

    /// <summary>
    /// Checks if the tile is connected in the specified direction.
    /// </summary>
    ///
    /// <param name="direction">The direction to look at.</param>
    /// <returns>True, if there is a connection, false otherwise.</returns>
    public bool IsConnected(HexDirection direction) => _connections.HasFlag(direction);

    /// <summary>
    /// Connect the tile in the specified <see cref="HexDirection"/>.
    /// </summary>
    ///
    /// <param name="direction">The direction to connect.</param>
    public void Connect(HexDirection direction)
    {
        _connections |= direction;
    }

    /// <summary>
    /// Disconnect the tile in the specified <see cref="HexDirection"/>.
    /// </summary>
    ///
    /// <param name="direction">The direction to disconnected.</param>
    public void Disconnect(HexDirection direction)
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
            Sprite.Modulate = Colors.White;
        }
        else
        {
            // Figure out the color mix and assign it to the modulation color
            Sprite.Modulate = _overlayColors.Aggregate((next, current) => next + current);
            Sprite.Modulate /= _overlayColors.Count;
        }
    }

    #endregion

    private HexDirection _connections;
    private HexDirection _direction;

    private readonly List<Color> _overlayColors = new();
}