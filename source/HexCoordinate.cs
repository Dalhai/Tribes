using Godot;
/// <summary>
/// Cube coordinates based index.
/// </summary>
/// <param name="X">Left to right</param>
/// <param name="Y">Bottom right to top left</param>
/// <param name="Z">Top right to bottom left</param>
public record CubeCoordinate<T>(T X, T Y, T Z);

/// <summary>
/// Axial coordinates based index.
/// </summary>
/// <param name="Q">Left to right</param>
/// <param name="R">Top right to bottom left</param>
public record AxialCoordinate<T>(T Q, T R);


public static class HexCoordinate
{
    public static CubeCoordinate<int> ToCubeCoordinate(this AxialCoordinate<int> axialCoordinate)
    {
        var (q, r) = axialCoordinate;
        return new (q, -(q + r), r);
    }

    public static CubeCoordinate<float> ToCubeCoordinate(this AxialCoordinate<float> axialCoordinate)
    {
        var (q, r) = axialCoordinate;
        return new (q, -(q + r), r);
    }
    
    public static AxialCoordinate<int> ToAxialCoordinate(this CubeCoordinate<int> cubeCoordinate)
    {
        var (x, _, z) = cubeCoordinate;
        return new(x, z);
    }
    
    public static AxialCoordinate<float> ToAxialCoordinate(this CubeCoordinate<float> cubeCoordinate)
    {
        var (x, _, z) = cubeCoordinate;
        return new(x, z);
    }

    public static CubeCoordinate<int> Round(this CubeCoordinate<float> cubeCoordinate)
    {
        var rx = (int) Mathf.Round(cubeCoordinate.X);
        var ry = (int) Mathf.Round(cubeCoordinate.Y);
        var rz = (int) Mathf.Round(cubeCoordinate.Z);
        return new(rx, ry, rz);
    }

    public static AxialCoordinate<int> Round(this AxialCoordinate<float> axialCoordinate)
    {
        var rq = (int) Mathf.Round(axialCoordinate.Q);
        var rr = (int) Mathf.Round(axialCoordinate.R);
        return new(rq, rr);
    }
}