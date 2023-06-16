
using System;
using TribesOfDust.Core;

namespace TribesOfDust.Hex.Storage
{
    public static class ITileStorageViewExtensions
    {
        public static bool Contains(this ITileStorageView storageView, int q, int r) => storageView.Contains(new(q, r));
        public static bool Contains(this ITileStorageView storageView, int x, int y, int z) => storageView.Contains(new CubeCoordinate(x, y, z));
        public static bool Contains(this ITileStorageView storageView, CubeCoordinate coordinates) => storageView.Contains(coordinates.ToAxialCoordinate());

        /// <summary>
        /// Gets the item at the specified coordinates.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when the item could not be found in the storage.
        /// </exception>
        ///
        /// <param name="storageView">The storage view to get the item from.</param>
        /// <param name="q">The q - component of an axial coordinate.</param>
        /// <param name="r">The r - component of an axial coordinate.</param>
        ///
        /// <returns>The item at the specified coordinates.</returns>
        public static T? Get<T>(this ITileStorageView<T> storageView, int q, int r) where T: Entity => storageView.Get(new(q, r));

        /// <summary>
        /// Gets the item at the specified coordinates.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when the item could not be found in the storage.
        /// </exception>
        ///
        /// <param name="storageView">The storage view to get the item from.</param>
        /// <param name="x">The x - component of a cube coordinate.</param>
        /// <param name="y">The y - component of a cube coordinate.</param>
        /// <param name="z">The z - component of a cube coordinate.</param>
        ///
        /// <returns>The item at the specified coordinates.</returns>
        public static T? Get<T>(this ITileStorageView<T> storageView, int x, int y, int z) where T: Entity => storageView.Get(new CubeCoordinate(x, y, z));

        /// <summary>
        /// Gets the item at the specified coordinates.
        /// </summary>
        /// 
        /// <exception cref="ArgumentException">
        /// Thrown when the item could not be found in the storage.
        /// </exception>
        /// 
        /// <param name="storageView">The storage view to get the item from.</param>
        /// <param name="coordinates">The coordinates of the item to get.</param>
        /// 
        /// <returns>The item at the specified coordinates.</returns>
        public static T? Get<T>(this ITileStorageView<T> storageView, CubeCoordinate coordinates) where T: Entity => storageView.Get(coordinates.ToAxialCoordinate());
    }
}