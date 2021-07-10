using Godot;
using Godot.Collections;
using TribesOfDust.Hex;

namespace TribesOfDust.Utils
{
    public static class Json
    {
        /// <summary>
        /// Serializes a <see cref="Vector2"/> into a JSON dictionary.
        /// </summary>
        /// <param name="vector">The vector to serialize.</param>
        /// <returns>The serialized vector.</returns>
        public static Dictionary<string, object> Serialize(this Vector2 vector)
        {
            return new()
            {
                {nameof(Vector2.x).ToLower(), vector.x},
                {nameof(Vector2.y).ToLower(), vector.y}
            };
        }

        /// <summary>
        /// Deserializes a JSON dictionary into a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="json">The JSON dictionary.</param>
        /// <param name="output">The deserialized output vector.</param>
        /// <returns>True, if the deserialization succeeded, false otherwise.</returns>
        public static bool TryDeserialize(Dictionary<string, object> json, out Vector2 output)
        {
            string keyX = nameof(Vector2.x).ToLower();
            string keyY = nameof(Vector2.y).ToLower();

            if (!json.ContainsKey(keyX) || !json.ContainsKey(keyY))
            {
                output = Vector2.Zero;
                return false;
            }

            float x = (float) json[keyX];
            float y = (float) json[keyY];

            output = new(x, y);
            return true;
        }

        /// <summary>
        /// Serializes a <see cref="Vector3"/> into a JSON dictionary.
        /// </summary>
        /// <param name="vector">The vector to serialize.</param>
        /// <returns>The serialized vector.</returns>
        public static Dictionary<string, object> Serialize(this Vector3 vector)
        {
            return new()
            {
                {nameof(Vector3.x).ToLower(), vector.x},
                {nameof(Vector3.y).ToLower(), vector.y},
                {nameof(Vector3.z).ToLower(), vector.z}
            };
        }

        /// <summary>
        /// Deserializes a JSON dictionary into a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="json">The JSON dictionary.</param>
        /// <param name="output">The deserialized output vector.</param>
        /// <returns>True, if the deserialization succeeded, false otherwise.</returns>
        public static bool TryDeserialize(Dictionary<string, object> json, out Vector3 output)
        {
            string keyX = nameof(Vector3.x).ToLower();
            string keyY = nameof(Vector3.y).ToLower();
            string keyZ = nameof(Vector3.z).ToLower();

            if (!json.ContainsKey(keyX) || !json.ContainsKey(keyY) || !json.ContainsKey(keyZ))
            {
                output = Vector3.Zero;
                return false;
            }

            float x = (float) json[keyX];
            float y = (float) json[keyY];
            float z = (float) json[keyZ];

            output = new(x, y, z);
            return true;
        }

        /// <summary>
        /// Serializes a <see cref="AxialCoordinate"/> into a JSON dictionary.
        /// </summary>
        /// <param name="coordinates">The coordinates to serialize.</param>
        /// <returns>The serialized coordinates.</returns>
        public static Dictionary<string, object> Serialize(this AxialCoordinate<int> coordinates)
        {
            return new()
            {
                {nameof(AxialCoordinate<int>.R).ToLower(), coordinates.R},
                {nameof(AxialCoordinate<int>.Q).ToLower(), coordinates.Q},
            };
        }

        /// <summary>
        /// Deserializes a JSON dictionary into a <see cref="AxialCoordinate"/>.
        /// </summary>
        /// <param name="json">The JSON dictionary.</param>
        /// <param name="output">The deserialized output coordinate.</param>
        /// <returns>True, if the deserialization succeeded, false otherwise.</returns>
        public static bool TryDeserialize(Dictionary<string, object> json, out AxialCoordinate<int> output)
        {
            string keyQ = nameof(AxialCoordinate<int>.R).ToLower();
            string keyR = nameof(AxialCoordinate<int>.Q).ToLower();

            if (!json.ContainsKey(keyQ) || !json.ContainsKey(keyR))
            {
                output = new(0, 0);
                return false;
            }

            int q = (int) json[keyQ];
            int r = (int) json[keyR];

            output = new(q, r);
            return true;
        }

        /// <summary>
        /// Serializes a <see cref="CubeCoordinate"/> into a JSON dictionary.
        /// </summary>
        /// <param name="coordinates">The coordinates to serialize.</param>
        /// <returns>The serialized coordinates.</returns>
        public static Dictionary<string, object> Serialize(this CubeCoordinate<int> coordinates)
        {
            return new()
            {
                {nameof(CubeCoordinate<int>.X).ToLower(), coordinates.X},
                {nameof(CubeCoordinate<int>.Y).ToLower(), coordinates.Y},
                {nameof(CubeCoordinate<int>.Z).ToLower(), coordinates.Z}
            };
        }

        /// <summary>
        /// Deserializes a JSON dictionary into a <see cref="CubeCoordinate"/>.
        /// </summary>
        /// <param name="json">The JSON dictionary.</param>
        /// <param name="output">The deserialized output coordinate.</param>
        /// <returns>True, if the deserialization succeeded, false otherwise.</returns>
        public static bool TryDeserialize(Dictionary<string, object> json, out CubeCoordinate<int> output)
        {
            string keyX = nameof(CubeCoordinate<int>.X).ToLower();
            string keyY = nameof(CubeCoordinate<int>.X).ToLower();
            string keyZ = nameof(CubeCoordinate<int>.X).ToLower();

            if (!json.ContainsKey(keyX) || !json.ContainsKey(keyY) || !json.ContainsKey(keyZ))
            {
                output = new(0, 0, 0);
                return false;
            }

            int x = (int) json[keyX];
            int y = (int) json[keyY];
            int z = (int) json[keyZ];

            output = new(x, y, z);
            return true;
        }
    }
}