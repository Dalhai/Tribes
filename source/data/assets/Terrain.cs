using Godot;

using TribesOfDust.Hex;
using TribesOfDust.Utils.Misc;

namespace TribesOfDust.Data.Assets
{
    public class Terrain : Resource, IAsset<TileType>
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
        [Export(PropertyHint.ResourceType, "Texture")]
        public Texture? Texture;

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
        [ExportFlags(typeof(TileDirection))]
        public int Connections = (int)TileDirections.All;

        #endregion
        #region Size

        /// <summary>
        /// Gets the scale in x-direction necessary to match the expected width.
        /// </summary>
        public float WidthScaleToExpected => Texture != null ? HexConstants.DefaultWidth / Texture.GetWidth() : 1.0f;

        /// <summary>
        /// Gets the scale in y-direction necessary to match the expected height.
        /// </summary>
        public float HeightScaleToExpected => Texture != null ? HexConstants.DefaultHeight / Texture.GetHeight() : 1.0f;

        #endregion
    }
}