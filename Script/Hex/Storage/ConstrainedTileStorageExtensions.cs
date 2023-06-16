using System.Collections.Generic;
using TribesOfDust.Core;

namespace TribesOfDust.Hex.Storage
{
    public static class ConstraintedTileStorageExtensions
    {
        public static ITileStorage<T> Constrain<T>(this ITileStorageView storageView) where T: Entity
        {
            return new ConstraintedTileStorage<T>(storageView);
        }
    }
}