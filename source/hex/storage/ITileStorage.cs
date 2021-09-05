using System;
using System.Collections.Generic;

namespace TribesOfDust.Hex.Storage
{
    public interface ITileStorage<T> : ITileStorageView<T>
    {
        /// <summary>
        /// Adds the item at the specified coordinates.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when there is already an item in the storage at the specified coordinates.
        /// </exception>
        ///
        /// <param name="coordinates">The coordinates to add the item at.</param>
        /// <param name="item">The item to add.</param>
        void Add(AxialCoordinate coordinates, T item);
        bool TryAdd(AxialCoordinate coordinates, T item);

        /// <summary>
        /// Removes the item at the specified coordinates.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when there is no item in the storage at the specified coordinates.
        /// </exception>
        ///
        /// <param name="coordinates">The coordinates of the item to remove.</param>
        void Remove(AxialCoordinate coordinates);
        bool TryRemove(AxialCoordinate coordinates);
    }
}