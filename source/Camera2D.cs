using System;
using Godot;

namespace TribesOfDust
{
    public class Camera2D : Godot.Camera2D
    {
        public const string InputActionZoomIn = "zoom_in";
        public const string InputActionZoomOut = "zoom_out";
        public const float ZoomChange = 0.1f;
        public const float OffsetLengthWeight = 0.15f;
        public const float OffsetSpeed = 50f;

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

            if (Input.IsActionJustReleased(InputActionZoomIn))
            {
                Vector2 oldZoom = Zoom;
                Vector2 newZoom = oldZoom * (1.0f - ZoomChange);

                Vector2 offset = GetGlobalMousePosition() - GetCameraScreenCenter();
                Vector2 offsetNormalized = offset.Normalized();

                float maxOffsetWeight = offset.Length() * OffsetLengthWeight;
                float currentOffsetWeight = OffsetSpeed * (float)Math.Pow(newZoom.x, 2f);
                float offsetWeight = (float)Math.Min(maxOffsetWeight, currentOffsetWeight);

                Viewport viewport = GetViewport();
                Position = Position + 0.5f * viewport.Size * (oldZoom - newZoom) + offsetNormalized * offsetWeight;
                Zoom = newZoom;
            }


            if (Input.IsActionJustReleased(InputActionZoomOut))
            {
                Vector2 oldZoom = Zoom;
                Vector2 newZoom = oldZoom * (1.0f + ZoomChange);

                Viewport viewport = GetViewport();
                Position = Position + 0.5f * viewport.Size * (oldZoom - newZoom);
                Zoom = newZoom;
            }

        }
    }
}