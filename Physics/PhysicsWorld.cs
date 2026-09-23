using System;
using System.Collections.Generic;
using Box_collider.Physics;
using Microsoft.Xna.Framework;

internal class PhysicsWorld
{
    public Vector3 Gravity = new Vector3(0, 0, -9.81f);

    public PlaneCollider Ground;

    public PhysicsWorld(PlaneCollider ground)
    {
        Ground = ground;
    }

    public void Update(List<PhysicsObject> objects, float dt)
    {
        foreach (PhysicsObject obj in objects)
        {
            RigidBody body = obj.Body;

            if (!body.IsActive)
                continue;

            body.Velocity += Gravity * dt;
            body.Shape.Position += body.Velocity * dt;
        }

        // Box vs Box
        for (int i = 0; i < objects.Count; i++)
        {
            RigidBody a = objects[i].Body;

            if (!a.IsActive)
                continue;

            for (int j = i + 1; j < objects.Count; j++)
            {
                RigidBody b = objects[j].Body;

                if (!b.IsActive)
                    continue;

                ResolveBoxBoxCollision(a, b);
            }
        }

        // Box vs Ground
        foreach (PhysicsObject obj in objects)
        {
            RigidBody body = obj.Body;

            if (!body.IsActive)
                continue;

            ResolveBoxPlaneCollision(body);
            CheckFall(body);
        }

        for (int i = objects.Count - 1; i >= 0; i--)
        {
            if (!objects[i].Body.IsActive)
            {
                objects.RemoveAt(i);
            }
        }
    }

    private void ResolveBoxPlaneCollision(RigidBody body)
    {
        Vector3 min = body.Collider.GetMin(body.Shape.Position, body.Shape.Scale);

        Vector3 max = body.Collider.GetMax(body.Shape.Position, body.Shape.Scale);

        // Ground boundaries
        float groundMinX = Ground.Position.X - Ground.Size.X / 2f;
        float groundMaxX = Ground.Position.X + Ground.Size.X / 2f;

        float groundMinY = Ground.Position.Y - Ground.Size.Y / 2f;
        float groundMaxY = Ground.Position.Y + Ground.Size.Y / 2f;

        // Is the cube horizontally over the ground?
        bool overGround =
            max.X > groundMinX && min.X < groundMaxX && max.Y > groundMinY && min.Y < groundMaxY;

        if (!overGround)
            return;

        // Cube is below the ground
        float distance = min.Z - Ground.Position.Z;

        if (distance < 0)
        {
            body.Shape.Position.Z -= distance;

            if (body.Velocity.Z < 0)
            {
                body.Velocity.Z = 0;
            }
        }
    }

    private bool CheckBoxBoxCollision(RigidBody a, RigidBody b, out CollisionInfo collision)
    {
        collision = default;

        Vector3 minA = a.Collider.GetMin(a.Shape.Position, a.Shape.Scale);

        Vector3 maxA = a.Collider.GetMax(a.Shape.Position, a.Shape.Scale);

        Vector3 minB = b.Collider.GetMin(b.Shape.Position, b.Shape.Scale);

        Vector3 maxB = b.Collider.GetMax(b.Shape.Position, b.Shape.Scale);

        float overlapX = MathF.Min(maxA.X, maxB.X) - MathF.Max(minA.X, minB.X);

        float overlapY = MathF.Min(maxA.Y, maxB.Y) - MathF.Max(minA.Y, minB.Y);

        float overlapZ = MathF.Min(maxA.Z, maxB.Z) - MathF.Max(minA.Z, minB.Z);

        // No collision
        if (overlapX <= 0 || overlapY <= 0 || overlapZ <= 0)
        {
            return false;
        }

        // Find smallest penetration
        if (overlapX < overlapY && overlapX < overlapZ)
        {
            collision.Penetration = overlapX;

            collision.Normal =
                a.Shape.Position.X < b.Shape.Position.X ? -Vector3.UnitX : Vector3.UnitX;
        }
        else if (overlapY < overlapZ)
        {
            collision.Penetration = overlapY;

            collision.Normal =
                a.Shape.Position.Y < b.Shape.Position.Y ? -Vector3.UnitY : Vector3.UnitY;
        }
        else
        {
            collision.Penetration = overlapZ;

            collision.Normal =
                a.Shape.Position.Z < b.Shape.Position.Z ? -Vector3.UnitZ : Vector3.UnitZ;
        }

        return true;
    }

    private void ResolveBoxBoxCollision(RigidBody a, RigidBody b)
    {
        if (!CheckBoxBoxCollision(a, b, out CollisionInfo collision))
        {
            return;
        }

        // Position correction
        Vector3 correction = collision.Normal * collision.Penetration;

        a.Shape.Position += correction * 0.5f;
        b.Shape.Position -= correction * 0.5f;

        // Velocity correction
        Vector3 relativeVelocity = a.Velocity - b.Velocity;

        float velocityAlongNormal = Vector3.Dot(relativeVelocity, collision.Normal);

        // Already moving apart
        if (velocityAlongNormal > 0)
            return;

        // Remove velocity along collision normal
        a.Velocity -= velocityAlongNormal * collision.Normal;

        b.Velocity += velocityAlongNormal * collision.Normal;
    }

    private void CheckFall(RigidBody body)
    {
        if (body.Shape.Position.Z < Ground.Position.Z - 20f)
        {
            body.IsActive = false;
        }
    }
}
