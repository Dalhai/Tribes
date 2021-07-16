using System;
using TribesOfDust.Hex;

namespace TribesOfDust.Utils
{
    public class TileAssetNotFoundException : Exception
    {

        /// <summary>
        /// Initializes a new <see cref="TileAssetNotFoundException"/> for a specific tile asset.
        /// The tile asset is known, but could not be found in another context.
        /// </summary>
        /// <param name="tileAsset">The tile asset that couldn't be found.</param>
        public TileAssetNotFoundException(TileAsset tileAsset)
            : base($"TileAsset {tileAsset} could not be found.")
        {
            TileAsset = tileAsset;
        }

        /// <summary>
        /// Initializes a new <see cref="TileAssetNotFoundException"/> for a tile asset
        /// with the specified <see cref="TileType"/> with all other information unknown.
        /// </summary>
        /// <param name="tileType">The type of the tile asset that couldn't be found.</param>
        public TileAssetNotFoundException(TileType tileType)
            : base($"TileAsset of type {tileType} could not be found.")
        {
            TileAsset = new TileAsset
            {
                Type = tileType
            };
        }

        /// <summary>
        /// Initializes a new <see cref="TileAssetNotFoundException"/> for a tile asset
        /// with the specified resource path and with all other information unknown.
        /// </summary>
        /// <param name="tileResourcePath">The path of the tile asset that couldn't be found.</param>
        public TileAssetNotFoundException(string tileResourcePath)
            : base($"TileAsset at path '{tileResourcePath}' not found.")
        {
            TileAsset = new TileAsset
            {
                ResourcePath = tileResourcePath
            };
        }

        /// <summary>
        /// The tile asset that could not be found.
        /// </summary>
        ///
        /// <remarks>
        /// This <see cref="TileAsset"/> might contain incomplete information. Only the parts of
        /// the tile asset that have been used for the search are provided by the exception.
        /// </remarks>
        public readonly TileAsset TileAsset;
    }
}