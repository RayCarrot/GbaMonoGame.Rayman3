using System;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

// BombeVolante
public sealed partial class FlyingBomb : MovableActor
{
    public FlyingBomb(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        SoundDelay = 0;
        Destroyed = false;

        Action firstActionId = (Action)actorResource.FirstActionId;

        // Hard-code an object to be removed. Odd that it's not a level change - maybe a last minute decision?
        if (GameInfo.MapId == MapId.SanctuaryOfStoneAndFire_M1)
        {
            if (InstanceId == 63)
                ProcessMessage(this, Message.Destroy);
        }
        else if (GameInfo.MapId == MapId.BeneathTheSanctuary_M1)
        {
            if (InstanceId == 68)
            {
                ActionId = Action.Action_5;
                firstActionId = Action.Action_5;
            }
        }

        if (firstActionId == Action.Action_4)
        {
            State.SetTo(FUN_10011270);
        }
        else if (firstActionId == Action.Action_5)
        {
            State.SetTo(FUN_10011168);
        }
        else
        {
            CurrentDirectionalType = firstActionId switch
            {
                Action.Move_Left => PhysicalTypeValue.Enemy_Left,
                Action.Move_Right => PhysicalTypeValue.Enemy_Right,
                Action.Move_Up => PhysicalTypeValue.Enemy_Up,
                Action.Move_Down => PhysicalTypeValue.Enemy_Down,
                _ => throw new Exception("Invalid flying bomb action")
            };
            State.SetTo(Fsm_Move);
        }
    }

    public PhysicalTypeValue? CurrentDirectionalType { get; set; } // Null is used for the boss machine level - games uses 0xFE
    public byte SoundDelay { get; set; }
    public bool Destroyed { get; set; }

    private bool HitWall()
    {
        Box vulnerabilityBox = GetVulnerabilityBox();

        vulnerabilityBox = new Box(
            minX: vulnerabilityBox.MinX - 8, 
            minY: vulnerabilityBox.MinY - 8, 
            maxX: vulnerabilityBox.MaxX + 8,
            maxY: vulnerabilityBox.MaxY - 8);

        // TODO: Have some helper which checks this? Might be done like this in other places too where it checks all around a box.

        // Check bottom-right
        PhysicalType type = Scene.GetPhysicalType(new Vector2(vulnerabilityBox.MaxX, vulnerabilityBox.MaxY));

        if (type.IsSolid || type == PhysicalTypeValue.MoltenLava)
            return true;

        // Check middle-right
        type = Scene.GetPhysicalType(new Vector2(vulnerabilityBox.MaxX, vulnerabilityBox.Center.Y));

        if (type.IsSolid || type == PhysicalTypeValue.MoltenLava)
            return true;

        // Check top-right
        type = Scene.GetPhysicalType(new Vector2(vulnerabilityBox.MaxX, vulnerabilityBox.MinY));

        if (type.IsSolid || type == PhysicalTypeValue.MoltenLava)
            return true;

        // Check top-middle
        type = Scene.GetPhysicalType(new Vector2(vulnerabilityBox.Center.X, vulnerabilityBox.MinY));

        if (type.IsSolid || type == PhysicalTypeValue.MoltenLava)
            return true;

        // Check top-left
        type = Scene.GetPhysicalType(new Vector2(vulnerabilityBox.MinX, vulnerabilityBox.MinY));

        if (type.IsSolid || type == PhysicalTypeValue.MoltenLava)
            return true;

        // Check middle-left
        type = Scene.GetPhysicalType(new Vector2(vulnerabilityBox.MinX, vulnerabilityBox.Center.Y));

        if (type.IsSolid || type == PhysicalTypeValue.MoltenLava)
            return true;

        // Check bottom-left
        type = Scene.GetPhysicalType(new Vector2(vulnerabilityBox.MinX, vulnerabilityBox.MaxY));

        if (type.IsSolid || type == PhysicalTypeValue.MoltenLava)
            return true;

        // Check bottom-middle
        type = Scene.GetPhysicalType(new Vector2(vulnerabilityBox.Center.X, vulnerabilityBox.MaxY));

        if (type.IsSolid || type == PhysicalTypeValue.MoltenLava)
            return true;

        return false;
    }

    public override void Step()
    {
        base.Step();
        GameInfo.ActorSoundFlags &= ~ActorSoundFlags.HelicopterBomb;
    }
}