using System;
using TribesOfDust.Core.Input;

using GDIn = Godot.Input;
using Godot;

namespace TribesOfDust.Core
{
    public partial class Camera: Camera2D
    {
        public const string InputActionZoomIn = "zoom_in";
        public const string InputActionZoomOut = "zoom_out";
        public const float ZoomChange = 50f;
        public const float ZoomMax = 1.0f;
        public const float OffsetLengthWeight = 0.15f;
        public const float OffsetSpeed = 3000.0f;
        public const float MovementSpeed = 10000.0f;

        public override void _Process(double delta)
        {
            if (GDIn.IsActionPressed(Actions.CameraUp))
            {
                Position = Position.MoveToward(Position + Vector2.Up, (float)delta * MovementSpeed);
            }

            if (GDIn.IsActionPressed(Actions.CameraDown))
            {
                Position = Position.MoveToward(Position + Vector2.Down, (float)delta * MovementSpeed);
            }

            if (GDIn.IsActionPressed(Actions.CameraLeft))
            {
                Position = Position.MoveToward(Position + Vector2.Left, (float)delta * MovementSpeed);
            }

            if (GDIn.IsActionPressed(Actions.CameraRight))
            {
                Position = Position.MoveToward(Position + Vector2.Right, (float)delta * MovementSpeed);
            }

            if (GDIn.IsActionJustPressed(Actions.ZoomIn))
            {
                if (Zoom is { X: > ZoomMax, Y: > ZoomMax })
                {
                    Vector2 oldZoom = Zoom;
                    Vector2 newZoom = oldZoom * (1.0f - ZoomChange * (float)delta);
                    
                    var viewportRect = GetViewport().GetVisibleRect();

                    Vector2 offset = viewportRect.Size - GetLocalMousePosition();
                    Vector2 offsetNormalized = offset.Normalized();

                    float maxOffsetWeight = offset.Length() * OffsetLengthWeight;
                    float currentOffsetWeight = OffsetSpeed * (float)delta * (float)Math.Pow(newZoom.X, 2f);
                    float offsetWeight = Math.Min(maxOffsetWeight, currentOffsetWeight);

                    Position = Position + 0.5f * viewportRect.Size * (oldZoom - newZoom) + offsetNormalized * offsetWeight;
                    Zoom = newZoom;
                }
            }


            if (GDIn.IsActionJustPressed(Actions.ZoomOut))
            {
                Vector2 oldZoom = Zoom;
                Vector2 newZoom = oldZoom * (1.0f + ZoomChange * (float)delta);

                Viewport viewport = GetViewport();
                Position = Position + 0.5f * viewport.GetVisibleRect().Size * (oldZoom - newZoom);
                Zoom = newZoom;
            }

        }
    }
}