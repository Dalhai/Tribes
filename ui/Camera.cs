using Godot;
using System;

public partial class Camera : Camera2D
{
    #region Exports

    [Export(PropertyHint.Range, "0, 1, 0.00f")]
    public float ZoomMax = 10.0f;

    [Export(PropertyHint.Range, "10.0, 1000.0, 1.0")]
    public float ZoomSpeed = 100.0f;

    [Export(PropertyHint.Range, "100.0, 100000.0, 10.0")]
    public float MovementSpeed = 1000.0f;

    #endregion

    #region Lifecycle

    public override void _Ready()
    {
        AnchorMode = AnchorModeEnum.FixedTopLeft;
        Zoom = new(
            Math.Min(ZoomMax, Zoom.X),
            Math.Min(ZoomMax, Zoom.Y)
        );
    }

    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.W))
            Position.MoveToward(Position + Vector2.Up, (float)delta * MovementSpeed / Zoom.Y);
        if (Input.IsKeyPressed(Key.S))
            Position.MoveToward(Position + Vector2.Down, (float)delta * MovementSpeed / Zoom.Y);
        if (Input.IsKeyPressed(Key.A))
            Position.MoveToward(Position + Vector2.Right, (float)delta * MovementSpeed / Zoom.X);
        if (Input.IsKeyPressed(Key.D))
            Position.MoveToward(Position + Vector2.Left, (float)delta * MovementSpeed / Zoom.X);

        ForceUpdateScroll();

        var screenCenter = GetScreenCenterPosition();
        var mousePosition = GetGlobalMousePosition();

        if (Input.IsMouseButtonPressed(MouseButton.WheelDown))
            ZoomStable(screenCenter, -ZoomSpeed * (float)delta);
        if (Input.IsMouseButtonPressed(MouseButton.WheelUp))
            ZoomStable(mousePosition, ZoomSpeed * (float)delta);
    }
    
    #endregion

    #region Zooming

    private void ZoomStable(Vector2 anchor, float amount)
    {
        var oldZoom = Zoom;
        var newZoom = oldZoom * (1.0f - amount);

        newZoom.X = Math.Clamp(newZoom.X, 0, ZoomMax);
        newZoom.Y = Math.Clamp(newZoom.Y, 0, ZoomMax);

        var canvasAnchor = GetCanvasTransform() * anchor;
        var windowAnchor = canvasAnchor / GetViewportRect().Size;
        Zoom = newZoom;

        canvasAnchor = windowAnchor * GetViewportRect().Size;
        var target = GetCanvasTransform().AffineInverse() * canvasAnchor;
        var targetOffset = target - anchor;

        Position -= targetOffset;
        ForceUpdateScroll();
    }

    #endregion
}