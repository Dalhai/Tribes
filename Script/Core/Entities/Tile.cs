using System.Collections.Generic;
using System.Linq;
using Godot;
using TribesOfDust.Core.Controllers;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex;

namespace TribesOfDust.Core;

public class Tile : IEntity<TileConfiguration>
{
    #region Constructors

    public Tile(AxialCoordinate coordinates, TileConfiguration configuration)
    {
        Configuration = configuration;
        Coordinates = coordinates;

        // Initialize tile with properties from tile asset

        Key = configuration.Key;
        Identity = Identities.GetNextIdentity();

        _connections = (HexDirection)configuration.Connections;
        _direction = configuration.Direction;

        // Scale tile according to specified texture

        Sprite = new();
        Sprite.Texture = configuration.Texture2D;
        Sprite.Scale = new Vector2(configuration.WidthScaleToExpected, configuration.HeightScaleToExpected);

        // Position tile according to specified coordinates

        Sprite.Centered = true;
        Sprite.Position = HexConversions.HexToUnit(coordinates) * HexConstants.DefaultSize;
    }

    #endregion
    #region Queries

    public ulong Identity { get; }
    public TileType Key { get; }
    public TileConfiguration Configuration { get; }

    public float Size => Width / 2.0f;
    public float Width => Configuration.Texture.GetWidth();
    public float Height => Configuration.Texture.GetHeight();
    
    public bool IsBlocked => Key == TileType.Blocked;
    public bool IsOpen => Key == TileType.Open;

    public AxialCoordinate Coordinates { get; }

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

    private HexDirection _connections;
    private HexDirection _direction;
}