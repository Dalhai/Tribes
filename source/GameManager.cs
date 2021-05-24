using Godot;
using System;

public class GameManager : Node2D
{
    public override void _Ready()
    {
        Texture texture = GD.Load<Texture>("res://assets/textures/tile_mountain_1.png");
        float size = texture.GetWidth() / 2.0f;

        for (int x = 0; x < 10; ++x)
        {
            for (int z = 0; z < 10; ++z)
            {
                var axialCoordinates = new HexCoordinate.AxialCoordinate(x, z);
                var position = HexConversions.HexToPixel(axialCoordinates);
                position.x *= size;
                position.y *= size;

                var tile = new HexTile
                {
                    Texture = texture,
                    Centered = true,
                    Coordinates = axialCoordinates,
                    Position = position
                };

                AddChild(tile);
            }
        }
    }

    public override void _Input(InputEvent InputEvent)
    {
        
    }
}