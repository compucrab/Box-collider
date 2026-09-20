using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Box_collider.Shapes
{
    // no index buffer used - rendered via non indexed rendering
    // GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 12);

    internal static class ShapeData
    {
        public static VertexPositionColor[] maincube =
        {
            // FRONT
            new VertexPositionColor(new Vector3(-1, -1, 1), Color.Red),
            new VertexPositionColor(new Vector3(-1, 1, 1), Color.Red),
            new VertexPositionColor(new Vector3(1, 1, 1), Color.Red),
            new VertexPositionColor(new Vector3(-1, -1, 1), Color.Red),
            new VertexPositionColor(new Vector3(1, 1, 1), Color.Red),
            new VertexPositionColor(new Vector3(1, -1, 1), Color.Red),
            // BACK
            new VertexPositionColor(new Vector3(1, -1, -1), Color.Blue),
            new VertexPositionColor(new Vector3(1, 1, -1), Color.Blue),
            new VertexPositionColor(new Vector3(-1, 1, -1), Color.Blue),
            new VertexPositionColor(new Vector3(1, -1, -1), Color.Blue),
            new VertexPositionColor(new Vector3(-1, 1, -1), Color.Blue),
            new VertexPositionColor(new Vector3(-1, -1, -1), Color.Blue),
            // LEFT
            new VertexPositionColor(new Vector3(-1, -1, -1), Color.Green),
            new VertexPositionColor(new Vector3(-1, 1, -1), Color.Green),
            new VertexPositionColor(new Vector3(-1, 1, 1), Color.Green),
            new VertexPositionColor(new Vector3(-1, -1, -1), Color.Green),
            new VertexPositionColor(new Vector3(-1, 1, 1), Color.Green),
            new VertexPositionColor(new Vector3(-1, -1, 1), Color.Green),
            // RIGHT
            new VertexPositionColor(new Vector3(1, -1, 1), Color.Yellow),
            new VertexPositionColor(new Vector3(1, 1, 1), Color.Yellow),
            new VertexPositionColor(new Vector3(1, 1, -1), Color.Yellow),
            new VertexPositionColor(new Vector3(1, -1, 1), Color.Yellow),
            new VertexPositionColor(new Vector3(1, 1, -1), Color.Yellow),
            new VertexPositionColor(new Vector3(1, -1, -1), Color.Yellow),
            // TOP
            new VertexPositionColor(new Vector3(-1, 1, 1), Color.White),
            new VertexPositionColor(new Vector3(-1, 1, -1), Color.White),
            new VertexPositionColor(new Vector3(1, 1, -1), Color.White),
            new VertexPositionColor(new Vector3(-1, 1, 1), Color.White),
            new VertexPositionColor(new Vector3(1, 1, -1), Color.White),
            new VertexPositionColor(new Vector3(1, 1, 1), Color.White),
            // BOTTOM
            new VertexPositionColor(new Vector3(-1, -1, -1), Color.Black),
            new VertexPositionColor(new Vector3(-1, -1, 1), Color.Black),
            new VertexPositionColor(new Vector3(1, -1, 1), Color.Black),
            new VertexPositionColor(new Vector3(-1, -1, -1), Color.Black),
            new VertexPositionColor(new Vector3(1, -1, 1), Color.Black),
            new VertexPositionColor(new Vector3(1, -1, -1), Color.Black),
        };

        public static VertexPositionTexture[] ground =
        {
            // Triangle 1
            new VertexPositionTexture(new Vector3(-10, -10, 0), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(-10, 10, 0), new Vector2(0, 10)),
            new VertexPositionTexture(new Vector3(10, 10, 0), new Vector2(10, 10)),
            // Triangle 2
            new VertexPositionTexture(new Vector3(-10, -10, 0), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(10, 10, 0), new Vector2(10, 10)),
            new VertexPositionTexture(new Vector3(10, -10, 0), new Vector2(10, 0)),
        };

        public static float DeltaTime = 0f;
    }
}
