namespace TribesOfDust.Hex
{
    /// <summary>
    /// Tile effects add a special effect to a tile.
    /// </summary>
    /// <example>
    /// A ruin allows units to fortify which provides them with a defense bonus.
    /// A ruin can spawn on any tile independent of the tile type.
    /// </example>
    public enum TileEffect
    {
        Ruin,
        Fountain,
        Start,
        Sandstorm,
    }
}