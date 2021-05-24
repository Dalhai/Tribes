using System;
using Godot;

public static class HexConversions
{
    public static Vector2 HexToPixel(HexCoordinate.AxialCoordinate coordinates)
    {
        var x = 3.0f / 2.0f * coordinates.X;
        var y = Mathf.Sqrt(1 - 0.25f) * coordinates.X + coordinates.Z * (2.0f * Mathf.Sqrt(1 - 0.25f));
        return new Vector2(x, y);
    }

    public static HexCoordinate.AxialCoordinate WorldToHex(Vector2 position)
    {
        throw new NotImplementedException();
    }
}