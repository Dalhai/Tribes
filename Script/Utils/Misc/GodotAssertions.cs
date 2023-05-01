using static System.Diagnostics.Debug;

using Godot;

namespace TribesOfDust.Utils.Misc
{
    public static class GodotAssertions
    {
        #region Scale

        public static void AssertScaleNegative(Vector2 scale) => Assert(scale.X < 0.0f && scale.Y < 0.0f);
        public static void AssertScalePositive(Vector2 scale) => Assert(scale.X > 0.0f && scale.Y > 0.0f);
        public static void AssertScaleValid(Vector2 scale) => Assert(!scale.IsEqualApprox(Vector2.Zero));

        #endregion
        #region Texture

        public static void AssertTextureValid(Texture2D texture)
        {
            Assert(!Mathf.IsZeroApprox(texture.GetWidth()));
            Assert(!Mathf.IsZeroApprox(texture.GetHeight()));
        }

        #endregion
    }
}