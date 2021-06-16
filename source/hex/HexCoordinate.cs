using System;
using Godot;

namespace TribesOfDust.Hex
{
    public static class CubeCoordinate
    {
        public static CubeCoordinate<int> FromQR(int q, int r) => new (q, -(q + r), r);
        public static CubeCoordinate<float> FromQR(float q, float r) => new (q, -(q + r), r);

        public static AxialCoordinate<int> ToAxialCoordinate(this CubeCoordinate<int> coordinate)
        {
            var (x, _, z) = coordinate;
            return new(x, z);
        }

        public static AxialCoordinate<float> ToAxialCoordinate(this CubeCoordinate<float> coordinate)
        {
            var (x, _, z) = coordinate;
            return new(x, z);
        }

        /// <summary> Round a cube coordinate to the nearest integer. </summary>
        /// <remarks> Rounds each element individually. </remarks>
        /// <param name="coordinate">The coordinate to round.</param>
        public static CubeCoordinate<int> Round(this CubeCoordinate<float> coordinate)
        {
            var x = (int) Mathf.Round(coordinate.X);
            var y = (int) Mathf.Round(coordinate.Y);
            var z = (int) Mathf.Round(coordinate.Z);
            return new(x, y, z);
        }
    }

    /// <summary>
    /// Cube coordinates based index.
    /// </summary>
    /// <param name="X">Left to right</param>
    /// <param name="Y">Bottom right to top left</param>
    /// <param name="Z">Top right to bottom left</param>
    public record CubeCoordinate<T>(T X, T Y, T Z) where T: notnull
    {
        public override int GetHashCode()
        {
            return (X.GetHashCode(),
                    Y.GetHashCode(),
                    Z.GetHashCode())
                .GetHashCode();
        }
    }

    public static class AxialCoordinate
    {
        public static CubeCoordinate<int> ToCubeCoordinate(this AxialCoordinate<int> coordinate)
        {
            var (q, r) = coordinate;
            return CubeCoordinate.FromQR(q, r);
        }

        public static CubeCoordinate<float> ToCubeCoordinate(this AxialCoordinate<float> coordinate)
        {
            var (q, r) = coordinate;
            return CubeCoordinate.FromQR(q, r);
        }

        /// <summary> Round an axial coordinate to the nearest integer. </summary>
        /// <remarks> Rounds each element individually. </remarks>
        /// <param name="coordinate">The coordinate to round.</param>
        public static AxialCoordinate<int> Round(this AxialCoordinate<float> coordinate)
        {
            var q = (int) Mathf.Round(coordinate.Q);
            var r = (int) Mathf.Round(coordinate.R);
            return new(q, r);
        }
    }

    /// <summary>
    /// Axial coordinates based index.
    /// </summary>
    /// <param name="Q">Left to right</param>
    /// <param name="R">Top right to bottom left</param>
    public record AxialCoordinate<T>(T Q, T R) where T: notnull
    {
        public override int GetHashCode()
        {
            return (Q.GetHashCode(),
                    R.GetHashCode())
                .GetHashCode();
        }
    }
}