using System;
using Godot;

public static class HexConversions
{
    public static Vector2 HexToPixel(HexTile.HexIndex index)
    {
        var x = 3.0f / 2.0f * index.X;
        var y = Mathf.Sqrt(1 - 0.25f) * index.X + index.Z * (2.0f * Mathf.Sqrt(1 - 0.25f));
        return new Vector2(x, y);
    }

    public static HexTile.HexIndex WorldToHex(Vector2 position)
    {
        throw new NotImplementedException();
    }
}