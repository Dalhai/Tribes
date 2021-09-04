using System;

namespace TribesOfDust.Assets
{
    public interface IAsset<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// The path to the resource in the virtual file system.
        /// </summary>
        string ResourcePath { get; }

        /// <summary>
        /// The key this asset should be mapped to.
        /// </summary>
        TKey Key { get; }
    }
}