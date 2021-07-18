using System;
using TribesOfDust.Hex;

namespace TribesOfDust.Utils
{
    public class TileTypeNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new <see cref="TileTypeNotFoundException"/>.
        /// </summary>
        ///
        /// <param name="type">The type that couldn't be found.</param>
        public TileTypeNotFoundException(TileType type)
            : base($"TileType: {type} not found.")
        {
            Type = type;
        }

        /// <summary>
        /// Initializes a new <see cref="TileTypeNotFoundException"/>.
        /// </summary>
        ///
        /// <param name="type">The type that couldn't be found.</param>
        /// <param name="message">A message to further explain the exception.</param>
        public TileTypeNotFoundException(TileType type, string message)
            : base($"{message.TrimEnd('.')}. TileType: {type} not found.")
        {
            Type = type;
        }

        public readonly TileType Type;
    }
}