using System;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

// TODO: Create enum for actions?
public partial class Rayman : MovableActor
{
    public Rayman(int id, ActorResource actorResource) : base(id, actorResource)
    {
        Resource = actorResource;

        IsLocalPlayer = true;

        if (MultiplayerManager.IsInMultiplayer)
        {
            if (id >= MultiplayerManager.Data.Count)
            {
                SendMessage(Message.Destroy);
            }
            else
            {
                if (id != MultiplayerManager.Data.MachineId)
                {
                    IsLocalPlayer = false;
                    AnimatedObject.IsSoundEnabled = false;
                }

                // TODO: Load special palettes if id is 1, 2 or 3

                IsInvulnerable = true;

                // TODO: Enable all powers
            }
        }

        Fsm.ChangeAction(Fsm_LevelStart);
    }

    public ActorResource Resource { get; }
    public int NextActionId { get; set; }
    public uint Timer { get; set; }
    public float MechSpeedX { get; set; }
    public ushort RaymanFlags { get; set; }

    // Multiplayer flags TODO: Probably not multiplayer flags after all
    public bool MultiplayerFlag_0 { get; set; }
    public bool MultiplayerFlag_1 { get; set; }
    public bool MultiplayerFlag_2 { get; set; }
    public bool IsLocalPlayer { get; set; }
    public bool MultiplayerFlag_4 { get; set; }
    public bool MultiplayerFlag_5 { get; set; }
    public bool MultiplayerFlag_6 { get; set; }
    public bool MultiplayerFlag_7 { get; set; }

    // Unknown
    public byte field16_0x91 { get; set; }
    public byte field18_0x93 { get; set; }
    public byte field19_0x94 { get; set; }
    public byte field23_0x98 { get; set; }
    public byte field27_0x9c { get; set; }

