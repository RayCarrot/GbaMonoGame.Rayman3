using System;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public sealed partial class Rayman : MovableActor
{
    public Rayman(int id, ActorResource actorResource) : base(id, actorResource)
    {
        Resource = actorResource;

        IsLocalPlayer = true;

        if (MultiplayerManager.IsInMultiplayer)
        {
            if (id >= MultiplayerManager.Data.Count)
            {
                ProcessMessage(Message.Destroy);
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
    private Action? NextActionId { get; set; }
    public uint Timer { get; set; }
    public float MechSpeedX { get; set; }
    public byte PhysicalType { get; set; } // 32 seems to be mean it's slippery?

    // Unknown flags 1
    public bool Flag1_0 { get; set; }
    public bool Flag1_1 { get; set; }
    public bool Flag1_2 { get; set; }
    public bool Flag1_3 { get; set; }
    public bool Flag1_4 { get; set; }
    public bool Flag1_5 { get; set; }
    public bool Flag1_6 { get; set; }
    public bool Flag1_7 { get; set; }
    public bool Flag1_8 { get; set; }
    public bool Flag1_9 { get; set; }
    public bool Flag1_A { get; set; }
    public bool Flag1_B { get; set; }
    public bool Flag1_C { get; set; }
    public bool Flag1_D { get; set; }
    public bool Flag1_E { get; set; }
    public bool Flag1_F { get; set; }

    // Unknown flags 2
    public bool Flag2_0 { get; set; }
    public bool Flag2_1 { get; set; }
    public bool Flag2_2 { get; set; }
    public bool IsLocalPlayer { get; set; }
    public bool Flag2_4 { get; set; }
    public bool Flag2_5 { get; set; }
    public bool Flag2_6 { get; set; }
    public bool Flag2_7 { get; set; }

    // Unknown fields
    public byte field16_0x91 { get; set; }
    public byte field18_0x93 { get; set; }
    public byte field21_0x96 { get; set; }
    public byte field22_0x97 { get; set; }
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

    private void PlaySound(int id)
    {
        if (Frame.GetComponent<Scene2D>().Camera.LinkedObject == this)
            SoundManager.Play(id, -1);
    }

    private bool IsBossFight()
    {
        if (SoundManager.IsPlaying(194))
            return false;

        return GameInfo.MapId is MapId.BossMachine or MapId.BossBadDreams or MapId.BossRockAndLava or MapId.BossScaleMan or MapId.BossFinal_M1;
    }

    private void UpdatePhysicalType()
    {
        Scene2D scene = Frame.GetComponent<Scene2D>();
        byte type = scene.GetPhysicalType(Position);

        if (type is 1 or 3)
        {
            byte otherType = scene.GetPhysicalType(Position - new Vector2(0, Constants.TileSize));

            if (otherType < 4)
                type = otherType;
        }
        else if (type is not (22 or 23 or 24 or 25))
        {
            PhysicalType = 32;

            if (scene.Camera.LinkedObject == this)
                SoundManager.Play(208, -1);

            return;
        }

        if (PhysicalType == 32)
        {
            float speedX = Speed.X;

            if (speedX == 0)
                speedX = IsFacingRight ? 1 : -1;

            MechSpeedX = speedX;
        }

        PhysicalType = type;
    }

    private void HorizontalMovement()
    {
        if (PhysicalType == 32)
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
        if (PhysicalType is 22 or 23)
        {
            MechSpeedX -= 0.12109375f;

            if (CheckInput(GbaInput.Right))
                MechSpeedX -= 0.015625f;
        }
        else if (PhysicalType is 24 or 25)
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

    private void SlidingOnSlippery()
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
                    if (ActionId != Action.Sliding_Crouch_Right)
                        ActionId = Action.Sliding_Crouch_Right;
                }
                else
                {
                    if (ActionId != Action.Sliding_Slow_Right)
                        ActionId = Action.Sliding_Slow_Right;
                }
            }
            else
            {
                if (CheckInput(GbaInput.Down))
                {
                    if (ActionId != Action.Sliding_Crouch_Left)
                        ActionId = Action.Sliding_Crouch_Left;
                }
                else
                {
                    if (ActionId != Action.Sliding_Fast_Left)
                        ActionId = Action.Sliding_Fast_Left;
                }
            }
        }
        else
        {
            if (!IsFacingLeft)
            {
                if (CheckInput(GbaInput.Down))
                {
                    if (ActionId != Action.Sliding_Crouch_Right)
                        ActionId = Action.Sliding_Crouch_Right;
                }
                else
                {
                    if (ActionId != Action.Sliding_Fast_Right)
                        ActionId = Action.Sliding_Fast_Right;
                }
            }
            else
            {
                if (CheckInput(GbaInput.Down))
                {
                    if (ActionId != Action.Sliding_Crouch_Left)
                        ActionId = Action.Sliding_Crouch_Left;
                }
                else
                {
                    if (ActionId != Action.Sliding_Slow_Left)
                        ActionId = Action.Sliding_Slow_Left;
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

    // Checks if near edge?
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

    private bool Inlined_FUN_1004c544()
    {
        if (!Inlined_FUN_1004c1f4())
            return false;

        UpdatePhysicalType();
        HorizontalMovement();

        if (CheckForDamage())
        {
            Fsm.ChangeAction(FUN_08031d24);
            return false;
        }

        if (CheckSingleInput(GbaInput.A) && PhysicalType != 32)
        {
            Fsm.ChangeAction(FUN_0802cb38);
            return false;
        }
        else if (FUN_0802a0f8())
        {
            PlaySound(208);

            Fsm.ChangeAction(FUN_0802cb38);
            return false;
        }

        return true;
    }

    private bool Inlined_FUN_1004c1f4()
    {
        Scene2D scene = Frame.GetComponent<Scene2D>();
        CameraSideScroller cam = (CameraSideScroller)scene.Camera;

        if (Flag1_D)
        {
            field16_0x91++;

            if (field16_0x91 > 60)
            {
                cam.HorizontalOffset = Gfx.Platform switch
                {
                    Platform.GBA => 40,
                    Platform.NGage => 25,
                    _ => throw new UnsupportedPlatformException()
                };
                Flag1_D = false;
            }
        }

        if (IsLocalPlayer &&
            !Fsm.EqualsAction(Fsm_Jump) &&
            !Fsm.EqualsAction(FUN_0802ce54) &&
            !Fsm.EqualsAction(FUN_080284ac) &&
            !Fsm.EqualsAction(FUN_08033b34) &&
            !Fsm.EqualsAction(FUN_080287d8) &&
            !Flag1_6)
        {
            // TODO: Name camera messages
            Message message;

            if (!Fsm.EqualsAction(FUN_0802ddac) &&
                CheckInput(GbaInput.Down) &&
                (Speed.Y > 0 || Fsm.EqualsAction(FUN_080254d8)) &&
                !Fsm.EqualsAction(FUN_08026cd4))
            {
                field18_0x93 = 70;
                message = (Message)1040;
            }
            else if (CheckInput(GbaInput.Up) && (Fsm.EqualsAction(Fsm_Default) || Fsm.EqualsAction(FUN_0802ea74)))
            {
                field18_0x93 = 160;
                message = (Message)1040;
            }
            else if (Fsm.EqualsAction(FUN_0802d44c) && field27_0x9c == 0)
            {
                message = (Message)1039;
            }
            else if (Fsm.EqualsAction(FUN_0803283c))
            {
                field18_0x93 = 65;
                message = (Message)1040;
            }
            else if (Fsm.EqualsAction(FUN_08026cd4) || Fsm.EqualsAction(FUN_0802ddac))
            {
                field18_0x93 = 112;
                message = (Message)1040;
            }
            else
            {
                field18_0x93 = 120;
                message = (Message)1040;
            }

            cam.ProcessMessage(message, field18_0x93);
        }

        if (field23_0x98 != 0)
            field23_0x98--;

        if (IsOnInstaKillType())
        {
            if (!MultiplayerManager.IsInMultiplayer)
                Fsm.ChangeAction(FUN_08032650);
            else
                Fsm.ChangeAction(FUN_08033228);

            return false;
        }

        return true;
    }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        if (base.ProcessMessageImpl(message, param))
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
        NextActionId = null;
        //field3_0x68 = 0;
        //field4_0x6c = 0;
        //field5_0x70 = 0;
        //field6_0x74 = 0;
        Flag1_0 = false;
        Flag1_1 = false;
        Flag1_2 = false;
        Flag1_3 = false;
        Flag1_4 = false;
        Flag1_5 = false;
        Flag1_6 = false;
        Flag1_7 = false;
        Flag1_8 = false;
        Flag1_9 = false;
        Flag1_A = false;
        Flag1_B = false;
        Flag1_C = false;
        Flag1_D = false;
        Flag1_E = false;
        Flag1_F = false;
        //field17_0x92 = HitPoints;
        //field1_0x60 = 0;
        MechSpeedX = 0;
        PhysicalType = 32;
        //field7_0x78 = 0;
        field18_0x93 = 0;
        //field13_0x8c = 0;
        //field14_0x8e = 0;
        //field25_0x9a = HitPoints;
        field23_0x98 = 0;
        //field_0x90 = 0;
        //field_0x99 = 0;
        field21_0x96 = 0;
        field22_0x97 = 0;

        Flag2_0 = false;
        field27_0x9c = 0;
        Flag2_1 = false;
        Flag2_2 = true;

        HasMapCollision = true;
        HasObjectCollision = true;

        ActionId = (Action)Resource.FirstActionId;
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

        if (PhysicalType != 32 && NewAction)
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