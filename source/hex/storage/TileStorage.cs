using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime;

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

        public virtual void Add(AxialCoordinate coordinates, T item)
        {
            if (Contains(coordinates))
            {
                throw new ArgumentException(
                    $"Could not add {item} at coordinates {coordinates}. " +
                     "An item is already in the storage at the specified coordinates.",
                    "coordinates"
                );
            }

            AddUnchecked(coordinates, item);
        }

        public virtual bool TryAdd(AxialCoordinate coordinates, T item)
        {
            if (Contains(coordinates))
                return false;

            AddUnchecked(coordinates, item);
            return true;
        }

        protected void AddUnchecked(AxialCoordinate coordinates, T item)
        {
            var eventArgs = new TileStorageEventArgs<T>(coordinates, item);

            OnAdding(eventArgs);
            _items.Add(coordinates, item);
            OnAdded(eventArgs);
        }

        public virtual void Remove(AxialCoordinate coordinates)
        {
            if (!Contains(coordinates))
            {
                throw new ArgumentException(
                    $"Could not remove item at coordinates {coordinates}. " +
                     "No item is in the storage at the specified coordinates.",
                    "coordinates"
                );
            }

            RemoveUnchecked(coordinates);
        }

        public virtual bool TryRemove(AxialCoordinate coordinates)
        {
            if (!Contains(coordinates))
                return false;

            RemoveUnchecked(coordinates);
            return true;
        }

        protected void RemoveUnchecked(AxialCoordinate coordinates)
        {
            var eventArgs = new TileStorageEventArgs<T>(coordinates, _items[coordinates]);

            OnRemoving(eventArgs);
            _items.Remove(coordinates);
            OnRemoved(eventArgs);
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

        public AxialCoordinate GetCoordinates(T item)
        {
            try
            {
                return _items.First(stored => _comparer.Equals(stored.Value, item)).Key;
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException($"Could not get coordinates for item {item}.", "item", e);
            }
        }

        public T Get(AxialCoordinate coordinates)
        {
            if (Contains(coordinates))
            {
                return _items[coordinates];
            }

            throw new ArgumentException($"Could not get item at coordinates {coordinates}.", "coordinates");
        }

        public bool TryGet(AxialCoordinate coordinates, out T? item)
        {
            if (Contains(coordinates))
            {
                item = _items[coordinates];
                return true;
            }

            item = default;
            return false;
        }

        #endregion
        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<T>).GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _items.Values.GetEnumerator();
        IEnumerator<KeyValuePair<AxialCoordinate, T>> IEnumerable<KeyValuePair<AxialCoordinate, T>>.GetEnumerator() => _items.GetEnumerator();

        #endregion

        private readonly Dictionary<AxialCoordinate, T> _items = new();
        private readonly IEqualityComparer<T> _comparer;
    }
}