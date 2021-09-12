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

        /// <summary>Checks if the storage contains the coordinates.</summary>
        /// <param name="coordinates">The coordinates to chek for.</param>
        /// <returns>True, if the storage has an item at the coordinates, false otherwise.</returns>
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
        IEnumerable<KeyValuePair<AxialCoordinate, T>>
    {
        event EventHandler<TileStorageEventArgs<T>>? Removing;
        event EventHandler<TileStorageEventArgs<T>>? Removed;
        event EventHandler<TileStorageEventArgs<T>>? Adding;
        event EventHandler<TileStorageEventArgs<T>>? Added;

        /// <summary>Gets the item at the specified coordinates.</summary>
        /// <param name="coordinates">The coordinates of the item to get.</param>
        /// <returns>The item at the specified coordinates, or null, if not available..</returns>
        T? Get(AxialCoordinate coordinates);

        /// <summary>Checks if the storage contains the item.</summary>
        /// <param name="item">The item to check for.</param>
        /// <returns>True, if the item is in the storage, false otherwise.</returns>
        bool Contains(T item);

        /// <summary>Gets the coordinates of the item.</summary>
        /// <remarks>Potentially very slow as possibly every entry has to be inspected.</remarks>
        /// <param name="item">The item to get the coordinates of.</param>
        /// <returns>A list of coordinates this item is stored at.</returns>
        IEnumerable<AxialCoordinate> GetCoordinates(T item);
    }
}