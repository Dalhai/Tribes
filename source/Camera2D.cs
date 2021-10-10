using System;
using Godot;

namespace TribesOfDust
{
    public class Camera2D : Godot.Camera2D
    {
        public override void _Process(float delta)
        {
            if (Input.IsKeyPressed((int)KeyList.Up))
            {
                Position = Position.MoveToward(Position + Vector2.Up, delta * 10000.0f);
            }

            if (Input.IsKeyPressed((int)KeyList.Down))
            {
                Position = Position.MoveToward(Position + Vector2.Down, delta * 10000.0f);
            }

            if (Input.IsKeyPressed((int)KeyList.Left))
            {
                Position = Position.MoveToward(Position + Vector2.Left, delta * 10000.0f);
            }

            if (Input.IsKeyPressed((int)KeyList.Right))
            {
                Position = Position.MoveToward(Position + Vector2.Right, delta * 10000.0f);
            }

            if(Input.IsActionJustReleased("zoom_in"))
            {
                Vector2 oldZoom = Zoom;
                Vector2 newZoom = oldZoom * 0.9f;;
                Viewport viewport = GetViewport();
                Position = Position + 0.5f*viewport.Size*(oldZoom-newZoom);
                Zoom = newZoom;
            }


            if(Input.IsActionJustReleased("zoom_out"))
            {
                Vector2 oldZoom = Zoom;
                Vector2 newZoom = oldZoom * 1.1f;;
                Viewport viewport = GetViewport();
                Position = Position + 0.5f*viewport.Size*(oldZoom-newZoom);
                Zoom = newZoom;
            }
            
        }
    }
}