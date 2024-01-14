using System;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Ubisoft.GbaEngine;
using ImGuiNET;

namespace GbaMonoGame.Engine2d;

public class MovableActor : InteractableActor
{
    public MovableActor(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        MapCollisionType = actorResource.Model.MapCollisionType;
        HasMapCollision = actorResource.Model.HasMapCollision;
        HasObjectCollision = actorResource.Model.HasObjectCollision;

        HasMoved = false;
    }

    public MechModel MechModel { get; } = new();
    public MovableActor LinkedMovementActor { get; set; }
    public Vector2 Speed { get; set; }

    // Flags
    public ActorMapCollisionType MapCollisionType { get; }
    public bool HasMapCollision { get; set; }
    public bool HasObjectCollision { get; set; }

    private bool CheckObjectCollision1(Box actorDetectionBox, Box otherDetectionBox)
    {
        // TODO: The game removes the decimals in the commented out code - should we? I probably haven't done it
        //       elsewhere since the game goes between int and fixed-point a bunch.

        Box intersectBox = Box.Intersect(actorDetectionBox, otherDetectionBox);

        if (intersectBox == Box.Empty)
            return false;

        float width = intersectBox.Width;
        float height = intersectBox.Height;

        if (Speed.Y >= 1 &&
            otherDetectionBox.MinY > actorDetectionBox.MinY &&
            otherDetectionBox.MaxY > actorDetectionBox.MaxY &&
            height < Constants.TileSize)
        {
            Speed -= new Vector2(0, height);
            Position -= new Vector2(0, height);

            if (Speed.Y < 0)
                Speed += new Vector2(0, 1);

            // Speed = new Vector2(Speed.X, (int)Speed.Y);

            if (Position.Y < 0)
                Position += new Vector2(0, 1);

            // Position = new Vector2(Position.X, (int)Position.Y);
            return true;
        }

        if (Speed.Y < 0 &&
            actorDetectionBox.MaxY > otherDetectionBox.MaxY &&
            actorDetectionBox.MinY > otherDetectionBox.MinY)
        {
            Speed += new Vector2(0, height);
            Position += new Vector2(0, height);

            if (Speed.Y < 0)
                Speed += new Vector2(0, 1);

            // Speed = new Vector2(Speed.X, (int)Speed.Y);

            if (Position.Y < 0)
                Position += new Vector2(0, 1);

            // Position = new Vector2(Position.X, (int)Position.Y);
            return true;
        }

        if (Speed.X < 1)
        {
            if (-1 < Speed.X)
                return true;

            if (actorDetectionBox.MinX <= otherDetectionBox.MinX)
                return true;

            Speed += new Vector2(width, 0);
            Position += new Vector2(width, 0);

            if (Speed.X < 0)
                Speed += new Vector2(1, 0);

            // Speed = new Vector2((int)Speed.X, Speed.Y);

            if (Position.X < 0)
                Position += new Vector2(1, 0);

            // Position = new Vector2((int)Position.X, Position.Y);
            return true;
        }
        else
        {
            if (otherDetectionBox.MaxX <= actorDetectionBox.MaxX)
                return true;

            Speed -= new Vector2(width, 0);
            Position -= new Vector2(width, 0);

            if (Speed.X < 0)
                Speed += new Vector2(1, 0);

            // Speed = new Vector2((int)Speed.X, Speed.Y);

            if (Position.X < 0)
                Position += new Vector2(1, 0);

            // Position = new Vector2((int)Position.X, Position.Y);
            return true;
        }
    }

    private bool CheckObjectCollision2(Box actorDetectionBox, Box otherDetectionBox)
    {
        if (!actorDetectionBox.Intersects(otherDetectionBox))
            return false;

        // TODO: Modify position and speed
        throw new NotImplementedException();

        return true;
    }

    private void CheckMapCollisionX()
    {
        throw new NotImplementedException();
    }

    private void CheckMapCollisionY()
    {
        throw new NotImplementedException();
    }

    private void CheckMapCollisionXY()
    {
        Box detectionBox = GetDetectionBox();

        if (Speed.Y < 0)
        {
            // Get the top-center type
            PhysicalType type = Scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MinY));

