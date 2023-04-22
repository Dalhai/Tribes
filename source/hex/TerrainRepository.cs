using System.Collections.Generic;

using Godot;

using TribesOfDust.Utils;

namespace TribesOfDust.Hex
{
    public partial class TerrainRepository : Repository<TileType, Terrain>
    {
        /// <summary>
        /// The default resource path used for tile assets.
        /// </summary>
        private static readonly string DefaultPath = "res://assets/terrains";
        protected override List<Terrain> LoadAll() => LoadAll(DefaultPath);

        protected override bool TryLoad(string resourcePath, out Terrain? asset)
        {
            asset = GD.Load<Terrain>(resourcePath);

            if (asset != null && asset.Texture2D != null)
            {
                float width = asset.Texture2D.GetWidth();
                float height = asset.Texture2D.GetHeight();
                float ratio = width / height;

                if (!Mathf.IsEqualApprox(ratio, HexConstants.DefaultRatio))
                {
                    GD.PushWarning
                    (
                        $"Tile: {asset.ResourcePath} has invalid ratio:\n" +
                        $"Expected Ratio: {HexConstants.DefaultRatio}\n" +
                        $"Actual Ratio: {ratio}"
                    );
                }

                return true;
            }

            return false;
        }
    }
}