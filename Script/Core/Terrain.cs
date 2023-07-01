using Godot;

using TribesOfDust.Utils;

namespace TribesOfDust.Hex;

public partial class Terrain : Resource, IVariant<TileType>
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
    public Texture2D? Texture2D;

    /// <summary>
    /// The direction of the tile.
    /// </summary>
    ///
    /// <remarks>
    /// Most tiles will not be directed, which is allowed as well.
    /// </remarks>
    [Export(PropertyHint.Enum)]
    public TileDirection Direction = TileDirection.None;

    /// <summary>
    /// The connections to other tiles this tile has.
    /// </summary>
    ///
    /// <remarks>
    /// Most tiles will have connections to all surrounding tiles.
    /// </remarks>
    [Export(PropertyHint.Flags)]
    public int Connections = (int)TileDirections.All;

    #endregion
    #region Size

    /// <summary>
    /// Gets the scale in x-direction necessary to match the expected width.
    /// </summary>
    public float WidthScaleToExpected => Texture2D != null ? HexConstants.DefaultWidth / Texture2D.GetWidth() : 1.0f;

    /// <summary>
    /// Gets the scale in y-direction necessary to match the expected height.
    /// </summary>
    public float HeightScaleToExpected => Texture2D != null ? HexConstants.DefaultHeight / Texture2D.GetHeight() : 1.0f;

    #endregion
}