namespace TribesOfDust.Hex.Storage
{
    public static class TileStorageExtensions
    {
        public static bool Add<T>(this ITileStorage<T> storage, int q, int r, T item) => storage.Add(new(q, r), item);
        public static bool Add<T>(this ITileStorage<T> storage, int x, int y, int z, T item) => storage.Add(new CubeCoordinate(x, y, z), item);
        public static bool Add<T>(this ITileStorage<T> storage, CubeCoordinate coordinates, T item) => storage.Add(coordinates.ToAxialCoordinate(), item);

        public static bool Remove<T>(this ITileStorage<T> storage, int q, int r) => storage.Remove(new(q, r));
        public static bool Remove<T>(this ITileStorage<T> storage, int x, int y, int z) => storage.Remove(new CubeCoordinate(x, y, z));
        public static bool Remove<T>(this ITileStorage<T> storage, CubeCoordinate coordinates) => storage.Remove(coordinates.ToAxialCoordinate());
    }
}