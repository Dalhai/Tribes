using static System.Diagnostics.Debug;

using TribesOfDust.Core.Controllers;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;

namespace TribesOfDust.Core;

public class Tile : IEntity<TileConfiguration>
{

    public Tile(IHexLayer<Tile> tiles, AxialCoordinate location, TileConfiguration configuration)
    {
        Key = configuration.Key;
        
        Configuration = configuration;
        connections = (HexDirection)configuration.Connections;
        
        // Setup initial location

        _tiles = tiles;
        _location = location;

        if (_tiles.Get(Location) is null)
            _tiles.Add(this, Location);
        else Assert(false);
    }
    
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
    public bool IsConnected(HexDirection direction) => connections.HasFlag(direction);

    /// <summary>
    /// Connect the tile in the specified <see cref="HexDirection"/>.
    /// </summary>
    ///
    /// <param name="direction">The direction to connect.</param>
    public void Connect(HexDirection direction)
    {
        connections |= direction;
    }

    /// <summary>
    /// Disconnect the tile in the specified <see cref="HexDirection"/>.
    /// </summary>
    ///
    /// <param name="direction">The direction to disconnected.</param>
    public void Disconnect(HexDirection direction)
    {
        connections &= ~direction;
    }

    #endregion

    public ulong Identity { get; } = Identities.GetNextIdentity();
    public TileType Key { get; }
    public TileConfiguration Configuration { get; }
    public IController? Owner { get; } = null;

    public float Size => Width / 2.0f;
    public float Width => Configuration.Texture?.GetWidth() ?? 0f;
    public float Height => Configuration.Texture?.GetHeight() ?? 0f;
    
    public bool IsBlocked => Key == TileType.Blocked;
    public bool IsOpen => Key == TileType.Open;

    private AxialCoordinate _location;
    public AxialCoordinate Location
    {
        get => _location;
        set
        {
            if (_location == value)
                return;

            if (_tiles.Get(_location) is { } tile && tile.Identity == Identity)
                _tiles.Remove(_location);
            else Assert(false);
            
            _location = value;
            _tiles.Add(this, _location);
        }
    }

    private HexDirection connections;
    private readonly IHexLayer<Tile> _tiles;
}