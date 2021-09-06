using Godot;

namespace TribesOfDust.Hex
{
    public static class HexConversions
    {
        public static Vector2 HexToWorld(AxialCoordinate coordinates, float size)
        {
            var x = size * (coordinates.Q * 3.0f / 2.0f);
            var y = size * (coordinates.Q * Mathf.Sqrt(3.0f) / 2.0f + coordinates.R * Mathf.Sqrt(3.0f));

            return new Vector2(x, y);
        }

        public static AxialCoordinate WorldToHex(Vector2 position, float size)
        {
            var q = position.x * 2.0f / (3.0f * size);
            var r = position.x * -1.0f/ (3.0f * size) + position.y * 1.0f / (Mathf.Sqrt(3.0f) * size);

            return HexRound(q, r);
        }

        private static AxialCoordinate HexRound(float q, float r)
        {
            Vector3 vector = new (q, -q - r, r);
            Vector3 rounded = vector.Round();
            Vector3 delta = (rounded - vector).Abs();

            // Find the component with the largest offset and reset it to what is needed by the other components.
            // This is necessary because for cube coordinates, x + y + z must be fullfilled.

            int rx = (int) rounded.x, ry = (int) rounded.y, rz = (int) rounded.z;

            // For our floating point cube coordinate this has to be the case. For our rounded
            // floating point cube coordinate, this might however not be the case anymore.
            // Readjusting the component that is wrong "the most", will fix this issue and give
            // us the correct coordinate again.

            if (delta.x >= delta.y && delta.x >= delta.z)
            {
                rx = -(ry + rz);
            }
            else if (delta.y >= delta.x && delta.y >= delta.z)
            {
                ry = -(rx + rz);
            }
            else if (delta.z >= delta.x && delta.z >= delta.y)
            {
                rz = -(rx + ry);
            }

            return new CubeCoordinate(rx, ry, rz).ToAxialCoordinate();
        }
    }
}