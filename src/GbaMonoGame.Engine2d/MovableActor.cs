using System;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Ubisoft.GbaEngine;
using ImGuiNET;

namespace GbaMonoGame.Engine2d;

public abstract class MovableActor : InteractableActor
{
    protected MovableActor(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        MapCollisionType = actorResource.Model.MapCollisionType;
        CheckAgainstMapCollision = actorResource.Model.CheckAgainstMapCollision;
        CheckAgainstObjectCollision = actorResource.Model.CheckAgainstObjectCollision;

        HasMoved = false;
    }

    public MechModel MechModel { get; } = new();
    public MovableActor LinkedMovementActor { get; set; }
    public Vector2 Speed { get; set; }

    // Flags
    public ActorMapCollisionType MapCollisionType { get; }
    public bool CheckAgainstMapCollision { get; set; }
    public bool CheckAgainstObjectCollision { get; set; }

    private bool CheckObjectCollisionXY(Box actorDetectionBox, Box otherDetectionBox)
    {
        Box intersectBox = Box.Intersect(actorDetectionBox, otherDetectionBox);

        if (intersectBox == Box.Empty)
            return false;

        float width = intersectBox.Width;
        float height = intersectBox.Height;

        if (Speed.Y > 0 &&
            otherDetectionBox.MinY > actorDetectionBox.MinY &&
            otherDetectionBox.MaxY > actorDetectionBox.MaxY &&
            // TODO: Should we remove this? Only there in base engine because of limitations.
            height < Tile.Size)
        {
            Speed -= new Vector2(0, height);
            Position -= new Vector2(0, height);

            // NOTE: The original engine casts speed y and pos y to integers here (floor if positive, ceil if negative)

            return true;
        }

        if (Speed.Y < 0 &&
            actorDetectionBox.MaxY > otherDetectionBox.MaxY &&
            actorDetectionBox.MinY > otherDetectionBox.MinY)
        {
            Speed += new Vector2(0, height);
            Position += new Vector2(0, height);

            // NOTE: The original engine casts speed y and pos y to integers here (floor if positive, ceil if negative)

            return true;
        }

        if (Speed.X > 0)
        {
            if (otherDetectionBox.MaxX > actorDetectionBox.MaxX)
            {
                Speed -= new Vector2(width, 0);
                Position -= new Vector2(width, 0);

                // NOTE: The original engine casts speed x and pos x to integers here (floor if positive, ceil if negative)
            }
        }
        else if (Speed.X < 0)
        {
            if (actorDetectionBox.MinX > otherDetectionBox.MinX)
            {
                Speed += new Vector2(width, 0);
                Position += new Vector2(width, 0);

                // NOTE: The original engine casts speed x and pos x to integers here (floor if positive, ceil if negative)
            }
        }

        return true;
    }

    private bool CheckObjectCollisionX(Box actorDetectionBox, Box otherDetectionBox)
    {
        Box intersectBox = Box.Intersect(actorDetectionBox, otherDetectionBox);

        if (intersectBox == Box.Empty)
            return false;

        float width = intersectBox.Width;

        // TODO: Should we remove this? Only there in base engine because of limitations.
        if (width > Tile.Size - 1)
            width = Tile.Size - 1;

        if (Position.X > otherDetectionBox.Center.X)
        {
            Speed += new Vector2(width, 0);
            Position += new Vector2(width, 0);
        }
        else
        {
            Speed -= new Vector2(width, 0);
            Position -= new Vector2(width, 0);
        }

        // NOTE: The original engine casts speed x and pos x to integers here (floor if positive, ceil if negative)

        return true;
    }

