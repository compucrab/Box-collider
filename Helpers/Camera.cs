using System;
using Box_collider.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/*
 * View and Projection Matrix Fundamentals:
 * - View matrix: Defines camera position and orientation (Where is the eye, and where is it looking?).
 * - Projection matrix: Translates 3D world coordinates onto the 2D display viewport (Perspective/FOV).
 */
internal class Camera
{
    private readonly GraphicsDevice _graphics;

    #region Transform & Direction Vectors

    public Vector3 Position;
    private Vector3 _targetPosition;

    // 3 orthogonal unit vectors determining camera orientation (Z-Up coordinate system)
    public Vector3 Forward = Vector3.UnitY; // +Y is forward
    public Vector3 Right = Vector3.UnitX; // +X is right
    public Vector3 Up = Vector3.UnitZ; // +Z is up
    #endregion

    #region Matrices

    public Matrix View;
    public Matrix Projection;

    #endregion

    #region Rotation Settings & State

    private float _yaw = 0f;
    private float _pitch = 0f;
    private float _targetYaw = 0f;
    private float _targetPitch = 0f;

    public float RotationSpeed = 0.005f;
    public float RotationSmoothness = 8f;

    #endregion

    #region Translation & Zoom Settings

    public float MoveSpeed = 5f;
    public float PanSpeed = 0.05f;
    public float ZoomSpeed = 0.05f;
    public float PanSmoothness = 10f;

    #endregion

    #region Internal State Flags

    private bool _isOrbiting = false;
    private bool _isPanning = false;

    #endregion

    public Camera(GraphicsDevice graphics)
    {
        _graphics = graphics;

        // Default initial camera placement facing forward along +Y
        Position = new Vector3(0, -23, 5);
        _targetPosition = Position;

        // Setup perspective projection (60 deg vertical FOV, near/far clipping planes)
        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(60f),
            _graphics.Viewport.AspectRatio,
            0.1f,
            100f
        );

        RegisterKeyboardMovement();
        UpdateDirection();
        UpdateView();
    }

    private void RegisterKeyboardMovement()
    {
        // Directional translation
        KeyboardManager.RegisterHeld(Keys.Up, () => MoveTarget(Forward));
        KeyboardManager.RegisterHeld(Keys.Down, () => MoveTarget(-Forward));
        KeyboardManager.RegisterHeld(Keys.Left, () => MoveTarget(-Right));
        KeyboardManager.RegisterHeld(Keys.Right, () => MoveTarget(Right));

        KeyboardManager.RegisterHeld(Keys.S, () => MoveTarget(Forward));
        KeyboardManager.RegisterHeld(Keys.D, () => MoveTarget(-Forward));

        // Elevation translation
        KeyboardManager.RegisterHeld(Keys.Space, () => MoveTarget(Up));
        KeyboardManager.RegisterHeld(Keys.LeftControl, () => MoveTarget(-Up));
    }

    private void MoveTarget(Vector3 direction)
    {
        _targetPosition += direction * MoveSpeed * Globals.DeltaTime;
    }

    public void Update()
    {
        HandleMouseInput();
        ApplySmoothing();
        UpdateDirection();
        UpdateView();
    }

    private void HandleMouseInput()
    {
        // Input state checks
        bool isAltHeld = KeyboardManager.IsHeld(Keys.LeftAlt);
        bool isShiftHeld = KeyboardManager.IsHeld(Keys.LeftShift);
        bool isLeftClick = MouseManager.LeftPressed;

        bool panInput = isShiftHeld && isAltHeld && isLeftClick;
        bool orbitInput = isAltHeld && !isShiftHeld && isLeftClick;

        // 1. Pan Handling
        if (panInput)
        {
            ProcessPan();
            _isOrbiting = false;
        }
        // 2. Orbit Handling
        else if (orbitInput)
        {
            ProcessOrbit();
            _isPanning = false;
        }
        // Reset drag states when inputs released
        else
        {
            _isOrbiting = false;
            _isPanning = false;
        }

        // 3. Mouse Wheel Zoom Handling
        int wheelDelta = MouseManager.ScrollWheelDelta;
        if (wheelDelta != 0)
        {
            _targetPosition += Forward * wheelDelta * ZoomSpeed;
        }
    }

    private void ProcessPan()
    {
        if (!_isPanning)
        {
            _isPanning = true;
            return;
        }

        Point delta = MouseManager.Delta;

        // Move target position across local Right and Up camera vectors
        _targetPosition += Right * (-delta.X * PanSpeed);
        _targetPosition += Up * (delta.Y * PanSpeed);
    }

    private void ProcessOrbit()
    {
        if (!_isOrbiting)
        {
            _isOrbiting = true;
            return;
        }

        Point delta = MouseManager.Delta;

        _targetYaw += delta.X * RotationSpeed;
        _targetPitch -= delta.Y * RotationSpeed;

        // Clamp pitch to prevent camera flipping upside down (-89 to +89 degrees)
        _targetPitch = MathHelper.Clamp(
            _targetPitch,
            MathHelper.ToRadians(-89f),
            MathHelper.ToRadians(89f)
        );
    }

    private void ApplySmoothing()
    {
        // Frame-rate independent exponential interpolation factors
        float rotationFactor = 1f - MathF.Exp(-RotationSmoothness * Globals.DeltaTime);
        float panFactor = 1f - MathF.Exp(-PanSmoothness * Globals.DeltaTime);

        // Smooth rotation
        _yaw = MathHelper.Lerp(_yaw, _targetYaw, rotationFactor);
        _pitch = MathHelper.Lerp(_pitch, _targetPitch, rotationFactor);

        // Smooth translation
        Position = Vector3.Lerp(Position, _targetPosition, panFactor);
    }

    // Recomputes Forward, Right, and Up orientation vectors based on pitch and yaw angles.
    private void UpdateDirection()
    {
        float cosPitch = MathF.Cos(_pitch);
        float sinPitch = MathF.Sin(_pitch);
        float sinYaw = MathF.Sin(_yaw);
        float cosYaw = MathF.Cos(_yaw);

        /*
         * Calculating local forward vector relative to spherical yaw/pitch coordinates:
         * X = horizontal component * cosine pitch scaling
         * Y = vertical component * cosine pitch scaling
         * Z = pitch elevation
         */
        Forward = new Vector3(sinYaw * cosPitch, cosYaw * cosPitch, sinPitch);
        Forward.Normalize();

        // Right vector lies purely on the XY horizontal plane (Z=0)
        Right = new Vector3(cosYaw, -sinYaw, 0f);

        // Right-handed perpendicular up vector via cross product
        Up = Vector3.Cross(Right, Forward);
        Up.Normalize();
    }

    // Rebuilds the View matrix targeting Position looking along Forward vector.
    public void UpdateView()
    {
        View = Matrix.CreateLookAt(Position, Position + Forward, Up);
    }
}
