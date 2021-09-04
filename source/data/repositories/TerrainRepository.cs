using System;
using System.Collections.Generic;

using TribesOfDust.Hex;
using TribesOfDust.Data.Assets;
using TribesOfDust.Utils.Godot;

using Godot;
using System.IO;

namespace TribesOfDust.Data.Repositories
{
    public class TerrainRepository : Repository<TileType, Terrain>
    {
        /// <summary>
        /// The default resource path used for tile assets.
        /// </summary>
        public static readonly string DefaultPath = "res://assets/tiles";
        public override List<Terrain> LoadAll() => LoadAll(DefaultPath);

        /// <summary>
        /// Loads the terrain type at the specified resource path.
        /// </summary>
        ///
        /// <exception cref="FileNotFoundException">
        /// Thrown when the file at the resource path could not be found.
        /// </exception>
        ///
        /// <param name="resourcePath">The path to the tile asset.</param>
        /// <returns></returns>
        protected override Terrain Load(string resourcePath)
        {
            var asset = GD.Load<Terrain>(resourcePath);
            if (asset != null && asset.Texture != null)
            {
                float width = asset.Texture.GetWidth();
                float height = asset.Texture.GetHeight();
                float ratio = width / height;

                if (!Mathf.IsEqualApprox(ratio, Terrain.ExpectedRatio))
                {
                    GD.PushWarning
                    (
                        $"Tile: {asset.ResourcePath} has invalid ratio:\n" +
                        $"Expected Ratio: {Terrain.ExpectedRatio}\n" +
                        $"Actual Ratio: {ratio}"
                    );
                }

                return asset;
            }

            throw new FileNotFoundException("Could not load terrain asset.", resourcePath);
        }
    }
}