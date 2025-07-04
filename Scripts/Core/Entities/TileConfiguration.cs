using Godot;
using TribesOfDust.Hex;
using TribesOfDust.Utils;
using TribesOfDust.Utils.Extensions;

namespace TribesOfDust.Core.Entities;

public partial class TileConfiguration : Resource, IConfiguration, IVariant<TileType>
{
    public override string ToString() => $"Terrain: {Key}";

    #region Exports

    /// <summary>
    /// The overarching type the tile belongs to.
    /// </summary>
    [Export(PropertyHint.Enum)]
    public TileType Key { get; set; }

    /// <summary>
    /// The texture associated with the tile.
    /// </summary>
    [Export(PropertyHint.ResourceType, "Texture2D")]
    public Texture2D? Texture { get; set; }

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
    #region Size

    /// <summary>
    /// Gets the scale in x-direction necessary to match the expected width.
    /// </summary>
    /// <param name="tileSet">The TileSet to use for size calculations</param>
    /// <returns>The scale factor for width</returns>
    public float GetWidthScaleToExpected(TileSet tileSet) => Texture != null ? tileSet.GetTileWidth() / Texture.GetWidth() : 1.0f;

    /// <summary>
    /// Gets the scale in y-direction necessary to match the expected height.
    /// </summary>
    /// <param name="tileSet">The TileSet to use for size calculations</param>
    /// <returns>The scale factor for height</returns>
    public float GetHeightScaleToExpected(TileSet tileSet) => Texture != null ? tileSet.GetTileHeight() / Texture.GetHeight() : 1.0f;

    #endregion
}