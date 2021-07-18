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
        /// The tile typ used to indicate that the details of a tile are hidden from the system.
        /// Is intended to be used as fog of war or when a tile becomes hidden through a modifier.
        /// </summary>
        Unknown,

        /// <summary>
        /// The default tile type of the board, represent an empty field.
        /// Are replaced with the final tile types in the building phase.
        /// </summary>
        Open,

        /// <summary>
        /// The tile type used to indicate that a tile is not passable or modifiable.
        /// Is intended to be used as map borders and to indicate impassable terrain.
        /// Can not be manipulated by players in any way, other than in the editor.
        /// </summary>
        Blocked,

        Tundra,
        Rocks,
        Dune,
        Canyon
    }
}