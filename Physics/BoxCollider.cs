using Microsoft.Xna.Framework;

namespace Box_collider.Physics
{
    internal class BoxCollider
    {
        public Vector3 Size;

        public BoxCollider(Vector3 size)
        {
            Size = size;
        }

        public Vector3 GetHalfExtents(Vector3 scale)
        {
            return (Size * scale) / 2f;
        }

        public Vector3 GetMin(Vector3 position, Vector3 scale)
        {
            Vector3 halfExtents = GetHalfExtents(scale);
            return position - halfExtents;
        }

        public Vector3 GetMax(Vector3 position, Vector3 scale)
        {
            Vector3 halfExtents = GetHalfExtents(scale);
            return position + halfExtents;
        }
    }
}
