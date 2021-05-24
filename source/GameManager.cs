using Godot;
using System;

public class GameManager : Node2D
{
	public override void _Ready()
	{
		Vector2 offSet = new Vector2(500, 500);
		Texture texture = GD.Load<Texture>("res://assets/textures/tile_mountain_1.png");
		float width = texture.GetWidth();
		float height = texture.GetHeight();
		
		GD.Print(HexConversions.HexToPixel(new HexTile.HexIndex(0,0)));
		GD.Print(HexConversions.HexToPixel(new HexTile.HexIndex(1,0)));
		GD.Print(HexConversions.HexToPixel(new HexTile.HexIndex(0,1)));
		GD.Print(height/2.0f);
		GD.Print(Mathf.Sqrt(1 - 0.25f)*width/2.0f);hiz

		
		for (int row = 0; row < 10; ++row)
		{
			for (int column = 0; column < 10; ++column)
			{

				// float x = column * 3.0f / 4.0f * width;
				// float y = column % 2 == 0 ? row * height : height * ((row - 1) + 1.0f / 2.0f);
				// Vector2 position = new Vector2(x, y);
				
				var index = new HexTile.HexIndex(row, column);
				var position = HexConversions.HexToPixel(index);
				position.x *= width/2.0f;
				position.y *= width/2.0f;
				
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
	public override void _Input(InputEvent @event)
	{
		// Mouse in viewport coordinates.
		if (@event is InputEventMouseButton eventMouseButton)
		{
			Vector2 mouseLocation = eventMouseButton.Position;
			Vector2 cameraLocation = GetNode<Camera2D>("CameraRoot").Position;
			Vector2 mouseWorldLocation = mouseLocation + cameraLocation;


		}
	}
}
