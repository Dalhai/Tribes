namespace TribesOfDust.Hex.Storage
{
    public static class TileStorageExtensions
    {
        public static void Add<T>(this ITileStorage<T> storage, int q, int r, T item) => storage.Add(new(q, r), item);
        public static void Add<T>(this ITileStorage<T> storage, int x, int y, int z, T item) => storage.Add(new CubeCoordinate(x, y, z), item);
        public static void Add<T>(this ITileStorage<T> storage, CubeCoordinate coordinates, T item) => storage.Add(coordinates.ToAxialCoordinate(), item);

        public static bool TryAdd<T>(this ITileStorage<T> storage, int q, int r, T item) => storage.TryAdd(new(q, r), item);
        public static bool TryAdd<T>(this ITileStorage<T> storage, int x, int y, int z, T item) => storage.TryAdd(new CubeCoordinate(x, y, z), item);
        public static bool TryAdd<T>(this ITileStorage<T> storage, CubeCoordinate coordinates, T item) => storage.TryAdd(coordinates.ToAxialCoordinate(), item);

        public static void Remove<T>(this ITileStorage<T> storage, int q, int r) => storage.Remove(new(q, r));
        public static void Remove<T>(this ITileStorage<T> storage, int x, int y, int z) => storage.Remove(new CubeCoordinate(x, y, z));
        public static void Remove<T>(this ITileStorage<T> storage, CubeCoordinate coordinates) => storage.Remove(coordinates.ToAxialCoordinate());

        public static bool TryRemove<T>(this ITileStorage<T> storage, int q, int r) => storage.TryRemove(new(q, r));
        public static bool TryRemove<T>(this ITileStorage<T> storage, int x, int y, int z) => storage.TryRemove(new CubeCoordinate(x, y, z));
        public static bool TryRemove<T>(this ITileStorage<T> storage, CubeCoordinate coordinates) => storage.TryRemove(coordinates.ToAxialCoordinate());
    }
}