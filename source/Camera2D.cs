using System;
using Godot;

namespace TribesOfDust
{
    public class Camera2D : Godot.Camera2D
    {
        public const string InputActionZoomIn = "zoom_in";
        public const string InputActionZoomOut = "zoom_out";
        public const float ZoomChange = 30f;
        public const float ZoomMax = 1.0f;
        public const float OffsetLengthWeight = 0.15f;
        public const float OffsetSpeed = 3000.0f;
        public const float MovementSpeed = 10000.0f;


        public override void _Process(float delta)
        {
            if (Input.IsKeyPressed((int)KeyList.Up))
            {
                Position = Position.MoveToward(Position + Vector2.Up, delta * MovementSpeed);
            }

            if (Input.IsKeyPressed((int)KeyList.Down))
            {
                Position = Position.MoveToward(Position + Vector2.Down, delta * MovementSpeed);
            }

            if (Input.IsKeyPressed((int)KeyList.Left))
            {
                Position = Position.MoveToward(Position + Vector2.Left, delta * MovementSpeed);
            }

            if (Input.IsKeyPressed((int)KeyList.Right))
            {
                Position = Position.MoveToward(Position + Vector2.Right, delta * MovementSpeed);
            }

            if (Input.IsActionJustReleased(InputActionZoomIn))
            {
                if (Zoom.x > ZoomMax && Zoom.y > ZoomMax)
                {
                    Vector2 oldZoom = Zoom;
                    Vector2 newZoom = oldZoom * (1.0f - ZoomChange * delta);

                    Vector2 offset = GetGlobalMousePosition() - GetCameraScreenCenter();
                    Vector2 offsetNormalized = offset.Normalized();

                    float maxOffsetWeight = offset.Length() * OffsetLengthWeight;
                    float currentOffsetWeight = OffsetSpeed * delta * (float)Math.Pow(newZoom.x, 2f);
                    float offsetWeight = (float)Math.Min(maxOffsetWeight, currentOffsetWeight);

                    Viewport viewport = GetViewport();
                    Position = Position + 0.5f * viewport.Size * (oldZoom - newZoom) + offsetNormalized * offsetWeight;
                    Zoom = newZoom;
                }
            }


            if (Input.IsActionJustReleased(InputActionZoomOut))
            {
                Vector2 oldZoom = Zoom;
                Vector2 newZoom = oldZoom * (1.0f + ZoomChange * delta);

                Viewport viewport = GetViewport();
                Position = Position + 0.5f * viewport.Size * (oldZoom - newZoom);
                Zoom = newZoom;
            }

        }
    }
}