using System.Collections.Generic;

namespace TribesOfDust.Hex.Storage
{
    public static class ConstraintedTileStorageExtensions
    {
        public static ITileStorage<T> Constrain<T>(this ITileStorageView storageView)
        {
            return new ConstraintedTileStorage<T>(storageView);
        }

        public static ITileStorage<T> Constrain<T>(this ITileStorageView storageView, IEqualityComparer<T> comparer)
        {
            return new ConstraintedTileStorage<T>(storageView, comparer);
        }
    }
}