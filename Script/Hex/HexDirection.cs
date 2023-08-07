using System;

namespace TribesOfDust.Hex;

/// <summary>
/// A <see cref="HexDirection"/> describes the six possible directions along
/// the edges of a <see cref="Tile"/>.
/// </summary>
[Flags]
public enum HexDirection
{
    NW = 0x01,
    N  = 0x02,
    NE = 0x04,
    SE = 0x08,
    S  = 0x10,
    SW = 0x20,

    /// <summary>
    /// A special tile direction for when no direction is needed.
    /// </summary>
    None = 0
}

public static class HexDirections
{
    public const HexDirection Northern = HexDirection.NW | HexDirection.N | HexDirection.NE;
    public const HexDirection Southern = HexDirection.SW | HexDirection.S | HexDirection.SE;
    public const HexDirection All = Northern | Southern;

    public static HexDirection FromOffset(AxialCoordinate offset) => offset switch 
    {
        (-1,  0) => HexDirection.NW,
        ( 0, -1) => HexDirection.N,
        ( 1, -1) => HexDirection.NE,
        ( 1,  0) => HexDirection.SE,
        ( 0,  1) => HexDirection.S,
        ( -1, 1) => HexDirection.SW,
        _        => HexDirection.None
    };
}

public static class HexDirectionOffset
{
    /// <summary>
    /// Gets a <see cref="CubeCoordinate"/> offset for a <see cref="HexDirection"/>.
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
    public static CubeCoordinate GetCubeOffset(HexDirection direction) => GetAxialOffset(direction).ToCubeCoordinate();

    /// <summary>
    /// Gets a <see cref="AxialCoordinate"/> offset for a <see cref="HexDirection"/>.
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
    public static AxialCoordinate GetAxialOffset(HexDirection direction) => direction switch
    {
        HexDirection.NW => new (-1,  0),
        HexDirection.N  => new ( 0, -1),
        HexDirection.NE => new ( 1, -1),
        HexDirection.SE => new ( 1,  0),
        HexDirection.S  => new ( 0,  1),
        HexDirection.SW => new ( -1, 1),

        // Catch-all case for unsupported tile directions.
        // Should ideally be caught by the compiler.
        HexDirection other=> throw Error.NotImplementedFor<HexDirection>(other)
    };
}