using Box_collider.Shapes;
using Microsoft.Xna.Framework;

namespace Box_collider.Physics
{
    internal class RigidBody
    {
        public Shape Shape;
        public BoxCollider Collider;
        public bool IsActive = true;

        public Vector3 Velocity = Vector3.Zero;

        public RigidBody(Shape shape, BoxCollider collider)
        {
            Shape = shape;
            Collider = collider;
        }
    }
}
