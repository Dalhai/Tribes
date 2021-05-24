using Godot;
using System;

public class Camera2D : Godot.Node2D
{
    public override void _Process(float delta)
    {
        if (Input.IsKeyPressed((int) KeyList.Up))
        {
            Position = Position.MoveToward(Position + Vector2.Up, delta * 10000.0f);
        }

        if (Input.IsKeyPressed((int) KeyList.Down))
        {
            Position = Position.MoveToward(Position + Vector2.Down, delta * 10000.0f);
        }

        if (Input.IsKeyPressed((int) KeyList.Left))
        {
            Position = Position.MoveToward(Position + Vector2.Left, delta * 10000.0f);
        }

        if (Input.IsKeyPressed((int) KeyList.Right))
        {
            Position = Position.MoveToward(Position + Vector2.Right, delta * 10000.0f);
        }
    }
}