    private bool CheckInput(GbaInput input)
    {
        if (!MultiplayerManager.IsInMultiplayer)
        {
            return JoyPad.Check(input);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private bool CheckSingleInput(GbaInput input)
    {
        if (!MultiplayerManager.IsInMultiplayer)
        {
            return JoyPad.CheckSingle(input);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private bool FUN_0802c53c()
    {
        if (SoundManager.IsPlaying(194))
            return false;

        return GameInfo.MapId is MapId.BossMachine or MapId.BossBadDreams or MapId.BossRockAndLava or MapId.BossScaleMan or MapId.BossFinal_M1;
    }

    private void FUN_08029ec4()
    {
        Scene2D scene = Frame.GetComponent<Scene2D>();
        byte type = scene.GetPhysicalType(Position);

        if (type != 1 && type != 3)
        {
            if (type is not (22 or 23 or 24 or 25))
            {
                field19_0x94 = 32;

                if (scene.Camera.LinkedObject == this)
                    SoundManager.Play(208, -1);

                return;
            }
        }
        else
        {
            byte otherType = scene.GetPhysicalType(Position - new Vector2(0, Constants.TileSize));

            if (otherType < 4)
                type = otherType;
        }

        if (field19_0x94 == 32)
        {
            float speedX = Speed.X;

            if (speedX == 0)
                speedX = IsFacingLeft ? -1 : 1;

            MechSpeedX = speedX;
        }

        field19_0x94 = type;
    }

    // Maybe this is only for slippery movement?
    private void HorizontalMovement()
    {
        if (field19_0x94 == 32)
            return;

        Mechanic.Speed = new Vector2(MechSpeedX, 5.62501525879f);

        if (CheckInput(GbaInput.Left))
        {
            if (MechSpeedX > -3)
                MechSpeedX -= 0.12109375f;
        }
        else if (CheckInput(GbaInput.Right))
        {
            if (MechSpeedX < 3)
                MechSpeedX += 0.12109375f;
        }
        else
        {
            if (MechSpeedX >= 0.05859375f)
            {
                MechSpeedX -= 0.015625f;
            }
            else if (MechSpeedX <= -0.05859375f)
            {
                MechSpeedX += 0.015625f;
            }
            else
            {
                MechSpeedX = 0;
            }
        }

        // Slippery
        if (field19_0x94 is 22 or 23)
        {
            MechSpeedX -= 0.12109375f;

            if (CheckInput(GbaInput.Right))
                MechSpeedX -= 0.015625f;
        }
        else if (field19_0x94 is 24 or 25)
        {
            MechSpeedX += 0.12109375f;

            if (CheckInput(GbaInput.Left))
                MechSpeedX += 0.015625f;
        }
    }

    private bool CheckForDamage()
    {
        if (MultiplayerManager.IsInMultiplayer || (GameInfo.Cheats & CheatFlags.Invulnerable) != 0)
            return false;

        // TODO: Implement

        return false;
    }

    private bool FUN_0802a0f8()
    {
        // TODO: Implement
        return false;
    }

    private void FUN_0802a2dc()
    {
        if (!SoundManager.IsPlaying(181) && Frame.GetComponent<Scene2D>().Camera.LinkedObject == this)
            SoundManager.Play(181, -1);

        SoundManager.FUN_080ac468(181, Math.Abs(Speed.X));

        if (MechSpeedX < -1.5f)
        {
            if (!IsFacingLeft)
            {
                if (CheckInput(GbaInput.Down))
                {
                    if (ActionId != 76)
                        ActionId = 76;
                }
                else
                {
                    if (ActionId != 74)
                        ActionId = 74;
                }
            }
            else
            {
                if (CheckInput(GbaInput.Down))
                {
                    if (ActionId != 77)
                        ActionId = 77;
                }
                else
                {
                    if (ActionId != 67)
                        ActionId = 67;
                }
            }
        }
        else
        {
            if (!IsFacingLeft)
            {
                if (CheckInput(GbaInput.Down))
                {
                    if (ActionId != 76)
                        ActionId = 76;
                }
                else
                {
                    if (ActionId != 66)
                        ActionId = 66;
                }
            }
            else
            {
                if (CheckInput(GbaInput.Down))
                {
                    if (ActionId != 77)
                        ActionId = 77;
                }
                else
                {
                    if (ActionId != 75)
                        ActionId = 75;
                }
            }
        }
    }

    private bool FUN_0802c414(int unknown)
    {
        return false;

        // TODO: Implement
        //if ((!MultiplayerManager.IsInMultiplayer || FUN_0802aae4()) &&
        //    (field3_0x68 == 0 || (field4_0x6c == 0 && unknown == 2 && (GameInfo.Powers & 1) != 0)))
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
    }

    private int FUN_0802986c()
    {
        // TODO: Implement
        return 0;
    }

    private void FUN_0802a65c()
    {
        // TODO: Implement
    }

    private bool IsOnInstaKillType()
    {
        Scene2D scene = Frame.GetComponent<Scene2D>();
        Rectangle detectionBox = GetAbsoluteBox(DetectionBox);

        byte type = 0xFF;
        for (int i = 0; i < 3; i++)
        {
            type = scene.GetPhysicalType(new Vector2(detectionBox.Left + 16 * i, detectionBox.Bottom - 1));

            if (type is 32 or 74 or 48 or 90)
                break;
        }

        if ((HitPoints != 0 && type is not (32 or 74 or 48 or 90)) ||
            (Fsm.EqualsAction(FUN_080284ac) && (type is 32 or 90)))
        {
            return false;
        }

        throw new NotImplementedException();
    }

    protected override bool ProcessMessage(Message message, object param)
    {
        if (base.ProcessMessage(message, param))
            return true;

        switch (message)
        {
            // TODO: Implement

            default:
                return false;
        }
    }

    public override void Init()
    {
        // TODO: Implement
        Timer = 0;
        //field10_0x84 = 0;
        //field11_0x88 = 0;
        NextActionId = -1;
        //field3_0x68 = 0;
        //field4_0x6c = 0;
        //field5_0x70 = 0;
        //field6_0x74 = 0;
        RaymanFlags = 0;
        //field17_0x92 = HitPoints;
        //field1_0x60 = 0;
        MechSpeedX = 0;
        field19_0x94 = 32;
        //field7_0x78 = 0;
        field18_0x93 = 0;
        //field13_0x8c = 0;
        //field14_0x8e = 0;
        //field25_0x9a = HitPoints;
        field23_0x98 = 0;
        //field_0x90 = 0;
        //field_0x99 = 0;
        //field_0x96 = 0;
        //field_0x97 = 0;

        MultiplayerFlag_0 = false;
        field27_0x9c = 0;
        MultiplayerFlag_1 = false;
        MultiplayerFlag_2 = true;

        HasMapCollision = true;
        HasObjectCollision = true;

        ActionId = Resource.FirstActionId;
        ChangeAction();

        if (GameInfo.LastGreenLumAlive == 0)
        {
            // TODO: Implement
        }
        else
        {
            Position = GameInfo.CheckpointPosition;
            ActionId = 0;
            ChangeAction();
        }

        GameInfo.field12_0xf = 0;
        // TODO: Run cheats
    }

    public override void Step()
    {
        base.Step();

        if (field19_0x94 != 0x20 && NewAction)
            Mechanic.Init(1, null);

        // TODO: Implement
    }

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        CameraActor camera = Frame.GetComponent<Scene2D>().Camera;

        bool draw = camera.IsActorFramed(this) || forceDraw;

        // Conditionally don't draw every second frame during invulnerability
        if (draw)
        {
            if (IsInvulnerable && 
                (GameInfo.Cheats & CheatFlags.Invulnerable) == 0 && 
                !MultiplayerManager.IsInMultiplayer && 
                HitPoints != 0 && 
                (GameTime.ElapsedFrames & 1) == 0)
            {
                draw = false;
            }
        }

        if (draw)
        {
            animationPlayer.AddSecondaryObject(AnimatedObject);
        }
        else
        {
            AnimatedObject.ExecuteUnframed();
        }
    }
}