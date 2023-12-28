using System;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Onyx.Gba;
using BinarySerializer.Onyx.Gba.Rayman3;
using Microsoft.Xna.Framework.Input;
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public sealed partial class Rayman : MovableActor
{
    public Rayman(int id, Scene2D scene, ActorResource actorResource) : base(id, scene, actorResource)
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
    public BaseActor[] BodyParts { get; } = new BaseActor[4];
    public byte Charge { get; set; }
    public byte HangDelay { get; set; }
    public uint Timer { get; set; }
    public float MechSpeedX { get; set; } // TODO: SlidingSpeed?
    public byte PhysicalType { get; set; }
    public bool IsSliding => PhysicalType != 32 && Math.Abs(MechSpeedX) > 1.5f;
    public float PrevSpeedY { get; set; }

    public bool Debug_NoClip { get; set; } // Custom no-clip mode

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
    public bool FinishedMap { get; set; }
    public bool Flag1_F { get; set; }

    // Unknown flags 2
    public bool Flag2_0 { get; set; }
    public bool Flag2_1 { get; set; }
    public bool Flag2_2 { get; set; }
    public bool IsLocalPlayer { get; set; }
    public bool CanCoyoteJump { get; set; }
    public bool Flag2_5 { get; set; }
    public bool Flag2_6 { get; set; }
    public bool Flag2_7 { get; set; }

    // Unknown fields
    public byte field16_0x91 { get; set; }
    public byte field18_0x93 { get; set; }
    public byte field22_0x97 { get; set; }
    public byte field23_0x98 { get; set; }
    public byte field27_0x9c { get; set; } // Bool?

    // Disable collision when debug mode is on
    public override Box GetAttackBox() => Debug_NoClip ? Box.Empty : base.GetAttackBox();
    public override Box GetVulnerabilityBox() => Debug_NoClip ? Box.Empty : base.GetVulnerabilityBox();
    public override Box GetDetectionBox() => Debug_NoClip ? Box.Empty : base.GetDetectionBox();
    public override Box GetActionBox() => Debug_NoClip ? Box.Empty : base.GetActionBox();

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

    private bool CheckSingleInput2(GbaInput input)
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

    private bool CheckSingleReleasedInput(GbaInput input)
    {
        if (!MultiplayerManager.IsInMultiplayer)
        {
            return JoyPad.CheckSingleReleased(input);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private bool CheckReleasedInput(GbaInput input)
    {
        if (!MultiplayerManager.IsInMultiplayer)
        {
            return !JoyPad.Check(input);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private void PlaySoundEvent(Rayman3SoundEvent soundEventId)
    {
        if (Scene.Camera.LinkedObject == this)
            SoundManager.Play(soundEventId);
    }

    private bool IsBossFight()
    {
        // This condition is probably a leftover from earlier versions of the game
        if (SoundManager.IsPlaying(Rayman3SoundEvent.Play__lyfree))
            return false;

        return GameInfo.MapId is MapId.BossMachine or MapId.BossBadDreams or MapId.BossRockAndLava or MapId.BossScaleMan or MapId.BossFinal_M1;
    }

    private bool CanAttackWithFist(int punchCount)
    {
        if (MultiplayerManager.IsInMultiplayer)
        {
            // TODO: Call FUN_0802aae4 to perform some multiplayer specific check
        }

        if (BodyParts[0] == null)
            return true;

        if (BodyParts[1] == null && punchCount == 2 && (GameInfo.Powers & Power.DoubleFist) != 0)
            return true;

        return true;
    }

    private bool CanAttackWithFeet()
    {
        if (MultiplayerManager.IsInMultiplayer)
        {
            // TODO: Call FUN_0802aae4 to perform some multiplayer specific check
        }

        return BodyParts[2] == null;
    }

    private void Attack(int charge, byte type, Vector2 offset, byte param5)
    {
        RaymanBody bodyPart = (RaymanBody)Scene.GameObjects.SpawnActor(ActorType.RaymanBody);

        if (bodyPart == null)
            return;

        if (MultiplayerManager.IsInMultiplayer)
        {
            // TODO: Implement
        }

        bodyPart.Rayman = this;
        bodyPart.field4_0x66 = 0;

        switch (type)
        {
            case 0:
                // TODO: Hide sprites 3 and 2
                break;

            case 1:
                // TODO: Hide sprites 16 and 15
                break;

            case 2:
                // TODO: Hide sprites 5 and 4
                bodyPart.field4_0x66 = 6;
                break;

            case 3:
                // TODO: Hide sprites 12 and 11
                bodyPart.field4_0x66 = 12;
                break;

            case 5:
                // TODO: Hide sprites 3 and 2
                bodyPart.field4_0x66 = 18;
                break;

            case 6:
                // TODO: Hide sprites 16 and 15
                bodyPart.field4_0x66 = 18;
                break;
        }

        bodyPart.BodyPartType = type;

        if (type == 5)
        {
            BodyParts[0] = bodyPart;
            PlaySoundEvent(Rayman3SoundEvent.Play__SuprFist_Mix01);
        }
        else if (type == 6)
        {
            BodyParts[1] = bodyPart;
            PlaySoundEvent(Rayman3SoundEvent.Play__SuprFist_Mix01);
        }
        else
        {
            BodyParts[type] = bodyPart;

            if (type != 3)
            {
                if (ActionId is Action.ChargeFist_Right or Action.ChargeFist_Left or Action.ChargeFistVariant_Right or Action.ChargeFistVariant_Left)
                    PlaySoundEvent(Rayman3SoundEvent.Play__RayFist_Mix02);
                else
                    PlaySoundEvent(Rayman3SoundEvent.Play__RayFist2_Mix01);
            }
        }

        bodyPart.Charge = charge;
        bodyPart.field2_0x64 = param5; // TODO: A boolean?

        if (IsFacingLeft)
            offset *= new Vector2(-1, 1); // Flip x

        bodyPart.Position = Position + offset;

        bodyPart.ActionId = bodyPart.field4_0x66 + (IsFacingRight ? 1 : 2);

        if (MultiplayerManager.IsInMultiplayer && Engine.Settings.Platform == Platform.NGage)
        {
            // TODO: Implement
        }
    }

    private void UpdatePhysicalType()
    {
        PhysicalType type = Scene.GetPhysicalType(Position);

        if (type.Value is PhysicalTypeValue.Slippery or PhysicalTypeValue.SlipperyLedge)
        {
            PhysicalType otherType = Scene.GetPhysicalType(Position - new Vector2(0, Constants.TileSize));

            if (otherType.Value is PhysicalTypeValue.Solid or PhysicalTypeValue.Slippery or PhysicalTypeValue.Ledge or PhysicalTypeValue.SlipperyLedge)
                type = otherType;
        }
        else if (type.Value is not (PhysicalTypeValue.SlipperyAngle30Right1 or PhysicalTypeValue.SlipperyAngle30Right2 or PhysicalTypeValue.SlipperyAngle30Left2 or PhysicalTypeValue.SlipperyAngle30Left1))
        {
            PhysicalType = 32;
            PlaySoundEvent(Rayman3SoundEvent.Stop__SkiLoop1);
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

    private void MoveInTheAir(float speedX)
    {
        if (CheckInput(GbaInput.Left))
        {
            if (IsFacingRight)
                AnimatedObject.FlipX = true;

            speedX -= 1.79998779297f;

            if (speedX <= -3)
                speedX = -3;
        }
        else if (CheckInput(GbaInput.Right))
        {
            if (IsFacingLeft)
                AnimatedObject.FlipX = false;

            speedX += 1.79998779297f;

            if (speedX > 3)
                speedX = 3;
        }

        Mechanic.Speed = new Vector2(speedX, Mechanic.Speed.Y);
    }

    private bool HasLanded()
    {
        if (Speed.Y != 0 || PrevSpeedY < 0)
        {
            PrevSpeedY = Speed.Y;
            return false;
        }

        Box detectionBox = GetDetectionBox();
        
        // Check bottom right
        PhysicalType type = Scene.GetPhysicalType(new Vector2(detectionBox.MaxX, detectionBox.MaxY));
        if (type.IsSolid)
        {
            PrevSpeedY = 0;
            return true;
        }

        // Check if on another actor
        if (LinkedMovementActor != null)
        {
            PrevSpeedY = 0;
            return true;
        }

        // Check bottom left
        type = Scene.GetPhysicalType(new Vector2(detectionBox.MinX, detectionBox.MaxY));
        if (type.IsSolid)
        {
            PrevSpeedY = 0;
            return true;
        }

        // Check bottom middle
        type = Scene.GetPhysicalType(new Vector2(detectionBox.MaxX - detectionBox.Width / 2, detectionBox.MaxY));
        if (type.IsSolid)
        {
            PrevSpeedY = 0;
            return true;
        }

        return false;
    }

    private void FUN_0802c3c8()
    {
        if ((Speed.X <= 0 || MechSpeedX >= 0) &&
            (Speed.X >= 0 || MechSpeedX <= 0))
        {
            if (MechSpeedX <= 0)
            {
                if (MechSpeedX >= 0)
                    return;

                MechSpeedX += 0.03125f;

                if (MechSpeedX <= 0)
                    return;
            }
            else
            {
                MechSpeedX -= 0.03125f;

                if (MechSpeedX >= 0)
                    return;
            }
        }

        MechSpeedX = 0;
    }

    private void AttackInTheAir()
    {
        // TODO: Implement
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
        if (!SoundManager.IsPlaying(Rayman3SoundEvent.Play__SkiLoop1) && Scene.Camera.LinkedObject == this)
            SoundManager.Play(Rayman3SoundEvent.Play__SkiLoop1);

        SoundManager.FUN_080ac468(Rayman3SoundEvent.Play__SkiLoop1, Math.Abs(Speed.X));

        if (MechSpeedX < -1.5f)
        {
            if (IsFacingRight)
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
            if (IsFacingRight)
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

    // 0 = false, 1 = right, 2 = left
    private int IsNearEdge()
    {
        Box detectionBox = GetDetectionBox();
        
        PhysicalType centerType = Scene.GetPhysicalType(Position);
        PhysicalType rightType = Scene.GetPhysicalType(new Vector2(detectionBox.MaxX, detectionBox.MaxY));
        PhysicalType leftType = Scene.GetPhysicalType(new Vector2(detectionBox.MinX, detectionBox.MaxY));

        if (centerType.IsSolid)
            return 0;
        else if (leftType.IsSolid)
            return 1;
        else if (rightType.IsSolid)
            return 2;
        else
            return 0;
    }

    private bool IsNearHangableEdge()
    {
        if (HangDelay != 0)
        {
            HangDelay--;
            return false;
        }

        if (Position.Y <= 40)
            return false;

        Vector2 pos = Position - new Vector2(0, 40);

        if (IsFacingRight)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    PhysicalType type = Scene.GetPhysicalType(pos);

                    if (type.Value is PhysicalTypeValue.Ledge or PhysicalTypeValue.SlipperyLedge)
                    {
                        // Get tile to the left of the ledge
                        type = Scene.GetPhysicalType(pos - new Vector2(Constants.TileSize, 0));
                        
                        // Make sure it's not solid
                        if (!type.IsSolid)
                        {
                            Position = new Vector2(
                                x: pos.X - MathHelpers.Mod(pos.X, Constants.TileSize) - 17, 
                                y: Position.Y - MathHelpers.Mod(Position.Y, Constants.TileSize) + y * Constants.TileSize);
                            return true;
                        }
                    }

                    pos += new Vector2(Constants.TileSize, 0);
                }

                pos += new Vector2(0, Constants.TileSize);
                pos = new Vector2(Position.X, pos.Y);
            }
        }
        else
        {
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    PhysicalType type = Scene.GetPhysicalType(pos);

                    if (type.Value is PhysicalTypeValue.Ledge or PhysicalTypeValue.SlipperyLedge)
                    {
                        // Get tile to the right of the ledge
                        type = Scene.GetPhysicalType(pos + new Vector2(Constants.TileSize, 0));

                        // Make sure it's not solid
                        if (!type.IsSolid)
                        {
                            Position = new Vector2(
                                x: pos.X - MathHelpers.Mod(pos.X, Constants.TileSize) + 24,
                                y: Position.Y - MathHelpers.Mod(Position.Y, Constants.TileSize) + y * Constants.TileSize);
                            return true;
                        }
                    }

                    pos -= new Vector2(Constants.TileSize, 0);
                }

                pos += new Vector2(0, Constants.TileSize);
                pos = new Vector2(Position.X, pos.Y);
            }
        }

        return false;
    }

    // 0 = false, 1 = top and bottom, 2 = top, 3 = bottom
    private int IsOnClimbableVertical()
    {
        Vector2 pos = Position;

        if (pos.Y <= 48)
            return 0;

        pos -= new Vector2(0, 24);
        PhysicalType bottomType = Scene.GetPhysicalType(pos);

        if (bottomType.Value is 
            PhysicalTypeValue.ClimbSpider1 or 
            PhysicalTypeValue.ClimbSpider2 or 
            PhysicalTypeValue.ClimbSpider3 or 
            PhysicalTypeValue.ClimbSpider4)
        {
            bottomType = PhysicalTypeValue.Climb;
        }

        pos -= new Vector2(0, 24);
        PhysicalType topType = Scene.GetPhysicalType(pos);

        if (topType.Value is
            PhysicalTypeValue.ClimbSpider1 or
            PhysicalTypeValue.ClimbSpider2 or
            PhysicalTypeValue.ClimbSpider3 or
            PhysicalTypeValue.ClimbSpider4)
        {
            topType = PhysicalTypeValue.Climb;
        }

        if (bottomType == PhysicalTypeValue.Climb && topType == PhysicalTypeValue.Climb)
            return 1;

        if (bottomType == PhysicalTypeValue.Climb)
            return 3;

        if (topType == PhysicalTypeValue.Climb)
            return 2;

        return 0;
    }

    // 0 = false, 4 = right and left, 5 = right, 6 = left
    private int IsOnClimbableHorizontal()
    {
        Vector2 pos = Position;

        if (pos.Y <= 48)
            return 0;

        pos -= new Vector2(8, 40);
        PhysicalType leftType = Scene.GetPhysicalType(pos);

        if (leftType.Value is
            PhysicalTypeValue.ClimbSpider1 or
            PhysicalTypeValue.ClimbSpider2 or
            PhysicalTypeValue.ClimbSpider3 or
            PhysicalTypeValue.ClimbSpider4)
        {
            leftType = PhysicalTypeValue.Climb;
        }

        pos += new Vector2(16, 0);
        PhysicalType rightType = Scene.GetPhysicalType(pos);

        if (rightType.Value is
            PhysicalTypeValue.ClimbSpider1 or
            PhysicalTypeValue.ClimbSpider2 or
            PhysicalTypeValue.ClimbSpider3 or
            PhysicalTypeValue.ClimbSpider4)
        {
            rightType = PhysicalTypeValue.Climb;
        }

        if (leftType == PhysicalTypeValue.Climb && rightType == PhysicalTypeValue.Climb)
            return 4;

        if (leftType == PhysicalTypeValue.Climb)
            return 6;

        if (rightType == PhysicalTypeValue.Climb)
            return 5;

        return 0;
    }

    private void FUN_0802a65c()
    {
        // TODO: Implement
    }

    private bool IsOnInstaKillType()
    {
        Box detectionBox = GetDetectionBox();

        byte type = 0xFF;
        for (int i = 0; i < 3; i++)
        {
            type = Scene.GetPhysicalType(new Vector2(detectionBox.MinX + 16 * i, detectionBox.MaxY - 1));

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

    private void UpdateLastCompletedLevel()
    {
        if (GameInfo.MapId < MapId.Bonus1 && GameInfo.MapId > (MapId)GameInfo.PersistentInfo.LastCompletedLevel)
            GameInfo.PersistentInfo.LastCompletedLevel = (byte)GameInfo.MapId;
    }

    private void AutoSave()
    {
        switch (GameInfo.MapId)
        {
            case MapId.WoodLight_M1:
            case MapId.FairyGlade_M1:
            case MapId.SanctuaryOfBigTree_M1:
            case MapId.EchoingCaves_M1:
            case MapId.CavesOfBadDreams_M1:
            case MapId.MenhirHills_M1:
            case MapId.SanctuaryOfStoneAndFire_M1:
            case MapId.SanctuaryOfStoneAndFire_M2:
            case MapId.BeneathTheSanctuary_M1:
            case MapId.ThePrecipice_M1:
            case MapId.TheCanopy_M1:
            case MapId.SanctuaryOfRockAndLava_M1:
            case MapId.SanctuaryOfRockAndLava_M2:
            case MapId.TombOfTheAncients_M1:
            case MapId.IronMountains_M1:
            case MapId.PirateShip_M1:
            case MapId.BossFinal_M1:
                return;
        }

        if (GameInfo.MapId is not (MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4 or MapId.WorldMap))
        {
            GameInfo.PersistentInfo.LastPlayedLevel = (byte)GameInfo.MapId;
            GameInfo.Save(GameInfo.CurrentSlot);
        }
    }

    private void ToggleNoClip()
    {
        if (JoyPad.CheckSingle(Keys.Z)) // TODO: Do not hard-code this key
        {
            Debug_NoClip = !Debug_NoClip;

            if (Debug_NoClip)
            {
                ActionId = IsFacingRight ? Action.Idle_Right : Action.Idle_Left;
                ChangeAction();
                Mechanic.Speed = Vector2.Zero;
            }
            else
            {
                ActionId = IsFacingRight ? Action.Fall_Right : Action.Fall_Left;
                Fsm.ChangeAction(Fsm_Fall);
                ChangeAction();
            }
        }
    }

    private void DoNoClipBehavior()
    {
        int speed = JoyPad.Check(GbaInput.A) ? 7 : 4;

        if (JoyPad.Check(GbaInput.Up))
            Position -= new Vector2(0, speed);
        else if (JoyPad.Check(GbaInput.Down))
            Position += new Vector2(0, speed);

        if (JoyPad.Check(GbaInput.Left))
            Position -= new Vector2(speed, 0);
        else if (JoyPad.Check(GbaInput.Right))
            Position += new Vector2(speed, 0);
    }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        if (base.ProcessMessageImpl(message, param))
            return false;

        // TODO: Implement remaining messages
        switch (message)
        {
            case Message.Main_LinkMovement:
                if (!Fsm.EqualsAction(FUN_08032650))
                {
                    if (Fsm.EqualsAction(Fsm_Jump) && Speed.Y < 1)
                        return false;

                    MovableActor actorToLink = ((MovableActor)param);
                    Box actorToLinkBox = actorToLink.GetDetectionBox();

                    if (Position.Y < actorToLinkBox.MinY + 7)
                    {
                        LinkedMovementActor = actorToLink;
                        Position = new Vector2(Position.X, actorToLinkBox.MinY);
                    }

                    if (Fsm.EqualsAction(Fsm_HangOnEdge))
                        Fsm.ChangeAction(Fsm_Default);
                }
                return false;

            case Message.Main_UnlinkMovement:
                LinkedMovementActor = null;
                Flag2_2 = true;
                return false;

            case Message.Main_CollectedYellowLum:
                ((FrameSideScroller)Frame.Current).UserInfo.AddLums(1);
                return false;

            case Message.Main_CollectedRedLum:
                // TODO: Implement
                return false;

            case Message.Main_CollectedBlueLum:
                // TODO: Implement
                return false;

            case Message.Main_CollectedWhiteLum:
                // TODO: Implement
                return false;

            case Message.Main_CollectedBigYellowLum:
                ((FrameSideScroller)Frame.Current).UserInfo.AddLums(10);
                return false;

            case Message.Main_CollectedBigBlueLum:
                // TODO: Implement
                return false;

            case Message.Main_LevelEnd:
                FinishedMap = true;
                Fsm.ChangeAction(Fsm_EndMap);
                return false;

            case Message.Main_LevelExit:
                Fsm.ChangeAction(Fsm_EndMap);
                return false;

            case Message.Main_AllowCoyoteJump:
                if (!Fsm.EqualsAction(Fsm_Jump) && !Fsm.EqualsAction(FUN_0802cb38))
                    CanCoyoteJump = true;
                return false;

            default:
                return false;
        }
    }

    public override void Init()
    {
        AnimatedObject.YPriority = IsLocalPlayer ? 16 : 17;

        Timer = 0;
        //field10_0x84 = 0;
        //field11_0x88 = 0;
        NextActionId = null;
        Array.Clear(BodyParts);
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
        FinishedMap = false;
        Flag1_F = false;
        //field17_0x92 = HitPoints;
        PrevSpeedY = 0;
        MechSpeedX = 0;
        PhysicalType = 32;
        //field7_0x78 = 0;
        field18_0x93 = 0;
        //field13_0x8c = 0;
        //field14_0x8e = 0;
        //field25_0x9a = HitPoints;
        field23_0x98 = 0;
        HangDelay = 0;
        //field_0x99 = 0;
        Charge = 0;
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

    public override void DoBehavior()
    {
        if (Debug_NoClip)
            DoNoClipBehavior();
        else
            base.DoBehavior();
    }

    public override void Step()
    {
        base.Step();

        ToggleNoClip();

        if (PhysicalType != 32 && NewAction)
            Mechanic.Init(1, null);

        // TODO: Implement
    }

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        CameraActor camera = Scene.Camera;

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
            animationPlayer.AddSortedObject(AnimatedObject);
        }
        else
        {
            AnimatedObject.ExecuteUnframed();
        }
    }
}