    private void PushOutOfTile(float position, Direction direction, bool resetSpeed = false)
    {
        Vector2 delta = direction switch
        {
            Direction.Up => new Vector2(0, -MathHelpers.Mod(position, Tile.Size)),
            Direction.Down => new Vector2(0, Tile.Size - MathHelpers.Mod(position, Tile.Size)),
            Direction.Left => new Vector2(-MathHelpers.Mod(position, Tile.Size), 0),
            Direction.Right => new Vector2(Tile.Size - MathHelpers.Mod(position, Tile.Size), 0),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

        if (resetSpeed )
            Speed = direction switch
            {
                Direction.Up or Direction.Down => new Vector2(Speed.X, 0),
                Direction.Left or Direction.Right => new Vector2(0, Speed.Y),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        else
            Speed += delta;

        Position += delta;
        IsTouchingMap = true;
    }

    private void CheckMapCollisionX()
    {
        Box detectionBox = GetDetectionBox();

        // Moving left
        if (Speed.X < 0)
        {
            PhysicalType type = Scene.GetPhysicalType(detectionBox.MiddleLeft);
            if (type.IsSolid && type != PhysicalTypeValue.Grab && type != PhysicalTypeValue.Passthrough)
                PushOutOfTile(detectionBox.MinX, Direction.Right);
        }
        // Moving right
        else
        {
            PhysicalType type = Scene.GetPhysicalType(detectionBox.MiddleRight);
            if (type.IsSolid && type != PhysicalTypeValue.Grab && type != PhysicalTypeValue.Passthrough)
                PushOutOfTile(detectionBox.MaxX, Direction.Left);
        }
    }

    private void CheckMapCollisionY()
    {
        Box detectionBox = GetDetectionBox();

        // Moving up
        if (Speed.Y < 0)
        {
            PhysicalType type = Scene.GetPhysicalType(detectionBox.TopCenter);
            if (type.IsFullySolid && type != PhysicalTypeValue.Grab && type != PhysicalTypeValue.Passthrough)
                PushOutOfTile(detectionBox.MinY, Direction.Down);
        }
        // Moving down
        else
        {
            PhysicalType type = Scene.GetPhysicalType(detectionBox.BottomCenter);
            if (type.IsSolid)
                PushOutOfTile(detectionBox.MaxY, Direction.Up);
        }
    }

    private void CheckMapCollisionXY()
    {
        // The game doesn't call these, but rather re-implements them. Code
        // appears identical though, so might as well. The game probably does
        // it for performance reasons.
        CheckMapCollisionY();
        CheckMapCollisionX();
    }

    private void CheckMapCollisionExtendedX()
    {
        throw new NotImplementedException();
    }

    private void CheckMapCollisionExtendedY()
    {
        throw new NotImplementedException();
    }

    private void CheckMapCollisionExtendedXY()
    {
        Box detectionBox = GetDetectionBox();

        // Moving up
        if (Speed.Y < 0)
        {
            PhysicalType type = Scene.GetPhysicalType(detectionBox.TopCenter);

            if (!type.IsSolid)
            {
                // Get top-right type
                type = Scene.GetPhysicalType(detectionBox.TopRight);

                if (!type.IsSolid)
                {
                    type = Scene.GetPhysicalType(detectionBox.TopLeft);

                    if (type.IsSolid)
                    {
                        PhysicalType otherType = Scene.GetPhysicalType(detectionBox.TopLeft + Tile.Down);

                        if (otherType.IsSolid)
                            type = PhysicalTypeValue.None;
                    }
                }
                else
                {
                    PhysicalType otherType = Scene.GetPhysicalType(detectionBox.TopRight + Tile.Down);

                    if (otherType.IsSolid)
                        type = PhysicalTypeValue.None;

                    if (!type.IsSolid)
                    {
                        type = Scene.GetPhysicalType(detectionBox.TopLeft + Tile.Down * 2);

                        if (type.IsSolid)
                        {
                            otherType = Scene.GetPhysicalType(detectionBox.TopLeft + Tile.Down * 3);

                            if (otherType.IsSolid)
                                type = PhysicalTypeValue.None;
                        }
                    }
                }
            }

            if (type.IsFullySolid && type != PhysicalTypeValue.Grab && type != PhysicalTypeValue.Passthrough)
                PushOutOfTile(detectionBox.MinY, Direction.Down);
        }
        // Moving down or still
        else
        {
            PhysicalType type = Scene.GetPhysicalType(detectionBox.BottomCenter + Tile.Up);

            if (type.IsAngledSolid)
            {
                float tileHeight = type.GetAngleSolidHeight(detectionBox.CenterX);

                if (Speed.Y == 0 && tileHeight != 0)
                    Position -= new Vector2(0, tileHeight);
                else
                    Position -= new Vector2(0, tileHeight + MathHelpers.Mod(detectionBox.MaxY, Tile.Size));

                Speed = new Vector2(Speed.X, 0);
                IsTouchingMap = true;
            }
            else
            {
                type = Scene.GetPhysicalType(detectionBox.BottomCenter);

                if (type.IsFullySolid)
                {
                    PushOutOfTile(detectionBox.MaxY, Direction.Up, resetSpeed: true);
                }
                else if (type.IsAngledSolid)
                {
                    float tileHeight = Tile.Size - type.GetAngleSolidHeight(detectionBox.CenterX);

                    if (Speed.Y == 0)
                    {
                        Position += new Vector2(0, tileHeight - MathHelpers.Mod(detectionBox.MaxY, Tile.Size));
                        Speed = new Vector2(Speed.X, 0);
                        IsTouchingMap = true;
                    }
                    else if (MathHelpers.Mod(detectionBox.MaxY, Tile.Size) > tileHeight)
                    {
                        Position -= new Vector2(0, MathHelpers.Mod(detectionBox.MaxY, Tile.Size) - tileHeight);
                        Speed = new Vector2(Speed.X, 0);
                        IsTouchingMap = true;
                    }
                }
                else
                {
                    type = Scene.GetPhysicalType(detectionBox.BottomRight);

                    if (!type.IsSolid)
                    {
                        type = Scene.GetPhysicalType(detectionBox.BottomLeft);

                        if (type.IsSolid)
                        {
                            PhysicalType otherType = Scene.GetPhysicalType(detectionBox.BottomLeft + Tile.Up);

                            if (otherType.IsSolid)
                                type = PhysicalTypeValue.None;
                        }
                    }
                    else
                    {
                        PhysicalType otherType = Scene.GetPhysicalType(detectionBox.BottomRight + Tile.Up);

                        if (otherType.IsSolid)
                            type = PhysicalTypeValue.None;

                        if (!type.IsSolid)
                        {
                            type = Scene.GetPhysicalType(detectionBox.BottomLeft);

                            if (type.IsSolid)
                            {
                                otherType = Scene.GetPhysicalType(detectionBox.BottomLeft + Tile.Up);

                                if (otherType.IsSolid)
                                    type = PhysicalTypeValue.None;
                            }
                        }
                    }

                    if (type.IsFullySolid)
                        PushOutOfTile(detectionBox.MaxY, Direction.Up, resetSpeed: true);
                }
            }
        }

        if (Speed.X == 0)
            return;

        // Get the x position to detect
        float detectionX = Speed.X > 0 ? detectionBox.MaxX : detectionBox.MinX;

        // Get bottom-center type
        PhysicalType typeX = Scene.GetPhysicalType(detectionBox.BottomCenter);

        if (typeX.IsAngledSolid)
            return;

        typeX = Scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MaxY - Tile.Size));

        if (typeX.IsAngledSolid)
            return;

        typeX = Scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MaxY + Tile.Size));

        if (typeX.IsAngledSolid)
            return;

        typeX = Scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MaxY + Tile.Size * 2));

        if (typeX.IsAngledSolid)
            return;

        // Moving up or still
        if (Speed.Y <= 0)
        {
            float y = detectionBox.MinY + Tile.Size;
            int count = (int)(detectionBox.Height / Tile.Size) - 1;

            for (int i = 0; i < count; i++)
            {
                typeX = Scene.GetPhysicalType(new Vector2(detectionX, y));
                y += Tile.Size;

                if (typeX.IsFullySolid && typeX.Value != PhysicalTypeValue.Grab && typeX.Value != PhysicalTypeValue.Passthrough)
                    break;
            }
        }
        // Moving down
        else
        {
            typeX = Scene.GetPhysicalType(new Vector2(detectionX, detectionBox.MaxY));

            if (!typeX.IsFullySolid || typeX.Value == PhysicalTypeValue.Grab || typeX.Value == PhysicalTypeValue.Passthrough)
            {
                float y = detectionBox.MinY + Tile.Size;
                int count = (int)(detectionBox.Height / Tile.Size) - 1;

                for (int i = 0; i < count; i++)
                {
                    typeX = Scene.GetPhysicalType(new Vector2(detectionX, y));
                    y += Tile.Size;

                    if (typeX.IsFullySolid && typeX.Value != PhysicalTypeValue.Grab && typeX.Value != PhysicalTypeValue.Passthrough)
                        break;
                }
            }
        }

        if (typeX.IsFullySolid && typeX.Value != PhysicalTypeValue.Grab && typeX.Value != PhysicalTypeValue.Passthrough)
            PushOutOfTile(detectionX, Speed.X > 0 ? Direction.Left : Direction.Right);
    }

    public void Move()
    {
        if (HasMoved)
            return;

        // Update the speed
        Speed = MechModel.UpdateSpeedAction();

        if (LinkedMovementActor != null)
        {
            LinkedMovementActor.Move();

            // If on a linked object then we remove gravity
            Speed = new Vector2(Speed.X, 0);

            // Add speed from the linked object
            Speed += LinkedMovementActor.Speed;
        }

        if (Speed != Vector2.Zero)
        {
            Position += Speed;

            if (CheckAgainstObjectCollision)
            {
                IsTouchingActor = false;

                Box detectionBox = GetDetectionBox();

                foreach (BaseActor actor in Scene.KnotManager.EnumerateAllActors(isEnabled: true))
                {
                    if (actor != this && actor.IsSolid && actor is ActionActor actionActor)
                    {
                        Box otherDetectionBox = actionActor.GetDetectionBox();

                        if (!actionActor.IsObjectCollisionXOnly)
                        {
                            if (CheckObjectCollisionXY(detectionBox, otherDetectionBox))
                            {
                                IsTouchingActor = true;
                                detectionBox = GetDetectionBox();
                            }
                        }
                        else
                        {
                            if (CheckObjectCollisionX(detectionBox, otherDetectionBox))
                            {
                                IsTouchingActor = true;
                                detectionBox = GetDetectionBox();
                            }
                        }
                    }
                }
            }

            if (CheckAgainstMapCollision)
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
                        CheckMapCollisionXY();
                        break;

                    case ActorMapCollisionType.CheckExtendedX:
                        CheckMapCollisionExtendedX();
                        break;

                    case ActorMapCollisionType.CheckExtendedY:
                        CheckMapCollisionExtendedY();
                        break;

                    case ActorMapCollisionType.CheckExtendedXY:
                        CheckMapCollisionExtendedXY();
                        break;
                }
            }

            // If on a linked object then we remove gravity
            if (LinkedMovementActor != null)
                Speed = Speed with { Y = 0 };
        }

        HasMoved = true;
    }

    public override void Step()
    {
        IsTouchingMap = false;
        HasMoved = false;
        base.Step();
    }

    public override void DrawDebugLayout(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        base.DrawDebugLayout(debugLayout, textureManager);

        ImGui.Text($"Speed: {Speed.X} x {Speed.Y}");
    }

    private enum Direction
    {
        Up,
        Down, 
        Left, 
        Right
    }
}