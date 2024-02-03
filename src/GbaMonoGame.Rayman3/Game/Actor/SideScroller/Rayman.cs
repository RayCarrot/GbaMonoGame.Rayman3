using System;
using System.Collections.Generic;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;
using Microsoft.Xna.Framework.Input;

namespace GbaMonoGame.Rayman3;

// TODO: Move values, such as different speeds, to constants
public sealed partial class Rayman : MovableActor
{
    public Rayman(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        Resource = actorResource;

        IsLocalPlayer = true;

        // TODO: On N-Gage it loads a bunch of animated objects here for multiplayer - implement that?

        if (RSMultiplayer.IsActive)
        {
            if (instanceId >= RSMultiplayer.PlayersCount)
            {
                ProcessMessage(Message.Destroy);
            }
            else
            {
                if (instanceId != RSMultiplayer.MachineId)
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

    // TODO: Make properties private?
    public ActorResource Resource { get; }
    private Action? NextActionId { get; set; }
    public Dictionary<RaymanBody.RaymanBodyPartType, BaseActor> BodyParts { get; } = new(4); // Array with 4 entries in the game
    public BaseActor AttachedObject { get; set; }
    public byte Charge { get; set; }
    public byte HangOnEdgeDelay { get; set; }
    public uint Timer { get; set; }
    public uint InvulnerabilityStartTime { get; set; }
    public byte InvulnerabilityDuration { get; set; }
    public float PreviousXSpeed { get; set; }
    public PhysicalType? SlideType { get; set; } // Game uses 0x20 for null (first not solid type)
    public bool IsSliding => SlideType != null && Math.Abs(PreviousXSpeed) > 1.5f;
    public int PrevHitPoints { get; set; }
    public float PrevSpeedY { get; set; }

    public bool Debug_NoClip { get; set; } // Custom no-clip mode

    // Unknown flags 1
    public bool Flag1_0 { get; set; }
    public bool Flag1_1 { get; set; }
    public bool IsHanging { get; set; }
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
    public bool CanSafetyJump { get; set; } // Coyote jump
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
        if (!RSMultiplayer.IsActive)
        {
            return JoyPad.Check(input);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private bool CheckInput2(GbaInput input)
    {
        if (!RSMultiplayer.IsActive)
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
        if (!RSMultiplayer.IsActive)
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
        if (!RSMultiplayer.IsActive)
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
        if (!RSMultiplayer.IsActive)
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
        if (!RSMultiplayer.IsActive)
        {
            return !JoyPad.Check(input);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private bool CheckReleasedInput2(GbaInput input)
    {
        if (!RSMultiplayer.IsActive)
        {
            return !JoyPad.Check(input);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private void PlaySound(Rayman3SoundEvent soundEventId)
    {
        if (Scene.Camera.LinkedObject == this)
            SoundEventsManager.ProcessEvent(soundEventId);
    }

    private bool IsBossFight()
    {
        // This condition is probably a leftover from earlier versions of the game
        if (SoundEventsManager.IsPlaying(Rayman3SoundEvent.Play__lyfree))
            return false;

        return GameInfo.MapId is MapId.BossMachine or MapId.BossBadDreams or MapId.BossRockAndLava or MapId.BossScaleMan or MapId.BossFinal_M1;
    }

    private bool CanAttackWithFist(int punchCount)
    {
        if (RSMultiplayer.IsActive)
        {
            // TODO: Call FUN_0802aae4 to perform some multiplayer specific check
        }

        if (!BodyParts.ContainsKey(RaymanBody.RaymanBodyPartType.Fist))
            return true;

        if (!BodyParts.ContainsKey(RaymanBody.RaymanBodyPartType.SecondFist) && punchCount == 2 && HasPower(Power.DoubleFist))
            return true;

        return false;
    }

    private bool CanAttackWithFoot()
    {
        if (RSMultiplayer.IsActive)
        {
            // TODO: Call FUN_0802aae4 to perform some multiplayer specific check
        }

        return !BodyParts.ContainsKey(RaymanBody.RaymanBodyPartType.Foot);
    }

    private bool HasPower(Power power)
    {
        if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
        {
            throw new NotImplementedException();
        }
        else
        {
            return (GameInfo.Powers & power) != 0;
        }
    }

    private void Attack(uint chargePower, RaymanBody.RaymanBodyPartType type, Vector2 offset, bool hasCharged)
    {
        RaymanBody bodyPart = Scene.KnotManager.CreateProjectile<RaymanBody>(ActorType.RaymanBody);

        if (bodyPart == null)
            return;

        if (RSMultiplayer.IsActive)
        {
            // TODO: Implement
        }

        bodyPart.Rayman = this;
        bodyPart.BaseActionId = 0;

        switch (type)
        {
            case RaymanBody.RaymanBodyPartType.Fist:
                AnimatedObject.SetChannelInvisible(3);
                AnimatedObject.SetChannelInvisible(2);
                break;

            case RaymanBody.RaymanBodyPartType.SecondFist:
                AnimatedObject.SetChannelInvisible(16);
                AnimatedObject.SetChannelInvisible(15);
                break;

            case RaymanBody.RaymanBodyPartType.Foot:
                AnimatedObject.SetChannelInvisible(5);
                AnimatedObject.SetChannelInvisible(4);
                bodyPart.BaseActionId = 6;
                break;

            case RaymanBody.RaymanBodyPartType.Torso:
                AnimatedObject.SetChannelInvisible(12);
                AnimatedObject.SetChannelInvisible(11);
                bodyPart.BaseActionId = 12;
                break;

            case RaymanBody.RaymanBodyPartType.SuperFist:
                AnimatedObject.SetChannelInvisible(3);
                AnimatedObject.SetChannelInvisible(2);
                bodyPart.BaseActionId = 18;
                break;

            case RaymanBody.RaymanBodyPartType.SecondSuperFist:
                AnimatedObject.SetChannelInvisible(16);
                AnimatedObject.SetChannelInvisible(15);
                bodyPart.BaseActionId = 18;
                break;
        }

        bodyPart.BodyPartType = type;

        if (type == RaymanBody.RaymanBodyPartType.SuperFist)
        {
            BodyParts[RaymanBody.RaymanBodyPartType.Fist] = bodyPart;
            PlaySound(Rayman3SoundEvent.Play__SuprFist_Mix01);
        }
        else if (type == RaymanBody.RaymanBodyPartType.SecondSuperFist)
        {
            BodyParts[RaymanBody.RaymanBodyPartType.SecondFist] = bodyPart;
            PlaySound(Rayman3SoundEvent.Play__SuprFist_Mix01);
        }
        else
        {
            BodyParts[type] = bodyPart;

            if (type != RaymanBody.RaymanBodyPartType.Torso)
            {
                if (ActionId is Action.ChargeFist_Right or Action.ChargeFist_Left or Action.ChargeSecondFist_Right or Action.ChargeSecondFist_Left)
                    PlaySound(Rayman3SoundEvent.Play__RayFist_Mix02);
                else
                    PlaySound(Rayman3SoundEvent.Play__RayFist2_Mix01);
            }
        }

        bodyPart.ChargePower = chargePower;
        bodyPart.HasCharged = hasCharged;

        if (IsFacingLeft)
            offset *= new Vector2(-1, 1); // Flip x

        bodyPart.Position = Position + offset;

        bodyPart.ActionId = bodyPart.BaseActionId + (IsFacingRight ? 1 : 2);

        if (RSMultiplayer.IsActive && Engine.Settings.Platform == Platform.NGage)
        {
            // TODO: Implement
        }
    }

    private void CheckSlide()
    {
        Vector2 pos = Position;
        PhysicalType type = Scene.GetPhysicalType(pos);

        if (type.Value is 
            PhysicalTypeValue.Slide or PhysicalTypeValue.GrabSlide or 
            PhysicalTypeValue.SlideAngle30Left1 or PhysicalTypeValue.SlideAngle30Left2 or 
            PhysicalTypeValue.SlideAngle30Right1 or PhysicalTypeValue.SlideAngle30Right2)
        {
            if (type.Value is PhysicalTypeValue.Slide or PhysicalTypeValue.GrabSlide)
            {
                pos -= new Vector2(0, Constants.TileSize);
                PhysicalType type2 = Scene.GetPhysicalType(pos);

                if (type2.Value is 
                    PhysicalTypeValue.SlideAngle30Left1 or PhysicalTypeValue.SlideAngle30Left2 or
                    PhysicalTypeValue.SlideAngle30Right1 or PhysicalTypeValue.SlideAngle30Right2)
                    type = type2;
            }

            if (SlideType == null)
            {
                if (Speed.X == 0)
                    PreviousXSpeed = IsFacingRight ? 1 : -1;
                else
                    PreviousXSpeed = Speed.X;
            }

            SlideType = type;
        }
        else
        {
            SlideType = null;
            PlaySound(Rayman3SoundEvent.Stop__SkiLoop1);
        }

    }

    private void ManageSlide()
    {
        if (SlideType == null)
            return;

        MechModel.Speed = new Vector2(PreviousXSpeed, 5.62501525879f);

        if (CheckInput(GbaInput.Left))
        {
            if (PreviousXSpeed > -3)
                PreviousXSpeed -= 0.12109375f;
        }
        else if (CheckInput(GbaInput.Right))
        {
            if (PreviousXSpeed < 3)
                PreviousXSpeed += 0.12109375f;
        }
        else
        {
            if (PreviousXSpeed >= 0.05859375f)
            {
                PreviousXSpeed -= 0.015625f;
            }
            else if (PreviousXSpeed <= -0.05859375f)
            {
                PreviousXSpeed += 0.015625f;
            }
            else
            {
                PreviousXSpeed = 0;
            }
        }

        // Slippery
        if (SlideType?.Value is PhysicalTypeValue.SlideAngle30Left1 or PhysicalTypeValue.SlideAngle30Left2)
        {
            PreviousXSpeed -= 0.12109375f;

            if (CheckInput(GbaInput.Right))
                PreviousXSpeed -= 0.015625f;
        }
        else if (SlideType?.Value is PhysicalTypeValue.SlideAngle30Right1 or PhysicalTypeValue.SlideAngle30Right2)
        {
            PreviousXSpeed += 0.12109375f;

            if (CheckInput(GbaInput.Left))
                PreviousXSpeed += 0.015625f;
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

        MechModel.Speed = new Vector2(speedX, MechModel.Speed.Y);
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

    private void SlowdownAirSpeed()
    {
        if ((Speed.X > 0 && PreviousXSpeed < 0) ||
            (Speed.X < 0 && PreviousXSpeed > 0))
        {
            PreviousXSpeed = 0;
        }
        else if (PreviousXSpeed > 0)
        {
            PreviousXSpeed -= 0.03125f;

            if (PreviousXSpeed < 0)
                PreviousXSpeed = 0;
        }
        else if (PreviousXSpeed < 0)
        {
            PreviousXSpeed += 0.03125f;

            if (PreviousXSpeed > 0)
                PreviousXSpeed = 0;
        }
    }

    private void AttackInTheAir()
    {
        if (IsActionFinished && ActionId is 
                Action.BeginThrowFistInAir_Right or Action.BeginThrowFistInAir_Left or
                Action.BeginThrowSecondFistInAir_Right or Action.BeginThrowSecondFistInAir_Left or
                Action.Damage_Knockback_Right or Action.Damage_Knockback_Left)
        {
            ActionId = IsFacingRight ? Action.ThrowFistInAir_Right : Action.ThrowFistInAir_Left;
        }

        if (CheckSingleInput(GbaInput.B))
        {
            if (CanAttackWithFist(1))
            {
                ActionId = IsFacingRight ? Action.BeginThrowFistInAir_Right : Action.BeginThrowFistInAir_Left;
                Attack(15, RaymanBody.RaymanBodyPartType.Fist, new Vector2(16, -16), false);
            }
            else if (CanAttackWithFist(2))
            {
                ActionId = IsFacingRight ? Action.BeginThrowSecondFistInAir_Right : Action.BeginThrowSecondFistInAir_Left;
                Attack(15, RaymanBody.RaymanBodyPartType.SecondFist, new Vector2(16, -16), false);
            }
        }
    }

    private void CheckForTileDamage()
    {
        Box box = GetVulnerabilityBox();
        box = new Box(box.MinX, box.MinY, box.MaxX, box.MaxY - Constants.TileSize);

        if (Scene.GetPhysicalType(new Vector2(box.MaxX, box.MaxY)) == PhysicalTypeValue.Damage ||
            Scene.GetPhysicalType(new Vector2(box.MaxX, box.Center.Y)) == PhysicalTypeValue.Damage ||
            Scene.GetPhysicalType(new Vector2(box.MaxX, box.MinY)) == PhysicalTypeValue.Damage ||
            Scene.GetPhysicalType(new Vector2(box.Center.X, box.MinY)) == PhysicalTypeValue.Damage ||
            Scene.GetPhysicalType(new Vector2(box.MinX, box.MinY)) == PhysicalTypeValue.Damage ||
            Scene.GetPhysicalType(new Vector2(box.MinX, box.Center.Y)) == PhysicalTypeValue.Damage ||
            Scene.GetPhysicalType(new Vector2(box.MinX, box.MaxY)) == PhysicalTypeValue.Damage ||
            Scene.GetPhysicalType(new Vector2(box.Center.X, box.MaxY)) == PhysicalTypeValue.Damage)
        {
            ReceiveDamage(1);
        }
    }

    private bool ManageHit()
    {
        if (RSMultiplayer.IsActive || (GameInfo.Cheats & CheatFlags.Invulnerable) != 0)
            return false;

        CheckForTileDamage();

        bool takenDamage = false;

        // Check for taken damage
        if (HitPoints < PrevHitPoints)
        {
            IsInvulnerable = true;
            InvulnerabilityStartTime = GameTime.ElapsedFrames;

            if (InvulnerabilityDuration == 0)
                InvulnerabilityDuration = 120;

            if (AttachedObject != null)
            {
                // TODO: If keg, sphere or caterpillar then send message
                AttachedObject = null;
            }

            takenDamage = true;

            if (!SoundEventsManager.IsPlaying(Rayman3SoundEvent.Play__OnoRcvH1_Mix04))
                PlaySound(Rayman3SoundEvent.Play__OnoRcvH1_Mix04);
        }

        // Check for invulnerability to end
        if (IsInvulnerable && GameTime.ElapsedFrames - InvulnerabilityStartTime > InvulnerabilityDuration)
        {
            IsInvulnerable = false;
            InvulnerabilityDuration = 0;
        }

        PrevHitPoints = HitPoints;

        return takenDamage;
    }

    private bool FUN_0802a0f8()
    {
        // TODO: Implement
        return false;
    }

    private void SlidingOnSlippery()
    {
        if (!SoundEventsManager.IsPlaying(Rayman3SoundEvent.Play__SkiLoop1))
            PlaySound(Rayman3SoundEvent.Play__SkiLoop1);

        // TODO: Not on N-Gage
        SoundEventsManager.SetSoundPitch(Rayman3SoundEvent.Play__SkiLoop1, Math.Abs(Speed.X));

        if (PreviousXSpeed < -1.5f)
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
        if (HangOnEdgeDelay != 0)
        {
            HangOnEdgeDelay--;
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

                    if (type.Value is PhysicalTypeValue.Grab or PhysicalTypeValue.GrabSlide)
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

                    if (type.Value is PhysicalTypeValue.Grab or PhysicalTypeValue.GrabSlide)
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

    private bool IsOnHangable()
    {
        if (IsHanging)
            return true;

        if (Position.Y <= 48)
            return false;

        PhysicalType type = Scene.GetPhysicalType(Position - new Vector2(0, 56));

        return type == PhysicalTypeValue.Hang;
    }

    private void BeginHang()
    {
        if (IsHanging && AttachedObject != null)
        {
            Position = new Vector2(Position.X, AttachedObject.Position.Y + 58);
            AttachedObject = null;
        }
        else
        {
            Position = new Vector2(Position.X, Position.Y + Constants.TileSize - MathHelpers.Mod(Position.Y, Constants.TileSize) - 1);
            PlaySound(Rayman3SoundEvent.Play__HandTap1_Mix04);
        }
    }

    private void SetRandomIdleAction()
    {
        PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);

        int rand = Random.Shared.Next(41);

        if (IsBossFight())
        {
            NextActionId = IsFacingRight ? Action.Idle_Determined_Right : Action.Idle_Determined_Left;
        }
        else if (rand < 5 ||
                 (GameInfo.MapId == MapId.WoodLight_M1 && rand < 20 && GameInfo.LastGreenLumAlive != 0) ||
                 (GameInfo.MapId == MapId.WoodLight_M1 && GameInfo.LastGreenLumAlive == 0))
        {
            NextActionId = IsFacingRight ? Action.Idle_LookAround_Right : Action.Idle_LookAround_Left;
        }
        else if (rand < 10)
        {
            NextActionId = IsFacingRight ? Action.Idle_SpinBody_Right : Action.Idle_SpinBody_Left;
        }
        else if (rand < 15)
        {
            NextActionId = IsFacingRight ? Action.Idle_Bored_Right : Action.Idle_Bored_Left;
        }
        else if (rand < 20)
        {
            NextActionId = IsFacingRight ? Action.Idle_Yoyo_Right : Action.Idle_Yoyo_Left;
        }
        else if (rand < 25)
        {
            NextActionId = IsFacingRight ? Action.Idle_ChewingGum_Right : Action.Idle_ChewingGum_Left;
        }
        else if (rand < 30)
        {
            NextActionId = IsFacingRight ? Action.Idle_BasketBall_Right : Action.Idle_BasketBall_Left;
        }
        else if (rand < 35)
        {
            NextActionId = IsFacingRight ? Action.Idle_Grimace_Right : Action.Idle_Grimace_Left;
            PlaySound(Rayman3SoundEvent.Play__Grimace1_Mix04);
        }
        else
        {
            NextActionId = IsFacingRight ? Action.Idle_ThrowBody_Right : Action.Idle_ThrowBody_Left;
        }
    }

    private bool IsLavaInLevel()
    {
        return GameInfo.MapId switch
        {
            MapId.WoodLight_M1 => false,
            MapId.WoodLight_M2 => false,
            MapId.FairyGlade_M1 => false,
            MapId.FairyGlade_M2 => false,
            MapId.MarshAwakening1 => false,
            MapId.BossMachine => false,
            MapId.SanctuaryOfBigTree_M1 => false,
            MapId.SanctuaryOfBigTree_M2 => false,
            MapId.MissileSurPattes1 => false,
            MapId.EchoingCaves_M1 => false,
            MapId.EchoingCaves_M2 => false,
            MapId.CavesOfBadDreams_M1 => false,
            MapId.CavesOfBadDreams_M2 => false,
            MapId.BossBadDreams => false,
            MapId.MenhirHills_M1 => false,
            MapId.MenhirHills_M2 => false,
            MapId.MarshAwakening2 => false,

            MapId.SanctuaryOfStoneAndFire_M1 => true,
            MapId.SanctuaryOfStoneAndFire_M2 => true,
            MapId.SanctuaryOfStoneAndFire_M3 => true,
            MapId.BeneathTheSanctuary_M1 => true,
            MapId.BeneathTheSanctuary_M2 => true,

            MapId.ThePrecipice_M1 => false,
            MapId.ThePrecipice_M2 => false,
            MapId.BossRockAndLava => true,
            MapId.TheCanopy_M1 => false,
            MapId.TheCanopy_M2 => false,

            MapId.SanctuaryOfRockAndLava_M1 => true,
            MapId.SanctuaryOfRockAndLava_M2 => true,
            MapId.SanctuaryOfRockAndLava_M3 => true,

            MapId.TombOfTheAncients_M1 => false,
            MapId.TombOfTheAncients_M2 => false,
            MapId.BossScaleMan => false,

            MapId.IronMountains_M1 => true,
            MapId.IronMountains_M2 => true,
            MapId.MissileSurPattes2 => true,
            MapId.PirateShip_M1 => true,
            MapId.PirateShip_M2 => true,
            MapId.BossFinal_M1 => true,

            MapId.BossFinal_M2 => false,
            MapId.Bonus1 => false,
            MapId.Bonus2 => false,

            MapId.Bonus3 => true,

            MapId.Bonus4 => false,

            MapId._1000Lums => true,

            MapId.ChallengeLy1 => false,
            MapId.ChallengeLy2 => false,
            MapId.ChallengeLyGCN => false,
            MapId.Power1 => false,
            MapId.Power2 => false,
            MapId.Power3 => false,
            MapId.Power4 => false,
            MapId.Power5 => false,
            MapId.Power6 => false,
            MapId.World1 => false,
            MapId.World2 => false,
            MapId.World3 => false,
            MapId.World4 => false,
            MapId.WorldMap => false,

            MapId.GameCube_Bonus6 => true,

            _ => false
        };
    }

    private bool IsDead()
    {
        Box detectionBox = GetDetectionBox();

        PhysicalTypeValue type = PhysicalTypeValue.None;
        for (int i = 0; i < 3; i++)
        {
            type = Scene.GetPhysicalType(new Vector2(detectionBox.MinX + 16 * i, detectionBox.MaxY - 1));

            if (type is PhysicalTypeValue.InstaKill or PhysicalTypeValue.Lava or PhysicalTypeValue.Water or PhysicalTypeValue.MoltenLava)
                break;
        }

        if (HitPoints == 0 || type is PhysicalTypeValue.InstaKill or PhysicalTypeValue.Lava or PhysicalTypeValue.Water or PhysicalTypeValue.MoltenLava)
        {
            if (Fsm.EqualsAction(FUN_080284ac) && type is PhysicalTypeValue.InstaKill or PhysicalTypeValue.MoltenLava)
                return false;

            if (AttachedObject != null)
            {
                // TODO: If keg, sphere or caterpillar then send message
                AttachedObject = null;
            }

            // Handle drowning
            if (IsLavaInLevel() && type is PhysicalTypeValue.Lava or PhysicalTypeValue.MoltenLava)
            {
                LavaSplash lavaSplash = Scene.KnotManager.CreateProjectile<LavaSplash>(ActorType.LavaSplash);
                if (lavaSplash != null)
                {
                    lavaSplash.Position = Position;
                    lavaSplash.InitMainActorDrown();
                }

                ActionId = IsFacingRight ? Action.Drown_Right : Action.Drown_Left;
            }
            else if (type == PhysicalTypeValue.Water)
            {
                WaterSplash waterSplash = Scene.KnotManager.CreateProjectile<WaterSplash>(ActorType.WaterSplash);
                if (waterSplash != null)
                    waterSplash.Position = Position;

                ActionId = IsFacingRight ? Action.Drown_Right : Action.Drown_Left;
            }
            else if (type == PhysicalTypeValue.MoltenLava)
            {
                ActionId = IsFacingRight ? Action.Drown_Right : Action.Drown_Left;
            }

            return true;
        }
        else
        {
            return false;
        }
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
                MechModel.Speed = Vector2.Zero;
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
            case Message.RaymanBody_FinishedAttack:
                RaymanBody.RaymanBodyPartType bodyPartType = (RaymanBody.RaymanBodyPartType)param;
                BodyParts.Remove(bodyPartType);

                switch (bodyPartType)
                {
                    case RaymanBody.RaymanBodyPartType.Fist:
                    case RaymanBody.RaymanBodyPartType.SuperFist:
                        AnimatedObject.SetChannelVisible(3);
                        AnimatedObject.SetChannelVisible(2);
                        break;

                    case RaymanBody.RaymanBodyPartType.SecondFist:
                    case RaymanBody.RaymanBodyPartType.SecondSuperFist:
                        AnimatedObject.SetChannelVisible(16);
                        AnimatedObject.SetChannelVisible(15);
                        break;

                    case RaymanBody.RaymanBodyPartType.Foot:
                        AnimatedObject.SetChannelVisible(5);
                        AnimatedObject.SetChannelVisible(4);
                        break;

                    case RaymanBody.RaymanBodyPartType.Torso:
                        AnimatedObject.SetChannelVisible(12);
                        AnimatedObject.SetChannelVisible(11);
                        break;
                }
                return false;

            case Message.Main_LinkMovement:
                if (!Fsm.EqualsAction(Fsm_Dying))
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

            case Message.Main_CollectedCage:
                ((FrameSideScroller)Frame.Current).UserInfo.AddCages(1);
                return false;

            case Message.Main_AllowCoyoteJump:
                if (!Fsm.EqualsAction(Fsm_Jump) && !Fsm.EqualsAction(FUN_0802cb38))
                    CanSafetyJump = true;
                return false;

            default:
                return false;
        }
    }

    public override void Init()
    {
        AnimatedObject.YPriority = IsLocalPlayer ? 16 : 17;

        Timer = 0;
        InvulnerabilityStartTime = 0;
        //field11_0x88 = 0;
        NextActionId = null;
        BodyParts.Clear();
        Flag1_0 = false;
        Flag1_1 = false;
        IsHanging = false;
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
        PrevHitPoints = HitPoints;
        PrevSpeedY = 0;
        PreviousXSpeed = 0;
        SlideType = null;
        AttachedObject = null;
        field18_0x93 = 0;
        //field13_0x8c = 0;
        //field14_0x8e = 0;
        //field25_0x9a = HitPoints;
        field23_0x98 = 0;
        HangOnEdgeDelay = 0;
        InvulnerabilityDuration = 0;
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

        if (SlideType != null && NewAction)
            MechModel.Init(1, null);

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
                !RSMultiplayer.IsActive && 
                HitPoints != 0 && 
                (GameTime.ElapsedFrames & 1) == 0)
            {
                draw = false;
            }
        }

        if (draw)
        {
            AnimatedObject.IsFramed = true;
            animationPlayer.Play(AnimatedObject);
        }
        else
        {
            AnimatedObject.IsFramed = false;
            AnimatedObject.PlayChannelBox();
            AnimatedObject.ComputeNextFrame();
        }
    }
}