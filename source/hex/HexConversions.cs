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

            return HexRound(new AxialCoordinate<float>(q, r).ToCubeCoordinate());
        }

        private static AxialCoordinate<int> HexRound(CubeCoordinate<float> cubeCoordinate)
        {
            var roundedCubeCoordinate = cubeCoordinate.Round();
            var dx = Mathf.Abs(roundedCubeCoordinate.X - cubeCoordinate.X);
            var dy = Mathf.Abs(roundedCubeCoordinate.Y - cubeCoordinate.Y);
            var dz = Mathf.Abs(roundedCubeCoordinate.Z - cubeCoordinate.Z);

            var (rx, ry, rz) = roundedCubeCoordinate;

            if (dx > dy && dx > dz)
            {
                rx = -ry - rz;
            }
            else if (dy <= dz)
            {
                rz = -rx - ry;
            }

            return new(rx, rz);
        }
    }
}