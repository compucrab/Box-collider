using Box_collider;
using Microsoft.Xna.Framework.Graphics;

internal class Shape<T>
    where T : struct, IVertexType
{
    private GraphicsDevice _graphics;
    private VertexBuffer _vertexBuffer;
    private int VertexCount;

    private Camera _camera;

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
