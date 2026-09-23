using Box_collider.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// handles render geometry
internal class Mesh<T> : Shape
    where T : struct, IVertexType
{
    private GraphicsDevice _graphics;
    private VertexBuffer _vertexBuffer; // holds geometry data
    private int VertexCount;

    private Camera _camera;

    public Mesh(GraphicsDevice graphics, Camera camera, T[] vertices)
    {
        _graphics = graphics;
        _camera = camera;

        _vertexBuffer = new VertexBuffer(
            graphics,
            typeof(T),
            vertices.Length,
            BufferUsage.WriteOnly
        );

        _vertexBuffer.SetData(vertices);

        VertexCount = vertices.Length;
    }

    public void Draw(BasicEffect effect, PrimitiveType primitiveType = PrimitiveType.TriangleList)
    {
        Matrix scale = Matrix.CreateScale(Scale);

        Matrix rotation =
            Matrix.CreateRotationX(Rotation.X)
            * Matrix.CreateRotationY(Rotation.Y)
            * Matrix.CreateRotationZ(Rotation.Z);

        Matrix translation = Matrix.CreateTranslation(Position);

        /*
         * world matrix belongs to object
         * world matrix = scale x rotation x position (strictly)
         */

        effect.World = scale * rotation * translation;
        effect.View = _camera.View;
        effect.Projection = _camera.Projection;

        // for the next drawing use this vertex buffer
        _graphics.SetVertexBuffer(_vertexBuffer);

        // pass vertex data to GPU
        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _graphics.DrawPrimitives(primitiveType, 0, VertexCount / 3);
        }
    }
}
