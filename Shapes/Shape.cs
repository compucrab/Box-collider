using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

internal class Shape<T>
    where T : struct, IVertexType
{
    private GraphicsDevice _graphics;
    private VertexBuffer _vertexBuffer;
    private int VertexCount;

    private Camera _camera;

    // Object transform
    public Vector3 Position = Vector3.Zero;
    public Vector3 Rotation = Vector3.Zero;
    public Vector3 Scale = Vector3.One;

    public Shape(GraphicsDevice graphics, Camera camera, T[] vertices)
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

    public void Draw(BasicEffect effect, PrimitiveType primitiveType)
    {
        Matrix scale = Matrix.CreateScale(Scale);

        Matrix rotation =
            Matrix.CreateRotationX(Rotation.X)
            * Matrix.CreateRotationY(Rotation.Y)
            * Matrix.CreateRotationZ(Rotation.Z);

        Matrix translation = Matrix.CreateTranslation(Position);

        effect.World = scale * rotation * translation;

        effect.View = _camera.View;
        effect.Projection = _camera.Projection;

        _graphics.SetVertexBuffer(_vertexBuffer);

        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();

            _graphics.DrawPrimitives(primitiveType, 0, VertexCount / 3);
        }
    }
}
