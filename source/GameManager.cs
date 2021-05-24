using Godot;
using System;

public class GameManager : Node2D
{
    public override void _Ready()
    {
        Texture texture = GD.Load<Texture>("res://assets/textures/tile_mountain_1.png");
        float size = texture.GetWidth() / 2.0f;

        for (int row = 0; row < 10; ++row)
        {
            for (int column = 0; column < 10; ++column)
            {
                var index = new HexTile.HexIndex(row, column);
                var position = HexConversions.HexToPixel(index);
                position.x *= size;
                position.y *= size;

                var tile = new HexTile
                {
                    Texture = texture,
                    Centered = true,
                    Index = index,
                    Position = position
                };

                AddChild(tile);
            }
        }
    }
}