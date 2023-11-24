using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Onyx.Gba;
using ImGuiNET;

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
    public bool HasMapCollision { get; set; }
    public bool HasObjectCollision { get; set; }

    private bool CheckObjectCollision1(Box actorDetectionBox, Box otherDetectionBox)
    {
        if (!actorDetectionBox.Intersects(otherDetectionBox))
            return false;

        // TODO: Modify position and speed

        return true;
    }

    private bool CheckObjectCollision2(Box actorDetectionBox, Box otherDetectionBox)
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

    private void CheckMapCollisionXY()
    {
        // TODO: Implement
    }

    private void CheckMapCollisionExtendedX()
    {
        // TODO: Implement
    }

    private void CheckMapCollisionExtendedY()
    {
        // TODO: Implement
    }

    private void CheckMapCollisionExtendedXY()
    {
        Scene2D scene = Frame.GetComponent<Scene2D>();
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
            PhysicalType type = scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MaxY - Constants.TileSize));

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
                type = scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MaxY));

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
                    type = scene.GetPhysicalType(new Vector2(detectionBox.MaxX, detectionBox.MaxY));

                    if (type.IsSolid)
                    {
                        PhysicalType otherType = scene.GetPhysicalType(new Vector2(detectionBox.MaxX, detectionBox.MaxY - Constants.TileSize));

                        if (otherType.IsSolid)
                            type = PhysicalTypeValue.None;

                        if (!type.IsSolid)
                        {
                            type = scene.GetPhysicalType(new Vector2(detectionBox.MinX, detectionBox.MaxY));

                            if (type.IsSolid)
                            {
                                otherType = scene.GetPhysicalType(new Vector2(detectionBox.MinX, detectionBox.MaxY - Constants.TileSize));

                                if (otherType.IsSolid)
                                    type = PhysicalTypeValue.None;
                            }
                        }
                    }
                    else
                    {
                        type = scene.GetPhysicalType(new Vector2(detectionBox.MinX, detectionBox.MaxY));

                        if (type.IsSolid)
                        {
                            PhysicalType otherType = scene.GetPhysicalType(new Vector2(detectionBox.MinX, detectionBox.MaxY - Constants.TileSize));

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

        // Moving to the right
        float x = Speed.X > 0 ? detectionBox.MaxX : detectionBox.MinX;

        // Get bottom-center type
        PhysicalType typeX = scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MaxY));

        if (typeX.IsAngledSolid)
            return;

        typeX = scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MaxY - Constants.TileSize));

        if (typeX.IsAngledSolid)
            return;

        typeX = scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MaxY + Constants.TileSize));

        if (typeX.IsAngledSolid)
            return;

        typeX = scene.GetPhysicalType(new Vector2(detectionBox.Center.X, detectionBox.MaxY + Constants.TileSize * 2));

        if (typeX.IsAngledSolid)
            return;

        if (Speed.Y > 0)
        {
            typeX = scene.GetPhysicalType(new Vector2(x, detectionBox.MaxY));
            if (typeX.IsFullySolid && typeX.Value != PhysicalTypeValue.Ledge && typeX.Value != PhysicalTypeValue.Passthrough)
            {
                if (Speed.X > 0)
                {
                    Speed -= new Vector2(MathHelpers.Mod(x, Constants.TileSize), 0);
                    Position -= new Vector2(MathHelpers.Mod(x, Constants.TileSize), 0);
                }
                else
                {
                    Speed += new Vector2(Constants.TileSize - MathHelpers.Mod(x, Constants.TileSize), 0);
                    Position += new Vector2(Constants.TileSize - MathHelpers.Mod(x, Constants.TileSize), 0);
                }
                IsTouchingMap = true;
                return;
            }
        }

        for (float i = detectionBox.MinY; i < detectionBox.MaxY; i += Constants.TileSize)
        {
            typeX = scene.GetPhysicalType(new Vector2(x, i));

            if (typeX.IsFullySolid && typeX.Value != PhysicalTypeValue.Ledge && typeX.Value != PhysicalTypeValue.Passthrough)
            {
                if (Speed.X > 0)
                {
                    Speed -= new Vector2(MathHelpers.Mod(x, Constants.TileSize), 0);
                    Position -= new Vector2(MathHelpers.Mod(x, Constants.TileSize), 0);
                }
                else
                {
                    Speed += new Vector2(Constants.TileSize - MathHelpers.Mod(x, Constants.TileSize), 0);
                    Position += new Vector2(Constants.TileSize - MathHelpers.Mod(x, Constants.TileSize), 0);
                }
                IsTouchingMap = true;
                return;
            }
        }
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

                Box detectionBox = GetDetectionBox();

                foreach (BaseActor actor in Frame.GetComponent<Scene2D>().GameObjects.EnumerateAllActors(isEnabled: true))
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

    public override void DrawDebugLayout(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        base.DrawDebugLayout(debugLayout, textureManager);

        ImGui.Text($"Speed: {Speed.X} x {Speed.Y}");
    }
}