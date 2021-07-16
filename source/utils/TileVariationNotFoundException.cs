using System;
using TribesOfDust.Hex;

namespace TribesOfDust.Utils
{
    public class TileVariationNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new <see cref="TileVariationNotFoundException"/>.
        /// </summary>
        ///
        /// <param name="type">The tile type for which the variation couldn't be found.</param>
        /// <param name="variation">The tile variation index that couldn't be found.</param>
        public TileVariationNotFoundException(TileType type, int variation)
            : base($"TileType: {type}, Variation: {variation} not found.") 
        {
            Type = type;
            Variation = variation;
        }

        /// <summary>
        /// Initializes a new <see cref="TileVariationNotFoundException"/>.
        /// </summary>
        ///
        /// <param name="type">The tile type for which the variation couldn't be found.</param>
        /// <param name="variation">The tile variation index that couldn't be found.</param>
        /// <param name="message">A message further explaining the exception.</param>
        public TileVariationNotFoundException(TileType type, int variation, string message)
            : base($"{message.TrimEnd('.')}. TileType: {type}, Variation: {variation} not found.")
        {
            Type = type;
            Variation = variation;
        }

        public readonly TileType Type;
        public readonly int Variation;
    }
}