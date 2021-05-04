using Godot;
using System;

public class GameManager : Node2D
{
    public override void _Ready()
    {
        Vector2 offSet = new Vector2(500, 500);
        Texture texture = GD.Load<Texture>("res://assets/textures/tile_mountain_1.png");

        for (int row = 0; row < 10; ++row)
        {
            for (int col = 0; col < 10; ++col)
            {
                float width = texture.GetWidth();
                float height = texture.GetHeight();

                float x = col * 3.0f / 4.0f * width;
                float y = col % 2 == 0 ? row * height : height * ((row - 1) + 1.0f / 2.0f);
                Vector2 position = new Vector2(x, y);

                var tile = new HexTile
                {
                    Texture = texture,
                    Position = offSet + position
                };

                AddChild(tile);
            }
        }

    }
}
