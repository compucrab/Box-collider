using Microsoft.Xna.Framework;

internal class PlaneCollider
{
    public Vector3 Position;
    public Vector3 Normal;
    public Vector2 Size;

    public PlaneCollider(Vector3 position, Vector3 normal, Vector2 size)
    {
        Position = position;
        Normal = Vector3.Normalize(normal);
        Size = size;
    }
}