            if (type.IsFullySolid && type != PhysicalTypeValue.Grab && type != PhysicalTypeValue.Passthrough)
            {
                Speed += new Vector2(0, Constants.TileSize - MathHelpers.Mod(detectionBox.MinY, Constants.TileSize));
                Position += new Vector2(0, Constants.TileSize - MathHelpers.Mod(detectionBox.MinY, Constants.TileSize));
                IsTouchingMap = true;
            }
        }
        else
        {
            // Get the bottom-center type
            PhysicalType type = Scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MaxY));

            if (type.IsSolid)
            {
                Speed -= new Vector2(0, MathHelpers.Mod(detectionBox.MaxY, Constants.TileSize));
                Position -= new Vector2(0, MathHelpers.Mod(detectionBox.MaxY, Constants.TileSize));
                IsTouchingMap = true;
            }
        }

        if (Speed.X < 0)
        {
            // Get the left-center type
            PhysicalType type = Scene.GetPhysicalType(new Vector2(detectionBox.MinX, detectionBox.Center.Y));

            if (type.IsSolid && type != PhysicalTypeValue.Grab && type != PhysicalTypeValue.Passthrough)
            {
                Speed += new Vector2(Constants.TileSize - MathHelpers.Mod(detectionBox.MinX, Constants.TileSize), 0);
                Position += new Vector2(Constants.TileSize - MathHelpers.Mod(detectionBox.MinX, Constants.TileSize), 0);
                IsTouchingMap = true;
            }
        }
        else
        {
            // Get the right-center type
            PhysicalType type = Scene.GetPhysicalType(new Vector2(detectionBox.MaxX, detectionBox.Center.Y));

            if (type.IsSolid && type != PhysicalTypeValue.Grab && type != PhysicalTypeValue.Passthrough)
            {
                Speed -= new Vector2(MathHelpers.Mod(detectionBox.MaxX, Constants.TileSize), 0);
                Position -= new Vector2(MathHelpers.Mod(detectionBox.MaxX, Constants.TileSize), 0);
                IsTouchingMap = true;
            }
        }
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

        // The code below is very hard to read in the game's decompiled code, so there might be minor mistakes. Ideally this
        // should also be cleaned up so it becomes more readable since it's very confusing right now.

        // Moving up
        if (Speed.Y < 0)
        {
            // TODO: Implement
        }
        // Moving down or not moving vertically
        else
        {
            // Get bottom-center type
            PhysicalType type = Scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MaxY - Constants.TileSize));

            if (type.IsAngledSolid)
            {
                float tileHeight = type.GetAngleHeight(detectionBox.Center.X);

                if (Speed.Y == 0 && tileHeight != 0)
                    Position -= new Vector2(0, tileHeight);
                else
                    Position -= new Vector2(0, tileHeight + MathHelpers.Mod(detectionBox.MaxY, Constants.TileSize));

                Speed = new Vector2(Speed.X, 0);
                IsTouchingMap = true;
            }
            else
            {
                type = Scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MaxY));

                if (type.IsFullySolid)
                {
                    Position -= new Vector2(0, MathHelpers.Mod(detectionBox.MaxY, Constants.TileSize));
                    Speed = new Vector2(Speed.X, 0);
                    IsTouchingMap = true;
                }
                else if (type.IsAngledSolid)
                {
                    float tileHeight = Constants.TileSize - type.GetAngleHeight(detectionBox.Center.X);

                    if (Speed.Y == 0)
                    {
                        Position += new Vector2(0, tileHeight - MathHelpers.Mod(detectionBox.MaxY, Constants.TileSize));
                        Speed = new Vector2(Speed.X, 0);
                        IsTouchingMap = true;
                    }
                    else if (MathHelpers.Mod(detectionBox.MaxY, Constants.TileSize) > tileHeight)
                    {
                        Position -= new Vector2(0, MathHelpers.Mod(detectionBox.MaxY, Constants.TileSize) - tileHeight);
                        Speed = new Vector2(Speed.X, 0);
                        IsTouchingMap = true;
                    }
                }
                else
                {
                    type = Scene.GetPhysicalType(new Vector2(detectionBox.MaxX, detectionBox.MaxY));

                    if (type.IsSolid)
                    {
                        PhysicalType otherType = Scene.GetPhysicalType(new Vector2(detectionBox.MaxX, detectionBox.MaxY - Constants.TileSize));

                        if (otherType.IsSolid)
                            type = PhysicalTypeValue.None;

                        if (!type.IsSolid)
                        {
                            type = Scene.GetPhysicalType(new Vector2(detectionBox.MinX, detectionBox.MaxY));

                            if (type.IsSolid)
                            {
                                otherType = Scene.GetPhysicalType(new Vector2(detectionBox.MinX, detectionBox.MaxY - Constants.TileSize));

                                if (otherType.IsSolid)
                                    type = PhysicalTypeValue.None;
                            }
                        }
                    }
                    else
                    {
                        type = Scene.GetPhysicalType(new Vector2(detectionBox.MinX, detectionBox.MaxY));

                        if (type.IsSolid)
                        {
                            PhysicalType otherType = Scene.GetPhysicalType(new Vector2(detectionBox.MinX, detectionBox.MaxY - Constants.TileSize));

                            if (otherType.IsSolid)
                                type = PhysicalTypeValue.None;
                        }
                    }

                    if (type.IsFullySolid)
                    {
                        Position -= new Vector2(0, MathHelpers.Mod(detectionBox.MaxY, Constants.TileSize));
                        Speed = new Vector2(Speed.X, 0);
                        IsTouchingMap = true;   
                    }
                }
            }
        }

        if (Speed.X == 0)
            return;

        // Get the x position to detect
        float detectionX = Speed.X > 0 ? detectionBox.MaxX : detectionBox.MinX;

        // Get bottom-center type
        PhysicalType typeX = Scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MaxY));

        if (typeX.IsAngledSolid)
            return;

        typeX = Scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MaxY - Constants.TileSize));

        if (typeX.IsAngledSolid)
            return;

        typeX = Scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MaxY + Constants.TileSize));

        if (typeX.IsAngledSolid)
            return;

        typeX = Scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MaxY + Constants.TileSize * 2));

        if (typeX.IsAngledSolid)
            return;

        // Not moving down
        if (Speed.Y <= 0)
        {
            float y = detectionBox.MinY + Constants.TileSize;
            int count = (int)(detectionBox.Height / Constants.TileSize) - 1;

            for (int i = 0; i < count; i++)
            {
                typeX = Scene.GetPhysicalType(new Vector2(detectionX, y));
                y += Constants.TileSize;

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
                float y = detectionBox.MinY + Constants.TileSize;
                int count = (int)(detectionBox.Height / Constants.TileSize) - 1;

                for (int i = 0; i < count; i++)
                {
                    typeX = Scene.GetPhysicalType(new Vector2(detectionX, y));
                    y += Constants.TileSize;

                    if (typeX.IsFullySolid && typeX.Value != PhysicalTypeValue.Grab && typeX.Value != PhysicalTypeValue.Passthrough)
                        break;
                }
            }
        }

        if (typeX.IsFullySolid && typeX.Value != PhysicalTypeValue.Grab && typeX.Value != PhysicalTypeValue.Passthrough)
        {
            if (Speed.X > 0)
            {
                Speed -= new Vector2(MathHelpers.Mod(detectionX, Constants.TileSize), 0);
                Position -= new Vector2(MathHelpers.Mod(detectionX, Constants.TileSize), 0);
            }
            else
            {
                Speed += new Vector2(Constants.TileSize - MathHelpers.Mod(detectionX, Constants.TileSize), 0);
                Position += new Vector2(Constants.TileSize - MathHelpers.Mod(detectionX, Constants.TileSize), 0);
            }
            IsTouchingMap = true;
        }
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

            if (HasObjectCollision)
            {
                IsTouchingActor = false;

                Box detectionBox = GetDetectionBox();

                foreach (BaseActor actor in Scene.KnotManager.EnumerateAllActors(isEnabled: true))
                {
                    if (actor != this && actor.ActorFlag_6 && actor is ActionActor actionActor)
                    {
                        Box otherDetectionBox = actionActor.GetDetectionBox();

                        if (!actionActor.ActorFlag_E)
                        {
                            if (CheckObjectCollision1(detectionBox, otherDetectionBox))
                            {
                                IsTouchingActor = true;
                                detectionBox = GetDetectionBox();
                            }
                        }
                        else
                        {
                            if (CheckObjectCollision2(detectionBox, otherDetectionBox))
                            {
                                IsTouchingActor = true;
                                detectionBox = GetDetectionBox();
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
                Speed = new Vector2(Speed.X, 0);
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
}