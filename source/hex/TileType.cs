namespace TribesOfDust.Hex
{
    /// <summary>
    /// Tile types describe the fundamental properties of a hex tile such as the setting and art.
    /// </summary>
    /// <example>
    /// Rocks include a rock formation as texture as well as properties
    /// that make them harder to cross with certain unit types.
    /// </example>
    public enum TileType
    {
        /// <summary>
        /// The default tile type of the board, represent an empty field.
        /// Are replaced with the final tile types in the building phase.
        /// </summary>
        Open,
        Tundra,
        Rocks,
        Dune,
        Canyon
    }
}