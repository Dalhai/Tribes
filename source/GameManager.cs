using Godot;
using System;

public class GameManager : Node2D
{
    public float Size;

    public override void _Ready()
    {
        Texture texture = GD.Load<Texture>("res://assets/textures/tile_mountain_1.png");
        Size = texture.GetWidth() / 2.0f;

        for (int x = 0; x < 10; ++x)
        {
            for (int z = 0; z < 10; ++z)
            {
                var axialCoordinates = new AxialCoordinate<int>(x, z);
                var position = HexConversions.HexToPixel(axialCoordinates);
                position.x *= Size;
                position.y *= Size;
                var converted = HexConversions.WorldToHex(position, Size);
                if (converted != axialCoordinates)
                {
                    System.Diagnostics.Trace.WriteLine($"X {x} Z {z}");
                    System.Diagnostics.Trace.WriteLine($"original {axialCoordinates} converted {converted}");
                }
                position.x -= Size;
                position.y -= Mathf.Sqrt(3.0f) * Size;
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

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton eventMouseButton)
        {
            Vector2 worldSpacePosition = eventMouseButton.Position + GetNode<Camera2D>("CameraRoot").Position;
            System.Diagnostics.Trace.WriteLine($"Position {worldSpacePosition}" );
            
            System.Diagnostics.Trace.WriteLine(HexConversions.WorldToHex(worldSpacePosition, Size));
        }
    }
}