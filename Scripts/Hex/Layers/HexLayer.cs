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

    protected void OnRemoving(T item, AxialCoordinate location)
    {
        Removing?.Invoke(this, item, location);
        RemovingAt?.Invoke(this, location);
    }

    protected void OnRemoved(T item, AxialCoordinate location)
    {
        Removed?.Invoke(this, item, location);
        RemovedAt?.Invoke(this, location);
    }

    protected void OnAdding(T item, AxialCoordinate location)
    {
        Adding?.Invoke(this, item, location);
        AddingAt?.Invoke(this, location);
    }

    protected void OnAdded(T item, AxialCoordinate location)
    {
        Added?.Invoke(this, item, location);
        AddedAt?.Invoke(this, location);
    }

    #endregion
    #region Add

    public virtual bool TryAdd(AxialCoordinate location, T item)
    {
        if (Contains(location))
            return false;

        OnAdding(item, location);
        Items.Add(location, item);
        OnAdded(item, location);

        return true;
    }
    
    #endregion
    #region Remove

    public virtual bool TryRemove(AxialCoordinate location)
    {
        if (!Contains(location))
            return false;

        var item = Get(location);
        if (item is not null)
        {
            OnRemoving(item, location);
            Items.Remove(location);
            OnRemoved(item, location);
        }

        return true;
    }

    public virtual void Clear()
    {
        // Clone the list so we don't end up removing while iterating.
        // Unfortunately very inefficient if we clone often. Will be tackled later on.
        var coordinates = new List<AxialCoordinate>(Locations);

        // Iterate through all coordinates and remove them individually to trigger events.
        foreach (var coordinate in coordinates)
            TryRemove(coordinate);
    }

    #endregion
    #region Properties

    public int Count => Items.Count;
    public bool IsEmpty => Count == 0;
    public bool IsConstrained => false;

    public IEnumerable<AxialCoordinate> Locations => Items.Keys;

    #endregion
    #region Contains

    public virtual bool Contains(AxialCoordinate location) => Items.ContainsKey(location);
    public virtual bool Contains(T item) => Items.ContainsValue(item);

    #endregion
    #region Get

    public virtual AxialCoordinate? GetCoordinates(T item) => Items.First(entry => entry.Value.Equals(item)).Key;
    public virtual T? Get(AxialCoordinate location) => Items.ContainsKey(location) ? Items[location] : default;

    #endregion
    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
    IEnumerator<KeyValuePair<AxialCoordinate, T>> IEnumerable<KeyValuePair<AxialCoordinate, T>>.GetEnumerator() => Items.GetEnumerator();

    #endregion

    protected readonly Dictionary<AxialCoordinate, T> Items = new();
}