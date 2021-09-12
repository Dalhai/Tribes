using Godot;

using TribesOfDust.Hex;
using TribesOfDust.Utils.Misc;

namespace TribesOfDust.Data.Assets
{
    public class Terrain : Resource, IAsset<TileType>
    {
        public static readonly float ExpectedSize = 100.0f;
        public static readonly float ExpectedWidth = 2.0f * ExpectedSize;
        public static readonly float ExpectedHeight = 2.0f * Mathf.Sqrt(3.0f / 4.0f * ExpectedSize * ExpectedSize);
        public static readonly float ExpectedRatio = ExpectedWidth / ExpectedHeight;

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
        public float WidthScaleToExpected => Texture != null ? ExpectedWidth / Texture.GetWidth() : 1.0f;

        /// <summary>
        /// Gets the scale in y-direction necessary to match the expected height.
        /// </summary>
        public float HeightScaleToExpected => Texture != null ? ExpectedHeight / Texture.GetHeight() : 1.0f;

        #endregion
    }
}