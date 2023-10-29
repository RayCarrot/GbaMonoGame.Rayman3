using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.Engine2d;

public class MovableActor : InteractableActor
{
    public MovableActor(int id, ActorResource actorResource) : base(id, actorResource)
    {
        HasMapCollision = actorResource.Model.HasMapCollision;
        MapCollisionType = actorResource.Model.MapCollisionType;
    }

    public Mechanic Mechanic { get; } = new();
    public Vector2 Speed { get; set; } = new();
    public bool HasMapCollision { get; }
    public ActorMapCollisionType MapCollisionType { get; }

    private void CheckMapCollisionX()
    {
        // TODO: Implement
    }

    private void CheckMapCollisionY()
    {
        // TODO: Implement
    }

    private void CheckMapCollisionExtendedX()
    {
        // TODO: Implement
    }

    private void CheckMapCollisionExtendedY()
    {
        Scene2D scene = Frame.GetComponent<Scene2D>();
        Rectangle detectionBox = GetAbsoluteBox(DetectionBox);

        float speedY = Speed.Y;

        if (speedY < 0)
        {
            // TODO: Implement
        }
        else
        {
            byte type = scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.Bottom - Constants.TileSize));

            if (type is >= 16 and < 32)
            {
                // TODO: Implement
            }
            else
            {
                type = scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.Bottom));

                if (type < 32)
                {
                    if (type < 16)
                    {
                        Speed = new Vector2(Speed.X, 0);
                        Position = new Vector2(Position.X, Position.Y - (detectionBox.Bottom % Constants.TileSize));
                    }
                    else
                    {
                        // TODO: Implement
                    }
                }
                else
                {
                    // TODO: Implement
                }
            }
        }

        // TODO: Implement
    }

    public void Move()
    {
        // Update the speed
        Speed = Mechanic.UpdateSpeedAction();

        if (Speed == Vector2.Zero) 
            return;
        
        Position += Speed;

        if (HasMapCollision)
        {
            switch (MapCollisionType)
            {
                case ActorMapCollisionType.CheckX:
                    CheckMapCollisionX();
                    break;

                case ActorMapCollisionType.CheckY:
                    CheckMapCollisionY();
                    break;

                case ActorMapCollisionType.CheckXY:
                    CheckMapCollisionY();
                    CheckMapCollisionX();
                    break;

                case ActorMapCollisionType.CheckExtendedX:
                    CheckMapCollisionExtendedX();
                    break;

                case ActorMapCollisionType.CheckExtendedY:
                    CheckMapCollisionExtendedY();
                    break;

                case ActorMapCollisionType.CheckExtendedXY:
                    CheckMapCollisionExtendedY();
                    CheckMapCollisionExtendedX();
                    break;
            }
        }
    }

    public override void Step()
    {
        // TODO: Change some flags
        base.Step();
    }
}