using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Godot;

using TribesOfDust.Hex;
using TribesOfDust.Utils;

namespace TribesOfDust.Hex
{
    public class TileAssetRepository : IEnumerable<TileAsset>
    {
        /// <summary>
        /// Initializes an empty <see cref="TileAssetRepository"/>.
        /// </summary>
        public TileAssetRepository()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="TileAssetRepository"/> using the given tile assets.
        /// </summary>
        ///
        /// <remarks>
        /// The tile asset repository provides ways to manipulate the loaded tile assets.
        /// The initial set of tiles should therefore not be considered set in stone and
        /// access to tile assets should be restricted to go through the repository.
        /// </remarks>
        ///
        /// <param name="tileAssets">The initial set of tile assets for the repository.</param>
        public TileAssetRepository(IEnumerable<TileAsset> tileAssets)
        {
            foreach (TileAsset asset in tileAssets)
            {
                Add(asset);
            }
        }

        #region Properties

        /// <summary>
        /// Gets all tile assets in the repository.
        /// </summary>
        public IEnumerable<TileAsset> TileAssets => tileAssets;

        /// <summary>
        /// Gets all tile types in the repository.
        /// </summary>
        public IEnumerable<TileType> TileTypes => tileAssets.Select(tileAsset => tileAsset.Type);

        #endregion


        #region Add Assets

        /// <summary>
        /// Adds a new <see cref="TileAsset"/> to the repository.
        /// </summary>
        ///
        /// <remarks>
        /// The tile asset is automatically registered as a new variation of the tile
        /// asset tile type, if a tile asset for the specified type has already been
        /// registered.
        ///
        /// For now, this results in tile assets being addable multiple times, leading to
        /// potentially undesired behaviour.
        /// </remarks>
        ///
        /// <param name="tileAsset">The tile asset to add.</param>
        public void Add(TileAsset tileAsset)
        {
            if (!tileVariations.ContainsKey(tileAsset.Type))
                tileVariations.Add(tileAsset.Type, new List<TileAsset>());

            tileVariations[tileAsset.Type].Add(tileAsset);
            tileAssets.Add(tileAsset);
        }

        /// <summary>
        /// Removes all occurrences of the specified <see cref="TileType"/>.
        /// </summary>
        /// <remarks>If the tile type is not available, does nothing.</remarks>
        /// <param name="type">The tile type to remove.</param>
        public void Remove(TileType type)
        {
            tileAssets.RemoveAll(tileAsset => tileAsset.Type == type);
            tileVariations.Remove(type);
        }

        /// <summary>
        /// Removes a specific variation of the specified <see cref="TileType"/>.
        /// </summary>
        /// <remarks>If the variation does not exist, does nothing.</remarks>
        /// <param name="type">The tile type to remove.</param>
        /// <param name="variation">The variation to remove.</param>
        public void Remove(TileType type, int variation)
        {
            if (tileVariations.ContainsKey(type))
            {
                List<TileAsset> variations = tileVariations[type];

                if (0 >= variation && variation < variations.Count)
                {
                    TileAsset removed = variations[variation];
                    variations.RemoveAt(variation);
                    tileAssets.Remove(removed);
                }
            }
        }

        #endregion


        #region Access Assets

        /// <summary>
        /// Gets all tile variations of the specified <see cref="TileType"/>.
        /// </summary>
        ///
        /// <exception cref="TileTypeNotFoundException">
        /// Thrown when no tile variations for the requested tile type have been registered.
        /// </exception>
        ///
        /// <param name="type">The tile type.</param>
        /// <returns>An enumeration of <see cref="TileAsset"/> instances.</returns>
        public IEnumerable<TileAsset> GetVariations(TileType type)
        {
            EnsureTypeHasVariations(type);
            return tileVariations[type];
        }

        /// <summary>
        /// Gets a tile variation for the specified <see cref="TileType"/>.
        /// </summary>
        ///
        /// <exception cref="TileTypeNotFoundException">
        /// Thrown when no tile variations for the requested tile type have been registered.
        /// </exception>
        /// <exception cref="TileVariationNotFoundException">
        /// Thrown when the requested tile variation does not exist.
        /// </exception>
        ///
        /// <param name="type">The tile type.</param>
        /// <param name="variation">The tile variation index.</param>
        /// <returns>A <see cref="TileAsset"/> for the specified tile variation.</returns>
        public TileAsset GetVariation(TileType type, int variation)
        {
            EnsureVariationExists(type, variation);
            return tileVariations[type][variation];
        }

        /// <summary>
        /// Gets a random tile variation for the specified <see cref="TileType"/>.
        /// </summary>
        ///
        /// <exception cref="TileTypeNotFoundException">
        /// Thrown when no tile variations for the requested tile type have been registered.
        /// </exception>
        ///
        /// <param name="type">The tile type.</param>
        /// <returns>A random <see cref="TileAsset"/> variation.</returns>
        public TileAsset GetRandomVariation(TileType type)
        {
            EnsureTypeHasVariations(type);

            List<TileAsset> variations = tileVariations[type];
            int selectedVariation = random.Next(0, variations.Count - 1);

            return variations[selectedVariation];
        }

        #endregion


        #region IEnumerable
        /*
         * IEnumerable Implementation
         *
         * We implement `IEnumerable<TileAsset>` for convenience. Compared to `IDictionary<TileType List<TileAsset>>`
         * it's a very slim interface and does not cause much overhead. We could have just left it at exposing the
         * tile assets as enumerable directly, but this allows for an a bit more implicit style of manipulating the
         * repository.
         */

        /// <summary>
        /// Gets an enumerator for the tile assets.
        /// </summary>
        ///
        /// <returns>A new tile asset enumerator.</returns>
        public IEnumerator<TileAsset> GetEnumerator() => tileAssets.GetEnumerator();

        /// <summary>
        /// Gets an enumerator for the tile assets.
        /// </summary>
        ///
        /// <returns>A new tile asset enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => tileAssets.GetEnumerator();

        #endregion

        private void EnsureTypeHasVariations(TileType type)
        {
            if (!tileVariations.ContainsKey(type))
            {
                throw new TileTypeNotFoundException(type);
            }

            if (tileVariations[type].Count == 0)
            {
                throw new TileTypeNotFoundException(type);
            }
        }

        private void EnsureVariationExists(TileType type, int variation)
        {
            // Check if there are tile variations stored for the specified type.

            if (!tileVariations.ContainsKey(type))
            {
                throw new TileTypeNotFoundException(type);
            }

            // Check if there are enough variations to support the requested one.

            if (tileVariations[type].Count <= variation)
            {
                throw new TileVariationNotFoundException(type, variation);
            }
        }

        private readonly Dictionary<TileType, List<TileAsset>> tileVariations = new();
        private readonly List<TileAsset> tileAssets = new();

        private static readonly Random random = new();
    }
}