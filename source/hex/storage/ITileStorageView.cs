using System;
using System.Collections;
using System.Collections.Generic;

namespace TribesOfDust.Hex.Storage
{
    public interface ITileStorageView
    {
        event EventHandler<AxialCoordinate>? RemovingAt;
        event EventHandler<AxialCoordinate>? RemovedAt;
        event EventHandler<AxialCoordinate>? AddingAt;
        event EventHandler<AxialCoordinate>? AddedAt;

        /// <summary>
        /// Checks if the storage contains the coordinates.
        /// </summary>
        ///
        /// <param name="coordinates">The coordinates to chek for.</param>
        ///
        /// <returns>
        /// True, if the storage contains the coordinates.<br/>
        /// False, otherwise.
        /// </returns>
        bool Contains(AxialCoordinate coordinates);

        /// <summary>
        /// Gets the number of items in the storage.
        /// </summary>
        int Count { get; }

        bool IsEmpty { get; }
        bool IsConstrained { get; }

        /// <summary>
        /// The coordinates in this tile storage.
        /// </summary>
        IEnumerable<AxialCoordinate> Coordinates { get; }
    }

    public interface ITileStorageView<T> :
        ITileStorageView,
        IEnumerable<T>,
        IEnumerable<KeyValuePair<AxialCoordinate, T>>
    {
        event EventHandler<TileStorageEventArgs<T>>? Removing;
        event EventHandler<TileStorageEventArgs<T>>? Removed;
        event EventHandler<TileStorageEventArgs<T>>? Adding;
        event EventHandler<TileStorageEventArgs<T>>? Added;

        /// <summary>
        /// Gets the item at the specified coordinates.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when the item could not be found in the storage.
        /// </exception>
        ///
        /// <param name="coordinates">The coordinates of the item to get.</param>
        ///
        /// <returns>The item at the specified coordinates.</returns>
        T Get(AxialCoordinate coordinates)
        {
            if (TryGet(coordinates, out T? item) && item is not null)
            {
                return item;
            }

            throw new ArgumentException($"Could not get item at coordinates {coordinates} from storage {this}.");
        }

        /// <summary>
        /// Tries to get the item at the specified coordinates.
        /// </summary>
        ///
        /// <param name="coordinates">The coordinates of the item.</param>
        /// <param name="item">The item, if it exists.</param>
        ///
        /// <returns>
        /// True, if there is an item associated with the coordinates. <br/>
        /// False, if there is no item associated with the coordinates.
        /// </returns>
        bool TryGet(AxialCoordinate coordinates, out T? item);

        /// <summary>
        /// Checks if the storage contains the item.
        /// </summary>
        ///
        /// <param name="item">The item to check for.</param>
        ///
        /// <returns>
        /// True, if the storage contains the item. <br/>
        /// False, otherwise.
        /// </returns>
        bool Contains(T item);

        /// <summary>
        /// Gets the coordinates of the item.
        /// </summary>
        ///
        /// <remarks>Potentially very slow as possibly every entry has to be inspected.</remarks>
        /// <param name="item">The item to get the coordinates of.</param>
        ///
        /// <returns>A list of coordinates this item is stored at.</returns>
        List<AxialCoordinate> GetCoordinates(T item);
    }
}