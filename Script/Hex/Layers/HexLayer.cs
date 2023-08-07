using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TribesOfDust.Hex.Layers;
    
public class HexLayer<T> : IHexLayer<T> where T: notnull
{
    #region Events

    public event Action<IHexLayerView, AxialCoordinate>? RemovingAt;
    public event Action<IHexLayerView, AxialCoordinate>? RemovedAt;
    public event Action<IHexLayerView, AxialCoordinate>? AddingAt;
    public event Action<IHexLayerView, AxialCoordinate>? AddedAt;

    public event Action<IHexLayerView<T>, T, AxialCoordinate>? Removing;
    public event Action<IHexLayerView<T>, T, AxialCoordinate>? Removed;
    public event Action<IHexLayerView<T>, T, AxialCoordinate>? Adding;
    public event Action<IHexLayerView<T>, T, AxialCoordinate>? Added;

    protected void OnRemoving(T item, AxialCoordinate coordinates)
    {
        Removing?.Invoke(this, item, coordinates);
        RemovingAt?.Invoke(this, coordinates);
    }

    protected void OnRemoved(T item, AxialCoordinate coordinates)
    {
        Removed?.Invoke(this, item, coordinates);
        RemovedAt?.Invoke(this, coordinates);
    }

    protected void OnAdding(T item, AxialCoordinate coordinates)
    {
        Adding?.Invoke(this, item, coordinates);
        AddingAt?.Invoke(this, coordinates);
    }

    protected void OnAdded(T item, AxialCoordinate coordinates)
    {
        Added?.Invoke(this, item, coordinates);
        AddedAt?.Invoke(this, coordinates);
    }

    #endregion
    #region Add

    public virtual bool Add(T item, AxialCoordinate coordinates)
    {
        if (Contains(coordinates))
            return false;

        OnAdding(item, coordinates);
        Items.Add(coordinates, item);
        OnAdded(item, coordinates);

        return true;
    }
    
    #endregion
    #region Remove

    public virtual bool Remove(AxialCoordinate coordinates)
    {
        if (!Contains(coordinates))
            return false;

        var item = Get(coordinates);
        if (item is not null)
        {
            OnRemoving(item, coordinates);
            Items.Remove(coordinates);
            OnRemoved(item, coordinates);
        }

        return true;
    }

    public virtual void Clear()
    {
        // Clone the list so we don't end up removing while iterating.
        // Unfortunately very inefficient if we clone often. Will be tackled later on.
        var coordinates = new List<AxialCoordinate>(Coordinates);

        // Iterate through all coordinates and remove them individually to trigger events.
        foreach (var coordinate in coordinates)
            Remove(coordinate);
    }

    #endregion
    #region Properties

    public int Count => Items.Count;
    public bool IsEmpty => Count == 0;
    public bool IsConstrained => false;

    public IEnumerable<AxialCoordinate> Coordinates => Items.Keys;

    #endregion
    #region Contains

    public virtual bool Contains(AxialCoordinate coordinates) => Items.ContainsKey(coordinates);
    public virtual bool Contains(T item) => Items.ContainsValue(item);

    #endregion
    #region Get

    public virtual AxialCoordinate? GetCoordinates(T item) => Items.First(entry => entry.Value.Equals(item)).Key;
    public virtual T? Get(AxialCoordinate coordinates) => Items.ContainsKey(coordinates) ? Items[coordinates] : default;

    #endregion
    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
    IEnumerator<KeyValuePair<AxialCoordinate, T>> IEnumerable<KeyValuePair<AxialCoordinate, T>>.GetEnumerator() => Items.GetEnumerator();

    #endregion

    protected readonly Dictionary<AxialCoordinate, T> Items = new();
}