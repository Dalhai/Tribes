using System.Collections.Generic;

namespace TribesOfDust.Hex.Storage
{
    public partial class ConstraintedTileStorage<T> : TileStorage<T>
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
        /// Tries to add the item at the specified coordinates.
        ///
        /// <br/>
        /// To add an item to a constrained storage, it must be part of the constraint.
        /// The constraint is the storage from which this constrained storage has been
        /// built.
        /// </summary>
        ///
        /// <example>
        /// var constraint = new TileStorage<int>();
        /// var storage = constraint.Constrain<float>();
        /// constraint.Add(0, 0, 1);
        ///
        /// Debug.Assert(storage.Add(0, 0, 0.5));
        /// Debug.Assert(!storage.Add(0, 1, 1.5));
        /// </example>
        ///
        /// <param name="coordinates">The coordinates to add the item at.</param>
        /// <param name="item">The item to add.</param>
        /// <returns>True, if the item was added, false otherwise.</returns>
        public override bool Add(AxialCoordinate coordinates, T item) => _constraint.Contains(coordinates) && base.Add(coordinates, item);

        private void OnConstraintItemRemoving(object? sender, AxialCoordinate coordinates)
        {
            if (Contains(coordinates))
            {
                Remove(coordinates);
            }
        }

        private readonly ITileStorageView _constraint;
    }
}