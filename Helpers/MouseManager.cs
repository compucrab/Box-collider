using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Box_collider.Helpers
{
    public static class MouseManager
    {
        private static MouseState currentState;
        private static MouseState previousState;

        public static Point Position => currentState.Position;

        public static Point Delta => currentState.Position - previousState.Position;

        public static bool LeftPressed => currentState.LeftButton == ButtonState.Pressed;

        public static bool RightPressed => currentState.RightButton == ButtonState.Pressed;

        public static void Update()
        {
            previousState = currentState;
            currentState = Mouse.GetState();
        }

        public static void SetPosition(Point position)
        {
            Mouse.SetPosition(position.X, position.Y);
        }

        public static void ResetPosition(Point position)
        {
            Mouse.SetPosition(position.X, position.Y);

            currentState = Mouse.GetState();
            previousState = currentState;
        }
    }
}
