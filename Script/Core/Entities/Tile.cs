using Godot;
using TribesOfDust.Core.Controllers;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex;

namespace TribesOfDust.Core;

public class Tile(AxialCoordinate location, TileConfiguration configuration)
    : IEntity<TileConfiguration>
{
    #region Connectivity

    public static bool AreConnected(Tile first, Tile second) 
    {
        var firstToSecond = HexDirections.FromOffset(second.Location - first.Location);
        var secondToFirst = HexDirections.FromOffset(first.Location - second.Location);

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

    public ulong Identity { get; } = Identities.GetNextIdentity();
    public TileType Key { get; } = configuration.Key;
    public TileConfiguration Configuration { get; } = configuration;
    public IController? Owner { get; } = null;

    public float Size => Width / 2.0f;
    public float Width => Configuration.Texture?.GetWidth() ?? 0f;
    public float Height => Configuration.Texture?.GetHeight() ?? 0f;
    
    public bool IsBlocked => Key == TileType.Blocked;
    public bool IsOpen => Key == TileType.Open;

    public AxialCoordinate Location { get; } = location;
    
    private HexDirection _connections = (HexDirection)configuration.Connections;
    private HexDirection _direction = configuration.Direction;
}