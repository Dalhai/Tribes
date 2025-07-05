using Godot;
using Godot.Collections;
using TribesOfDust.Hex;
using TribesOfDust.Utils;

namespace TribesOfDust.Core.Entities;

public partial class TileConfiguration : Resource, IVariant<TileType>
{
    public override string ToString() => $"Terrain: {Key}";

    #region Exports

    /// <summary>
    /// The overarching type the tile belongs to.
    /// </summary>
    [Export(PropertyHint.Enum)]
    public TileType Key { get; set; }

    /// <summary>
    /// The tile set this tile uses.
    /// </summary>
    [Export(PropertyHint.ResourceType, "TileSet")]
    public TileSet TileSet { get; set; } = null!;

    /// <summary>
    /// The coordinate of the tile variation in the tile atlas.
    /// </summary>
    [Export]
    public Vector2I AtlasCoordinate { get; set; }

    /// <summary>
    /// The direction of the tile.
    /// </summary>
    ///
    /// <remarks>
    /// Most tiles will not be directed, which is allowed as well.
    /// </remarks>
    [Export(PropertyHint.Enum)]
    public HexDirection Direction { get; set; } = HexDirection.Undirected;

    /// <summary>
    /// The connections to other tiles this tile has.
    /// </summary>
    ///
    /// <remarks>
    /// Most tiles will have connections to all surrounding tiles.
    /// </remarks>
    [Export(PropertyHint.Flags, "NW,N,NE,SE,S,SW")]
    public int Connections { get; set; }= (int)HexDirections.All;

    #endregion
}