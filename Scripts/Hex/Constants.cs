using Godot;

namespace TribesOfDust.Hex
{
    public static class HexConstants 
    {
        /// <summary>
        /// The expected ratio for tile dimensions (516/448).
        /// </summary>
        public const float ExpectedTileRatio = 516.0f / 448.0f;
        
        /// <summary>
        /// The expected tile size (516x448).
        /// </summary>
        public static readonly Vector2I ExpectedTileSize = new(516, 448);
    }
}