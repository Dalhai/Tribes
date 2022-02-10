using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TribesOfDust.Utils.Misc;

namespace TribesOfDust.Data
{
    public abstract class Repository<TVariation, TAsset> : IEnumerable<TAsset>
        where TAsset : IVariant<TVariation>
        where TVariation : notnull
    {

        public override string ToString() => $"Repository: {typeof(TVariation).Name} => {typeof(TAsset).Name}";

        #region Loading

        public void Load() => LoadAll().ForEach(AddVariation);

        /// <summary>
        /// Loads the default tiles of the repository.
        /// </summary>
        ///
        /// <remarks>
        /// A repository should provide a default way to initialize.
        /// This will however not be automatically called, but must be executed by users when they
        /// are ready to load - and unload - assets.
        /// </remarks>
        ///
        /// <returns>A list of loaded assets.</returns>
        protected abstract List<TAsset> LoadAll();

        /// <summary>
        /// Recrusively load all assets at the specified resource path.
        /// </summary>
        ///
        /// <exception cref="GodotException">
        /// Thrown when the directory can not be opened.
        /// </exception>
        ///
        /// <param name="resourcePath">The path from which to load assets.</param>
        ///
        /// <returns>A list of all assets that have been loaded, if there are any.</returns>
        /// <returns>An empty list if there were no assets at the specified path.</returns>
        protected List<TAsset> LoadAll(string resourcePath)
        {
            var dir = new Godot.Directory();
            var err = dir.Open(resourcePath);
            if (err != Godot.Error.Ok)
            {
                throw Error.Wrap(err);
            }

            var results = new List<TAsset>();

            // Iterate through all files in the directory.

            err = dir.ListDirBegin(skipNavigational: true);
            if (err != Godot.Error.Ok)
            {
                throw Error.Wrap(err);
            }

            var name = dir.GetNext();
            while (!string.IsNullOrEmpty(name))
            {
                string path = $"{resourcePath}/{name}";
                if (TryLoad(path, out TAsset? asset) && asset is not null)
                {
                    results.Add(asset);
                }

                name = dir.GetNext();
            }

            return results;
        }

        /// <summary>
        /// Try to load a single asset from the specified resource path.
        /// </summary>
        ///
        /// <param name="resourcePath">The path to the resource.</param>
        /// <param name="asset">The asset to be initialized, if found.</param>
        ///
        /// <returns>True, if the asset could be loaded, false otherwise.</returns>
        protected abstract bool TryLoad(string resourcePath, out TAsset? asset);

        #endregion
        #region Dictionary Access

        /// <summary>
        /// Adds an asset to the repository.
        /// </summary>
        ///
        /// <param name="asset">The asset to add.</param>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when the asset has already been added to the repository.
        /// </exception>
        public void AddVariation(TAsset asset)
        {
            if (!_assets.ContainsKey(asset.Key))
            {
                // List for specified asset variation does not yet exist.
                // Create a new list and add the asset without further checks.

                _assets.Add(asset.Key, new List<TAsset>());
                _assets[asset.Key].Add(asset);
            }
            else if (!_assets[asset.Key].Contains(asset))
            {
                // List already exists and the asset is not yet in the list.

                _assets[asset.Key].Add(asset);
            }
            else
            {
                // That's a problem. The asset shouldn't be in there yet when
                // adding a variation. Throw an exception correspondingly.

                throw Error.CantAddDuplicate(nameof(asset), this);
            }
        }

        /// <summary>
        /// Remove all assets of the specified variation.
        /// </summary>
        ///
        /// <param name="variation">The variation to remove.</param>
        public void RemoveAll(TVariation variation)
        {
            if (HasVariations(variation))
            {
                _assets.Remove(variation);
            }
        }

        /// <summary>
        /// Remove an asset at the index of the specified variation.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when the requested asset could not be found at the specified index of the variation.
        /// </exception>
        ///
        /// <param name="variation">The variation to remove from.</param>
        /// <param name="index">The index in the variation to remove.</param>
        public void RemoveVariation(TVariation variation, int index)
        {
            if (HasVariation(variation, index))
            {
                _assets[variation].RemoveAt(index);
            }
            else
            {
                throw Error.CantRemove(nameof(index), this);
            }
        }

        /// <summary>
        /// Removes an asset reference from the specified variation.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when the requested asset could not be found for the specified variation.
        /// </exception>
        ///
        /// <param name="variation">The variation to remove the asset from.</param>
        /// <param name="asset">The asset to remove.</param>
        public void RemoveVariation(TVariation variation, TAsset asset)
        {
            if (Contains(variation, asset))
            {
                _assets[variation].Remove(asset);
            }
            else
            {
                throw Error.CantRemove(nameof(asset), this);
            }
        }

        public bool Contains(TAsset asset) => Contains(asset.Key, asset);
        public bool Contains(TVariation variation, TAsset asset) => _assets.ContainsKey(variation) && _assets[variation].Contains(asset);
        public bool Contains(TVariation variation, int index) => _assets.ContainsKey(variation) && _assets[variation].Count >= index;

        public int Count => _assets.Sum(list => list.Value.Count);
        public int CountVariations(TVariation variation) => _assets.ContainsKey(variation) ? _assets[variation].Count : 0;

        public bool HasVariations(TVariation variation) => CountVariations(variation) > 0;
        public bool HasVariation(TVariation variation, int index) => CountVariations(variation) > index;

        /// <summary>
        /// Gets the asset at the specified index of a variation.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when the requested asset could not be found at the specified index of the variation.
        /// </exception>
        ///
        /// <param name="variation">The variation to get the asset from.</param>
        /// <param name="index">The index of the asset in the variation.</param>
        ///
        /// <returns>The requested asset.</returns>
        public TAsset GetAsset(TVariation variation, int index)
        {
            if (Contains(variation, index))
            {
                return _assets[variation][index];
            }

            throw Error.CantFind(nameof(index), this);
        }

        /// <summary>
        /// Gets a random asset of a variation.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when the specified variation does not exist in this repository.
        /// </exception>
        ///
        /// <param name="variation">The variation to get the asset from.</param>
        ///
        /// <returns>The requested assetThe requested asset..</returns>
        public TAsset GetAsset(TVariation variation)
        {
            if (HasVariations(variation))
            {
                int index = _random.Next(0, CountVariations(variation));
                var asset = GetAsset(variation, index);

                return asset;
            }

            throw Error.CantFind(nameof(variation), this);
        }

        /// <summary>
        /// Gets a random asset from a random variation.
        /// </summary>
        ///
        /// <exception cref="InvalidOperationException">
        /// Thrown when the repository is empty.
        /// </exception>
        ///
        /// <returns>The requested asset.</returns>
        public TAsset GetAsset()
        {
            var assets = this.ToList();

            if (assets.Count > 0)
            {
                // Note that we wouldn't get a uniform distribution if we first selected the variation
                // randomly and then selected the index randomly, as the lists might not have the
                // same number of elements.

                int index = _random.Next(0, assets.Count);
                return assets[index];
            }

            throw Error.InvalidEmpty(this);
        }

        #endregion
        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<TAsset>).GetEnumerator();
        IEnumerator<TAsset> IEnumerable<TAsset>.GetEnumerator() => _assets.SelectMany(list => list.Value).GetEnumerator();

        #endregion

        private readonly Random _random = new();
        private readonly Dictionary<TVariation, List<TAsset>> _assets = new();
    }
}