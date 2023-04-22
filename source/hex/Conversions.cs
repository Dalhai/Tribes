using Godot;

namespace TribesOfDust.Hex
{
    public static class HexConversions
    {
        public static readonly float SideLength = 1.0f;
        public static readonly float CornerDistance = SideLength;

        /// <summary>
        /// The orthogonal projected distance from the center of a tile to any side.
        /// 
        /// Simple Pythagoras: 
        ///   sqrt(1 - 1/2 * 1/2)
        ///   sqrt(4/4 - 1/4)
        ///   sqrt(3/4)
        ///   sqrt(3)/2-0
        /// </summary>
        public static readonly float SideDistance = Mathf.Sqrt(3.0f) / 2.0f;

        public static readonly float UnitWidth = 2.0f * CornerDistance;
        public static readonly float UnitHeight = 2.0f * SideDistance;
        
        public static Vector2 HexToUnit(AxialCoordinate coordinates)
        {
            // Q: Top Left to Bottom Right
            // R: Top to Bottom
            var x = coordinates.Q * 1.5f * SideLength;
            var y = coordinates.Q * SideDistance + coordinates.R * UnitHeight;

            // Note that this conversion defines the conversion matrix used in the fractional
            // back conversion further down.

            return new Vector2(x, y);
        }

        public static AxialCoordinate UnitToHex(Vector2 position)
        {
            float centerToCenterDistance = 1.5f * CornerDistance;

            // Q: Top Left to Bottom Right
            // R: Top to Bottom
            var q =  position.X / centerToCenterDistance;
            var r = -position.X / 3.0f + position.Y / UnitHeight;

            // There is no direct relation of the third here to any of our other values.
            // This is a direct result of inverting the above conversion matrix. We divide
            // by the square root of three, which eliminates it from the X term and then we
            // elmininate the 2.0f in the divider and counter and are left with the third.
            //
            // Don't try to translate this to a visual value on the hex.
            //
            // NOTE (Marcel): There is a bug on the redblob pages where the inverted matrix still
            //                contains a square root in the x-term, which is wrong.

            return HexRound(q, r);
        }

        private static AxialCoordinate HexRound(float q, float r)
        {
            // Q: Left to right
            // R: Bottom left to top right
            Vector3 vector = new (q, -q - r, r);
            Vector3 rounded = vector.Round();
            Vector3 delta = (rounded - vector).Abs();

            // Find the component with the largest offset and reset it to what is needed by the other components.
            // This is necessary because for cube coordinates, x + y + z must be fullfilled.

            int rx = (int) rounded.X, ry = (int) rounded.Y, rz = (int) rounded.Z;

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

            return new CubeCoordinate(rx, ry, rz).ToAxialCoordinate();
        }
    }
}