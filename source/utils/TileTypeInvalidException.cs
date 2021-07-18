using System;
using TribesOfDust.Hex;

namespace TribesOfDust.Utils
{
    public class TileTypeInvalidException : Exception
    {
        /// <summary>
        /// Initializes a new <see cref="TileTypeInvalidException"/>.
        /// </summary>
        ///
        /// <param name="type">The tile type that was invalid in its' context.</param>
        public TileTypeInvalidException(TileType type)
            : base($"TileType: {type} is invalid.")
        {
            Type = type;
        }

        /// <summary>
        /// Initializes a new <see cref="TileTypeInvalidException"/>.
        /// </summary>
        ///
        /// <param name="type">The tile type that was invalid in its' context.</param>
        /// <param name="message">A message to further explain the exception.</param>
        public TileTypeInvalidException(TileType type, string message)
            : base($"{message.TrimEnd('.')}. TileType: {type} is invalid.")
        {
            Type = type;
        }

        public readonly TileType Type;
    }
}