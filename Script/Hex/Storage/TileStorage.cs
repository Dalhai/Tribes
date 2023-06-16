using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TribesOfDust.Core;

namespace TribesOfDust.Hex.Storage
{
    public class EntityTileStorage<T> : TileStorage<T> where T: Entity
    {
        #region Overrides

        public override bool Add(AxialCoordinate coordinates, T item)
        {
            if (Contains(coordinates))
                return false;

            var eventArgs = new TileStorageEventArgs<T>(coordinates, item);

            OnAdding(eventArgs);
            Items.Add(coordinates, item);
            _coords.Add(item.Id, coordinates);
            OnAdded(eventArgs);

            return true;
        }

        public override bool Remove(AxialCoordinate coordinates)
        {
            if (!Contains(coordinates))
                return false;

            var eventArgs = new TileStorageEventArgs<T>(coordinates, Items[coordinates]);

            OnRemoving(eventArgs);
            Items.Remove(coordinates);
            _coords.Remove(eventArgs.Item.Id);
            OnRemoved(eventArgs);

            return true;
        }

        public override bool Contains(T item) => _coords.ContainsKey(item.Id);
        public override AxialCoordinate? GetCoordinates(T item) => _coords.ContainsKey(item.Id) ? _coords[item.Id] : null;

        #endregion

        private readonly Dictionary<ulong, AxialCoordinate> _coords = new();
    }
    
    public class TileStorage<T> : ITileStorage<T> where T: notnull
    {
        #region Events

        public event EventHandler<AxialCoordinate>? RemovingAt;
        public event EventHandler<AxialCoordinate>? RemovedAt;
        public event EventHandler<AxialCoordinate>? AddingAt;
        public event EventHandler<AxialCoordinate>? AddedAt;

        public event EventHandler<TileStorageEventArgs<T>>? Removing;
        public event EventHandler<TileStorageEventArgs<T>>? Removed;
        public event EventHandler<TileStorageEventArgs<T>>? Adding;
        public event EventHandler<TileStorageEventArgs<T>>? Added;

        protected virtual void OnRemoving(TileStorageEventArgs<T> args)
        {
            Removing?.Invoke(this, args);
            RemovingAt?.Invoke(this, args.Coordinates);
        }

        protected virtual void OnRemoved(TileStorageEventArgs<T> args)
        {
            Removed?.Invoke(this, args);
            RemovedAt?.Invoke(this, args.Coordinates);
        }

        protected virtual void OnAdding(TileStorageEventArgs<T> args)
        {
            Adding?.Invoke(this, args);
            AddingAt?.Invoke(this, args.Coordinates);
        }

        protected virtual void OnAdded(TileStorageEventArgs<T> args)
        {
            Added?.Invoke(this, args);
            AddedAt?.Invoke(this, args.Coordinates);
        }

        #endregion
        #region Add / Remove

        public virtual bool Add(AxialCoordinate coordinates, T item)
        {
            if (Contains(coordinates))
                return false;

            var eventArgs = new TileStorageEventArgs<T>(coordinates, item);

            OnAdding(eventArgs);
            Items.Add(coordinates, item);
            OnAdded(eventArgs);

            return true;
        }

        public virtual bool Remove(AxialCoordinate coordinates)
        {
            if (!Contains(coordinates))
                return false;

            var eventArgs = new TileStorageEventArgs<T>(coordinates, Items[coordinates]);

            OnRemoving(eventArgs);
            Items.Remove(coordinates);
            OnRemoved(eventArgs);

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
}