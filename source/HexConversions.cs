using System;
using Godot;

public static class HexConversions
{
    public static Vector2 HexToPixel(AxialCoordinate<int> coordinates)
    {
        var x = 3.0f / 2.0f * coordinates.Q;
        var y = Mathf.Sqrt(1 - 0.25f) * coordinates.Q + coordinates.R * (2.0f * Mathf.Sqrt(1 - 0.25f));
        return new Vector2(x, y);
    }

    public static AxialCoordinate<int> WorldToHex(Vector2 position, float size)
    {
        var q = (2.0f / 3.0f * position.x) / size;
        var r = (-1.0f / 3.0f * position.x + Mathf.Sqrt(3.0f) / 3.0f * position.y) / size;
        var axialCoordinate = new AxialCoordinate<float>((int) q, (int) r);
        return HexRound(axialCoordinate.ToCubeCoordinate());
    }

    private static AxialCoordinate<int> HexRound(CubeCoordinate<float> cubeCoordinate)
    {
        var roundedCubeCoordinate = cubeCoordinate.Round();
        var dx = Mathf.Abs(roundedCubeCoordinate.X - cubeCoordinate.X);
        var dy = Mathf.Abs(roundedCubeCoordinate.Y - cubeCoordinate.Y);
        var dz = Mathf.Abs(roundedCubeCoordinate.Z - cubeCoordinate.Z);

        var (rx, ry, rz) = roundedCubeCoordinate;

        if (dx > dy && dx > dz)
        {
            rx = -ry - rz;
        }
        else if (dy <= dz)
        {
            rz = -rx - ry;
        }

        return new (rx, rz);
    }
}