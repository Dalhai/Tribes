using Godot;

namespace TribesOfDust.Hex
{
    public static class HexConstants 
    {
        public static readonly float DefaultSize = 100.0f;
        public static readonly float DefaultWidth = 2.0f * DefaultSize;
        public static readonly float DefaultHeight = 2.0f * Mathf.Sqrt(3.0f / 4.0f * DefaultSize * DefaultSize);
        public static readonly float DefaultRatio = DefaultWidth / DefaultHeight;
    }
}