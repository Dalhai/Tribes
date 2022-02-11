using System;

namespace TribesOfDust.Data
{
    public interface IVariant<TKey> where TKey : notnull
    {
        /// <summary>
        /// The key this asset should be mapped to.
        /// </summary>
        TKey Key { get; }
    }
}