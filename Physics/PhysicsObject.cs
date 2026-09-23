using Box_collider.Physics;
using Microsoft.Xna.Framework.Graphics;

internal class PhysicsObject
{
    public string Name;
    public Mesh<VertexPositionColor> Mesh;
    public RigidBody Body;

    public PhysicsObject(string name, Mesh<VertexPositionColor> mesh, RigidBody body)
    {
        Name = name;
        Mesh = mesh;
        Body = body;
    }
}
