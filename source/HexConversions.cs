using Godot;

public static class HexConversions
{
    public static Vector2 HexToPixel(HexTile.HexIndex index)
    {
        var x = 3.0f / 2.0f * index.Column;
        var y = Mathf.Sqrt(1 - 0.25f) * index.Column + index.Row * (2.0f * Mathf.Sqrt(1 - 0.25f));
        return new Vector2(x, y);
    }
}