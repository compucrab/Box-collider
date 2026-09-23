using System;
using Box_collider.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/* view and projection matrix belongs to camera
 * view matrix - Where is the player's eye standing, and where is it looking?
 * projection matrix - How does the 3D world flatten onto a 2D monitor screen?
 */

internal class Camera
{
    public Vector3 Position;

    //3 directions to determine camera's orientation

    public Vector3 Forward;
    public Vector3 Right;
    public Vector3 Up;

    public Matrix View;
    public Matrix Projection;

    public float Speed = 5f;

    private float yaw = 0f;
    private float pitch = 0f;

    private float RotationSpeed = 0.003f;
    public float PanSpeed = 0.05f;

    private GraphicsDevice _graphics;

    private bool orbiting = false;

    public Camera(GraphicsDevice graphics)
    {
        _graphics = graphics;

        Position = new Vector3(0, -23, 5);

        Forward = Vector3.UnitY; // y is foward
        Right = Vector3.UnitX; // x is right
        Up = Vector3.UnitZ; // z is up

        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(60), // The camera can see a 60° wide vertical field of view.
            graphics.Viewport.AspectRatio,
            0.1f, // Don't render objects closer than 0.1 units from the camera
            100f // Don't render objects farther than 100 units from the camera
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
        Position += direction * Speed * Globals.DeltaTime;
    }

    public void Update()
    {
        bool orbitInput = KeyboardManager.IsHeld(Keys.LeftAlt) && MouseManager.LeftPressed;

        bool panInput =
            KeyboardManager.IsHeld(Keys.LeftShift)
            && KeyboardManager.IsHeld(Keys.LeftAlt)
            && MouseManager.LeftPressed;

        if (panInput)
        {
            Pan();
        }
        else if (orbitInput)
        {
            if (!orbiting)
            {
                orbiting = true;
                MouseManager.ResetPosition(MouseCenter());
            }

            Point delta = MouseManager.Delta;

            yaw += delta.X * RotationSpeed;
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

    private void Pan()
    {
        if (!orbiting)
        {
            orbiting = true;
            MouseManager.ResetPosition(MouseCenter());
            return;
        }

        Point delta = MouseManager.Delta;

        Position += Right * -delta.X * PanSpeed;
        Position += Up * delta.Y * PanSpeed;
    }

    private void UpdateDirection()
    {
        // pitch controls up and down
        float cosPitch = MathF.Cos(pitch);
        float sinPitch = MathF.Sin(pitch);

        // yaw controls left right
        float sinYaw = MathF.Sin(yaw);
        float cosYaw = MathF.Cos(yaw);

        /*
         * Forward = Vector3(X,Y,Z)
         * X = horizontal X × horizontal amount of pitch
         * Y = horizontal Y × horizontal amount of pitch
         * Z = vertical amount
         * we multiplied horizontal amount of pitch because it affects the yaw directly
        */

        Forward = new Vector3(sinYaw * cosPitch, cosYaw * cosPitch, sinPitch);
        Forward.Normalize(); // make length 1 unit

        Right = new Vector3(cosYaw, -sinYaw, 0); // length already 1 unit

        // Find a direction that is perpendicular to both Right and Forward.
        Up = Vector3.Cross(Right, Forward); // order matters else it would be opposite direction
        Up.Normalize(); // make length 1 unit
    }

    public void UpdateView()
    {
        // parameters (camera's position, target position, up direction)
        View = Matrix.CreateLookAt(Position, Position + Forward, Up);
    }
}
