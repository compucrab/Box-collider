using System;
using Box_collider.Helpers;
using Box_collider.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

internal class Camera
{
    public Vector3 Position;

    public Vector3 Forward;
    public Vector3 Right;
    public Vector3 Up;

    public Matrix View;
    public Matrix Projection;

    public float Speed = 5f;

    private float yaw = 0f;
    private float pitch = 0f;

    private float RotationSpeed = 0.003f;

    private GraphicsDevice _graphics;

    private bool orbiting = false;

    public Camera(GraphicsDevice graphics)
    {
        _graphics = graphics;

        Position = new Vector3(0, -8, 5);

        Forward = Vector3.UnitY;
        Right = Vector3.UnitX;
        Up = Vector3.UnitZ;

        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(60),
            graphics.Viewport.AspectRatio,
            0.1f,
            100f
        );

        UpdateView();

        KeyboardManager.RegisterHeld(Keys.Up, () => Move(Forward));
        KeyboardManager.RegisterHeld(Keys.Down, () => Move(-Forward));
        KeyboardManager.RegisterHeld(Keys.Left, () => Move(-Right));
        KeyboardManager.RegisterHeld(Keys.Right, () => Move(Right));

        KeyboardManager.RegisterHeld(Keys.S, () => Move(Forward));
        KeyboardManager.RegisterHeld(Keys.D, () => Move(-Forward));

        KeyboardManager.RegisterHeld(Keys.Space, () => Move(Up));
        KeyboardManager.RegisterHeld(Keys.LeftControl, () => Move(-Up));
    }

    private void Move(Vector3 direction)
    {
        Position += direction * Speed * ShapeData.DeltaTime;
    }

    public void Update()
    {
        bool orbitInput = KeyboardManager.IsHeld(Keys.LeftAlt) && MouseManager.LeftPressed;

        if (orbitInput)
        {
            if (!orbiting)
            {
                orbiting = true;
                MouseManager.ResetPosition(MouseCenter());
            }

            Point delta = MouseManager.Delta;

            yaw -= delta.X * RotationSpeed;
            pitch -= delta.Y * RotationSpeed;
        }
        else
        {
            orbiting = false;
        }

        pitch = MathHelper.Clamp(pitch, MathHelper.ToRadians(-89f), MathHelper.ToRadians(89f));

        UpdateDirection();
        UpdateView();
    }

    private Point MouseCenter()
    {
        return new Point(_graphics.Viewport.Width / 2, _graphics.Viewport.Height / 2);
    }

    private void UpdateDirection()
    {
        Forward = new Vector3(
            MathF.Sin(yaw) * MathF.Cos(pitch),
            MathF.Cos(yaw) * MathF.Cos(pitch),
            MathF.Sin(pitch)
        );

        Forward.Normalize();

        Right = Vector3.Normalize(Vector3.Cross(Vector3.UnitZ, Forward));

        Up = Vector3.Normalize(Vector3.Cross(Forward, Right));
    }

    public void UpdateView()
    {
        View = Matrix.CreateLookAt(Position, Position + Forward, Up);
    }
}
