using System;

namespace TribesOfDust.Hex
{
    /// <summary>
    /// A <see cref="TileDirection"/> describes the six possible directions along
    /// the edges of a <see cref="HexTile"/>.
    /// </summary>
    [Flags]
    public enum TileDirection
    {
        NW = 0x01,
        N  = 0x02,
        NE = 0x04,
        SE = 0x08,
        S  = 0x10,
        SW = 0x20,

        /// <summary>
        /// A special direction used to signal all directions.
        /// </summary>
        All  = NW & N & NE & SE & S & SW,

        /// <summary>
        /// A special direction used to signal no direction.
        /// </summary>
        None = 0,
    }

    public static class TileDirectionOffset
    {
        /// <summary>
        /// Gets a <see cref="CubeCoordinate"/> offset for a <see cref="TileDirection"/>.
        /// </summary>
        /// <remarks>
        /// The main purpose of this helper function is to provide an easy way to find neighbouring
        /// tile coordinates based on an existing coordinate. Using this helper ensures you don't have
        /// to worry about translating tile directions to XYZ cuboid offsets yourself.
        /// </remarks>
        ///
        /// <example>
        /// var coordinate = CubeCoordinate.FromQR(0, 0);
        /// var offset = TileDirectionOffset.GetCubeOffset(TileDirection.N);
        ///
        /// /* Compute the coordinates of the tile just above our tile. */
        /// var coordinateUpper = coordinate + offset;
        /// </example>
        ///
        /// <param name="direction">The direction for which to get the offset.</param>
        /// <returns>The offset.</returns>
        public static CubeCoordinate GetCubeOffset(TileDirection direction) => GetAxialOffset(direction).ToCubeCoordinate();

        /// <summary>
        /// Gets a <see cref="AxialCoordinate"/> offset for a <see cref="TileDirection"/>.
        /// </summary>
        /// <remarks>
        /// The main purpose of this helper function is to provide an easy way to find neighbouring
        /// tile coordinates based on an existing coordinate. Using this helper ensures you don't have
        /// to worry about translating tile directions to QR axial offsets yourself.
        /// </remarks>
        ///
        /// <example>
        /// var coordinate = new AxialCoordinate(0, 0);
        /// var offset = TileDirectionOffset.GetAxialOffset(TileDirection.N);
        ///
        /// /* Compute the coordinates of the tile just above our tile. */
        /// var coordinateUpper = coordinate + offset;
        /// </example>
        ///
        /// <param name="direction">The direction for which to get the offset.</param>
        /// <returns>The offset.</returns>
        public static AxialCoordinate GetAxialOffset(TileDirection direction) => direction switch
        {
            TileDirection.NW => new (-1,  0),
            TileDirection.N  => new ( 0, -1),
            TileDirection.NE => new ( 1, -1),
            TileDirection.SE => new ( 1,  0),
            TileDirection.S  => new ( 0,  1),
            TileDirection.SW => new ( -1, 1),

            // Catch-all case for unsupported tile directions.
            // Should ideally be caught by the compiler.
            _ => throw new NotImplementedException($"Offset calculation not implemented for {direction}.")
        };
    }
}