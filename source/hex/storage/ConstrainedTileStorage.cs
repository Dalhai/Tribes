using System.Collections;
using System.Collections.Generic;

namespace TribesOfDust.Hex.Storage
{
    public class ConstraintedTileStorage<T> : TileStorage<T>
    {
        public ConstraintedTileStorage(ITileStorageView constraint) : this(constraint, EqualityComparer<T>.Default)
        {
        }

        public ConstraintedTileStorage(ITileStorageView constraint, IEqualityComparer<T> comparer) : base(comparer)
        {
            _constraint = constraint;
            _constraint.RemovingAt += OnConstraintItemRemoving;
        }

        /// <summary>
        /// Adds an item at the specified coordinates.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when there is already an item in the storage at the specified coordinates.
        /// </exception>
        ///
        /// <exception cref="ConstraintViolationException{T}">
        /// Thrown when there is not item at the coordinates in the constraint tile storage.
        /// You can only add items to a constrained tile storage in places where the base
        /// constraint already has an item.
        /// </exception>
        ///
        /// <param name="coordinates">The coordinates to add the item at.</param>
        /// <param name="item">The item to add.</param>
        public override void Add(AxialCoordinate coordinates, T item)
        {
            if (!_constraint.Contains(coordinates))
            {
                throw new ConstraintViolationException<T>(
                    coordinates,
                    _constraint,
                    this,

                    $"Could not add item at coordinates {coordinates} due to violation of constraint." +
                     "Ensure that the base tile storage of this constrained storage already has an item " +
                     "at the specified coordinates."
                );
            }

            base.Add(coordinates, item);
        }

        public override bool TryAdd(AxialCoordinate coordinates, T item) => _constraint.Contains(coordinates) && base.TryAdd(coordinates, item);

        private void OnConstraintItemRemoving(object? sender, AxialCoordinate coordinates)
        {
            if (Contains(coordinates))
                Remove(coordinates);
        }

        private readonly ITileStorageView _constraint;
    }
}