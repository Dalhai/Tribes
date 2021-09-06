using static System.Diagnostics.Debug;

using Godot;

namespace TribesOfDust.Utils.Godot
{
    public static class GodotAssertions
    {
        #region Scale

        public static void AssertScaleNegative(Vector2 scale) => Assert(scale.x < 0.0f && scale.y < 0.0f);
        public static void AssertScalePositive(Vector2 scale) => Assert(scale.x > 0.0f && scale.y > 0.0f);
        public static void AssertScaleValid(Vector2 scale) => Assert(!scale.IsEqualApprox(Vector2.Zero));

        #endregion
        #region Texture

        public static void AssertTextureValid(Texture texture)
        {
            Assert(!Mathf.IsZeroApprox(texture.GetWidth()));
            Assert(!Mathf.IsZeroApprox(texture.GetHeight()));
        }

        #endregion
    }
}