public static class HexCoordinate
{
    /// <summary>
    /// Cube coordinates based index.
    /// </summary>
    /// <param name="X">Left to right</param>
    /// <param name="Y">Bottom right to top left</param>
    /// <param name="Z">Top right to bottom left</param>
    public record CubeCoordinate(int X, int Y, int Z);

    /// <summary>
    /// Axial coordinates based index.
    /// </summary>
    /// <param name="X">Left to right</param>
    /// <param name="Z">Top right to bottom left</param>
    public record AxialCoordinate(int X, int Z);

    public static CubeCoordinate AxialToCube(this AxialCoordinate axialCoordinate)
    {
        var (x, z) = axialCoordinate;
        return new CubeCoordinate(x, -(x + z), z);
    }
}