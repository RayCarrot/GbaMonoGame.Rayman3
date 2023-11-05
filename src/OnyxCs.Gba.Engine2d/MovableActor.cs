using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.Engine2d;

public class MovableActor : InteractableActor
{
    public MovableActor(int id, ActorResource actorResource) : base(id, actorResource)
    {
        MapCollisionType = actorResource.Model.MapCollisionType;
        HasMapCollision = actorResource.Model.HasMapCollision;
        HasObjectCollision = actorResource.Model.HasObjectCollision;

        ActorFlag_C = false;
    }

    public Mechanic Mechanic { get; } = new();
    public MovableActor LinkedMovementActor { get; set; }
    public Vector2 Speed { get; set; }

    // Flags
    public ActorMapCollisionType MapCollisionType { get; }
    public bool HasMapCollision { get; }
    public bool HasObjectCollision { get; }

    private bool CheckObjectCollision1(Rectangle actorDetectionBox, Rectangle otherDetectionBox)
    {
        if (!actorDetectionBox.Intersects(otherDetectionBox))
            return false;

        // TODO: Modify position and speed

        return true;
    }

    private bool CheckObjectCollision2(Rectangle actorDetectionBox, Rectangle otherDetectionBox)
    {
        if (!actorDetectionBox.Intersects(otherDetectionBox))
            return false;

        // TODO: Modify position and speed

        return true;
    }

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

        if (LinkedMovementActor != null)
        {
            LinkedMovementActor.Move();
            Speed += LinkedMovementActor.Speed;
        }

        if (Speed != Vector2.Zero)
        {
            Position += Speed;

            if (HasObjectCollision)
            {
                IsTouchingActor = false;

                Rectangle detectionBox = GetAbsoluteBox(DetectionBox);

                foreach (BaseActor actor in Frame.GetComponent<Scene2D>().GameObjects.EnumerateAllActors(isEnabled: true))
                {
                    if (actor != this && actor.ActorFlag_6 && actor is ActionActor actionActor)
                    {
                        Rectangle otherDetectionBox = actionActor.GetAbsoluteBox(actionActor.DetectionBox);

                        if (!actionActor.ActorFlag_E)
                        {
                            if (CheckObjectCollision1(detectionBox, otherDetectionBox))
                            {
                                IsTouchingActor = true;
                                detectionBox = GetAbsoluteBox(DetectionBox);
                            }
                        }
                        else
                        {
                            if (CheckObjectCollision2(detectionBox, otherDetectionBox))
                            {
                                IsTouchingActor = true;
                                detectionBox = GetAbsoluteBox(DetectionBox);
                            }
                        }
                    }
                }
            }

            if (HasMapCollision)
            {
                IsTouchingMap = false;

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

            if (LinkedMovementActor != null)
            {
                Speed = new Vector2(Speed.X, 0);
            }
        }

        ActorFlag_C = true;
    }

    public override void Step()
    {
        IsTouchingMap = false;
        base.Step();
    }
}