using System;

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
        public static Dictionary Serialize(this Vector2 vector)
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
        public static bool TryDeserialize(Dictionary json, out Vector2 output)
        {
            output = Vector2.Zero;

            string keyX = nameof(Vector2.x).ToLower();
            string keyY = nameof(Vector2.y).ToLower();

            if (!json.Contains(keyX) || !json.Contains(keyY))
            {
                return false;
            }

            try
            {
                float x = Convert.ToSingle(json[keyX]);
                float y = Convert.ToSingle(json[keyY]);

                output = new(x, y);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Serializes a <see cref="Vector3"/> into a JSON dictionary.
        /// </summary>
        /// <param name="vector">The vector to serialize.</param>
        /// <returns>The serialized vector.</returns>
        public static Dictionary Serialize(this Vector3 vector)
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
        public static bool TryDeserialize(Dictionary json, out Vector3 output)
        {
            output = Vector3.Zero;

            string keyX = nameof(Vector3.x).ToLower();
            string keyY = nameof(Vector3.y).ToLower();
            string keyZ = nameof(Vector3.z).ToLower();

            if (!json.Contains(keyX) || !json.Contains(keyY) || !json.Contains(keyZ))
            {
                return false;
            }

            try
            {
                float x = Convert.ToSingle(json[keyX]);
                float y = Convert.ToSingle(json[keyY]);
                float z = Convert.ToSingle(json[keyZ]);

                output = new(x, y, z);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Serializes a <see cref="AxialCoordinate"/> into a JSON dictionary.
        /// </summary>
        /// <param name="coordinates">The coordinates to serialize.</param>
        /// <returns>The serialized coordinates.</returns>
        public static Dictionary Serialize(this AxialCoordinate<int> coordinates)
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
        public static bool TryDeserialize(Dictionary json, out AxialCoordinate<int> output)
        {
            output = new(0, 0);

            string keyQ = nameof(AxialCoordinate<int>.Q).ToLower();
            string keyR = nameof(AxialCoordinate<int>.R).ToLower();

            if (!json.Contains(keyQ) || !json.Contains(keyR))
            {
                return false;
            }

            try
            {
                int q = Convert.ToInt32(json[keyQ]);
                int r = Convert.ToInt32(json[keyR]);

                output = new(q, r);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Serializes a <see cref="CubeCoordinate"/> into a JSON dictionary.
        /// </summary>
        /// <param name="coordinates">The coordinates to serialize.</param>
        /// <returns>The serialized coordinates.</returns>
        public static Dictionary Serialize(this CubeCoordinate<int> coordinates)
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
        public static bool TryDeserialize(Dictionary json, out CubeCoordinate<int> output)
        {
            output = new(0, 0, 0);

            string keyX = nameof(CubeCoordinate<int>.X).ToLower();
            string keyY = nameof(CubeCoordinate<int>.X).ToLower();
            string keyZ = nameof(CubeCoordinate<int>.X).ToLower();

            if (!json.Contains(keyX) || !json.Contains(keyY) || !json.Contains(keyZ))
            {
                return false;
            }

            try
            {
                int x = Convert.ToInt32(json[keyX]);
                int y = Convert.ToInt32(json[keyY]);
                int z = Convert.ToInt32(json[keyZ]);

                output = new(x, y, z);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}