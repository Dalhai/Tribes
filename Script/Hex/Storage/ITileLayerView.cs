using System;
using System.Collections.Generic;

namespace TribesOfDust.Hex.Storage;

public interface ITileLayerView
{
    event Action<ITileLayerView, AxialCoordinate>? RemovingAt;
    event Action<ITileLayerView, AxialCoordinate>? RemovedAt;
    event Action<ITileLayerView, AxialCoordinate>? AddingAt;
    event Action<ITileLayerView, AxialCoordinate>? AddedAt;

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

public interface ITileLayerView<T> :
    ITileLayerView,
    IEnumerable<KeyValuePair<AxialCoordinate, T>>
{
    event Action<ITileLayerView<T>, T, AxialCoordinate>? Removing;
    event Action<ITileLayerView<T>, T, AxialCoordinate>? Removed;
    event Action<ITileLayerView<T>, T, AxialCoordinate>? Adding;
    event Action<ITileLayerView<T>, T, AxialCoordinate>? Added;

    /// <summary>Gets the item at the specified coordinates.</summary>
    /// <param name="coordinates">The coordinates of the item to get.</param>
    /// <returns>The item at the specified coordinates, or null, if not available..</returns>
    T? Get(AxialCoordinate coordinates);

    /// <summary>Gets the coordinates of the item.</summary>
    /// <param name="item">The item to get the coordinates of.</param>
    /// <returns>The coordinate of the item, or null if the item is not in the storage.</returns>
    AxialCoordinate? GetCoordinates(T item);

    /// <summary>Checks if the storage contains the item.</summary>
    /// <param name="item">The item to check for.</param>
    /// <returns>True, if the item is in the storage, false otherwise.</returns>
    bool Contains(T item);
}