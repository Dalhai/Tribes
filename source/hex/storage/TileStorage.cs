using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TribesOfDust.Hex.Storage
{
    public class TileStorage<T> : ITileStorage<T>
    {
        #region Constructors

        public TileStorage() : this(EqualityComparer<T>.Default) {}
        public TileStorage(IEqualityComparer<T> comparer)
        {
            _comparer = comparer;
        }

        #endregion
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
            if (Removing is not null)
                Removing(this, args);
            if (RemovingAt is not null)
                RemovingAt(this, args.Coordinates);
        }

        protected virtual void OnRemoved(TileStorageEventArgs<T> args)
        {
            if (Removed is not null)
                Removed(this, args);
            if (RemovedAt is not null)
                RemovedAt(this, args.Coordinates);
        }

        protected virtual void OnAdding(TileStorageEventArgs<T> args)
        {
            if (Adding is not null)
                Adding(this, args);
            if (AddingAt is not null)
                AddingAt(this, args.Coordinates);
        }

        protected virtual void OnAdded(TileStorageEventArgs<T> args)
        {
            if (Added is not null)
                Added(this, args);
            if (AddedAt is not null)
                AddedAt(this, args.Coordinates);
        }

        #endregion
        #region Add / Remove

        public virtual bool Add(AxialCoordinate coordinates, T item)
        {
            if (Contains(coordinates))
                return false;

            var eventArgs = new TileStorageEventArgs<T>(coordinates, item);

            OnAdding(eventArgs);
            _items.Add(coordinates, item);
            OnAdded(eventArgs);

            return true;
        }

        public virtual bool Remove(AxialCoordinate coordinates)
        {
            if (!Contains(coordinates))
                return false;

            var eventArgs = new TileStorageEventArgs<T>(coordinates, _items[coordinates]);

            OnRemoving(eventArgs);
            _items.Remove(coordinates);
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

        public int Count => _items.Count;
        public bool IsEmpty => Count == 0;
        public bool IsConstrained => false;

        public IEnumerable<AxialCoordinate> Coordinates => _items.Keys;

        #endregion
        #region Contains

        public bool Contains(AxialCoordinate coordinates) => _items.ContainsKey(coordinates);
        public bool Contains(T item) => _items.ContainsValue(item);

        #endregion
        #region Get

        public IEnumerable<AxialCoordinate> GetCoordinates(T item) => _items
            .Where(stored => _comparer.Equals(stored.Value, item))
            .Select(stored => stored.Key);

        public T? Get(AxialCoordinate coordinates) => _items.ContainsKey(coordinates) ? _items[coordinates] : default;

        #endregion
        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
        IEnumerator<KeyValuePair<AxialCoordinate, T>> IEnumerable<KeyValuePair<AxialCoordinate, T>>.GetEnumerator() => _items.GetEnumerator();

        #endregion

        private readonly Dictionary<AxialCoordinate, T> _items = new();
        private readonly IEqualityComparer<T> _comparer;
    }
}