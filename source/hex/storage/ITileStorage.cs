using System;

namespace TribesOfDust.Hex.Storage
{
    public interface ITileStorage<T> : ITileStorageView<T>
    {
        #region Add

        /// <summary>
        /// Adds the item at the specified coordinates.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when the item could not be added to the storage.
        /// </exception>
        ///
        /// <param name="coordinates">The coordinates to add the item at.</param>
        /// <param name="item">The item to add.</param>
        void Add(AxialCoordinate coordinates, T item)
        {
            if (!TryAdd(coordinates, item))
            {
                throw new ArgumentException($"Could not add item {item} at coordinates {coordinates} to storage {this}.");
            }
        }

        /// <summary>
        /// Tries to add the item at the specified coordinates.
        /// </summary>
        ///
        /// <param name="coordinates">The coordinates to add the item at.</param>
        /// <param name="item">The item to add.</param>
        ///
        /// <returns>True, if the item was added, false otherwise.</returns>
        bool TryAdd(AxialCoordinate coordinates, T item);

        #endregion
        #region Remove

        /// <summary>
        /// Removes the item at the specified coordinates.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when the item could not be removed from the storage.
        /// </exception>
        ///
        /// <param name="coordinates">The coordinates of the item to remove.</param>
        void Remove(AxialCoordinate coordinates)
        {
            if (!TryRemove(coordinates))
            {
                throw new ArgumentException($"Could not remove item at coordinates {coordinates} from storage {this}.");
            }
        }

        /// <summary>
        /// Removes the item at the specified coordinates.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when there is no item in the storage at the specified coordinates.
        /// </exception>
        ///
        /// <param name="coordinates">The coordinates of the item to remove.</param>
        bool TryRemove(AxialCoordinate coordinates);

        #endregion
    }
}