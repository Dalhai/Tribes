using System;
using TribesOfDust.Core;

namespace TribesOfDust.Hex.Storage
{
    public interface ITileStorage<T> : ITileStorageView<T> 
    {
        #region Add

        /// <summary>
        /// Tries to add the item at the specified coordinates.
        /// </summary>
        /// <param name="coordinates">The coordinates to add the item at.</param>
        /// <param name="item">The item to add.</param>
        /// <returns>True, if the item was added, false otherwise.</returns>
        bool Add(AxialCoordinate coordinates, T item);

        #endregion
        #region Remove

        /// <summary>
        /// Removes the item at the specified coordinates.
        /// </summary>
        /// <param name="coordinates">The coordinates of the item to remove.</param>
        /// <returns>True, if the item was removed, false otherwise.</returns>
        bool Remove(AxialCoordinate coordinates);
        void Clear();

        #endregion
    }
}