using System;

namespace TribesOfDust.Hex.Storage
{
    /// <summary>
    /// The <see cref="ConstraintViolationException{T}"/> is used to notify clients of
    /// an issue with tryign to modify a <see cref="ConstraintedTileStorage{T}"/> when
    /// the constraining <see cref="TileStorage{T}"/> does not allow the modification.
    /// </summary>
    ///
    /// <typeparam name="T">The type of items in the tile storage.</typeparam>
    public class ConstraintViolationException<T> : Exception
    {
        public ConstraintViolationException(
            AxialCoordinate coordinates,
            ITileStorageView constraint,
            ITileStorageView<T> constrained)
        {
            Coordinates = coordinates;
            Constraint = constraint;
            Constrained = constrained;
        }

        public ConstraintViolationException(
            AxialCoordinate coordinates,
            ITileStorageView constraint,
            ITileStorageView<T> constrained,
            string message) : base(message)
        {
            Coordinates = coordinates;
            Constraint = constraint;
            Constrained = constrained;
        }

        public ConstraintViolationException(
            AxialCoordinate coordinates,
            ITileStorageView constraint,
            ITileStorageView<T> constrained,
            string message,
            Exception innerException) : base(message, innerException)
        {
            Coordinates = coordinates;
            Constraint = constraint;
            Constrained = constrained;
        }

        /// <summary>
        /// The coordinates at which the constraint was violated.
        /// </summary>
        public readonly AxialCoordinate Coordinates;

        /// <summary>
        /// The tile storage providing the constraint.
        /// </summary>
        public readonly ITileStorageView Constraint;

        /// <summary>
        /// The tile storage being constrained by the constraint.
        /// </summary>
        public readonly ITileStorageView<T> Constrained;
    }
}