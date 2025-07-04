using Godot;

namespace TribesOfDust.Hex
{
    public static class HexConstants 
    {
        /// <summary>
        /// The expected tile size (516x448).
        /// </summary>
        public static readonly Vector2I ExpectedTileSize = new(516, 448);
        
        /// <summary>
        /// The expected ratio for tile dimensions, computed from ExpectedTileSize.
        /// </summary>
        public static readonly float ExpectedTileRatio = (float)ExpectedTileSize.X / ExpectedTileSize.Y;
    }
}