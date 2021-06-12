using System;
using Godot;

using static System.Diagnostics.Trace;

namespace TribesOfDust.Hex
{
    public static class HexConversions
    {
        public static Vector2 HexToWorld(AxialCoordinate<int> coordinates, float size)
        {
            var x = size * (coordinates.Q * 3.0f / 2.0f);
            var y = size * (coordinates.Q * Mathf.Sqrt(3.0f) / 2.0f + coordinates.R * Mathf.Sqrt(3.0f));

            return new Vector2(x, y);
        }

        public static AxialCoordinate<int> WorldToHex(Vector2 position, float size)
        {
            var q = position.x * 2.0f / (3.0f * size);
            var r = position.x * -1.0f/ (3.0f * size) + position.y * 1.0f / (Mathf.Sqrt(3.0f) * size);

            return HexRound(CubeCoordinate.FromQR(q, r));
        }

        private static AxialCoordinate<int> HexRound(CubeCoordinate<float> coordinate)
        {
            var rounded = coordinate.Round();
            var delta = new CubeCoordinate<float>(
                Mathf.Abs(rounded.X - coordinate.X),
                Mathf.Abs(rounded.Y - coordinate.Y),
                Mathf.Abs(rounded.Z - coordinate.Z)
            );

            // Find the component with the largest offset and reset it to what is needed by the other components.
            // This is necessary because for cube coordinates, x + y + z must be fullfilled.

            int rx = rounded.X, ry = rounded.Y, rz = rounded.Z;
            // For our floating point cube coordinate this has to be the case. For our rounded
            // floating point cube coordinate, this might however not be the case anymore.
            // Readjusting the component that is wrong "the most", will fix this issue and give
            // us the correct coordinate again.

            if (delta.X >= delta.Y && delta.X >= delta.Z)
            {
                rx = -(ry + rz);
            }
            else if (delta.Y >= delta.X && delta.Y >= delta.Z)
            {
                ry = -(rx + rz);
            }
            else if (delta.Z >= delta.X && delta.Z >= delta.Y)
            {
                rz = -(rx + ry);
            }

            return new CubeCoordinate<int>(rx, ry, rz).ToAxialCoordinate();
        }
    }
}