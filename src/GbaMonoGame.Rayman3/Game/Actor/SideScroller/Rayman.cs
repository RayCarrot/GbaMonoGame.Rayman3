﻿using System;
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

        if (RSMultiplayer.IsActive)
        {
            if (Engine.Settings.Platform == Platform.NGage)
            {
                MultiplayerData = new NGageMultiplayerData()
                {
                    field_b8 = 1,
                };
            }

            if (instanceId >= RSMultiplayer.PlayersCount)
            {
                ProcessMessage(this, Message.Destroy);
            }
            else
            {
                if (instanceId != RSMultiplayer.MachineId)
                {
                    IsLocalPlayer = false;
                    AnimatedObject.IsSoundEnabled = false;
                }

                if (Engine.Settings.Platform == Platform.NGage &&
                    MultiplayerInfo.GameType == MultiplayerGameType.CaptureTheFlag &&
                    MultiplayerInfo.CaptureTheFlag_field_04 != 0)
                {
                    // TODO: Implement setting palettes
                }
                else
                {
                    // This is some hacky code to add the additional multiplayer palettes. The game doesn't store this
                    // in the animated object resource to avoid them being allocated in single player. So the game
                    // manually allocates them to vram here. We however can't just modify this actor's animations since
                    // we cache sprites between all actors that share the same animated object. So the easiest solution
                    // is to add the palettes to the animated object resource and then just change the base pal index.
                    if (AnimatedObject.Resource.PalettesCount == 2)
                    {
                        Palette16 pal2 = Storage.LoadResource<Resource<Palette16>>(GameResource.Player2RaymanPalette).Value;
                        Palette16 pal3 = Storage.LoadResource<Resource<Palette16>>(GameResource.Player3RaymanPalette).Value;
                        Palette16 pal4 = Storage.LoadResource<Resource<Palette16>>(GameResource.Player4RaymanPalette).Value;

                        AnimatedObject.Resource.PalettesCount = 2 * 4;
                        AnimatedObject.Resource.Palettes = new SpritePalettes
                        {
                            Palettes = new[]
                            {
                                AnimatedObject.Resource.Palettes.Palettes[0],
                                AnimatedObject.Resource.Palettes.Palettes[1],

                                AnimatedObject.Resource.Palettes.Palettes[0],
                                pal2,

                                AnimatedObject.Resource.Palettes.Palettes[0],
                                pal3,

                                AnimatedObject.Resource.Palettes.Palettes[0],
                                pal4,
                            }
                        };
                    }

                    AnimatedObject.BasePaletteIndex = InstanceId * 2;
                }

                if (Engine.Settings.Platform == Platform.NGage &&
                    MultiplayerInfo.GameType == MultiplayerGameType.CaptureTheFlag &&
                    IsLocalPlayer)
                {
                    AnimatedObjectResource arrowResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.CaptureTheFlagArrowAnimations);

                    for (int i = 0; i < RSMultiplayer.PlayersCount - 1; i++)
                    {
                        MultiplayerData!.FlagArrows[i] = new AnimatedObject(arrowResource, arrowResource.IsDynamic)
                        {
                            IsFramed = true,
                            SpritePriority = 0,
                            YPriority = 2,
                            CurrentAnimation = 1,
                            AffineMatrix = AffineMatrix.Identity,
                        };
                    }
                }

                IsInvulnerable = true;
                SetPowers(Power.All);
            }
        }

        State.SetTo(Fsm_LevelStart);
    }

    public ActorResource Resource { get; }
    public NGageMultiplayerData MultiplayerData { get; }
    public Action? NextActionId { get; set; }
    public RaymanBody[] ActiveBodyParts { get; } = new RaymanBody[4];
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
    public int CameraTargetY { get; set; }
    public byte ReverseControlsTimer { get; set; }

    public bool Debug_NoClip { get; set; } // Custom no-clip mode

    // TODO: Name flags
    // Unknown flags 1
    public bool Flag1_0 { get; set; }
    public bool Flag1_1 { get; set; }
    public bool IsHanging { get; set; }
    public bool Flag1_3 { get; set; }
    public bool Flag1_4 { get; set; }
    public bool Flag1_5 { get; set; }
    public bool IsInFrontOfLevelCurtain { get; set; }
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
    public bool Flag2_1 { get; set; } // TODO: Seems to be some general purpose flag. Name something like "TestFlag" or "TempFlag"?
    public bool Flag2_2 { get; set; }
    public bool IsLocalPlayer { get; set; }
    public bool CanSafetyJump { get; set; } // Coyote jump
    public bool Flag2_5 { get; set; }
    public bool Flag2_6 { get; set; }
    public bool Flag2_7 { get; set; }

    // Unknown fields
    public byte field16_0x91 { get; set; }
    public byte field22_0x97 { get; set; }
    public byte field23_0x98 { get; set; }
    public byte field27_0x9c { get; set; } // Bool?

    // TODO: Maybe we should just make the state methods public to avoid this?
    public bool IsInDefaultState => State == Fsm_Default;
    public bool IsInCutsceneState => State == Fsm_Cutscene;
    public bool IsInDyingState => State == Fsm_Dying;
    public bool IsInEndMapState => State == Fsm_EndMap;

    // Disable collision when debug mode is on
    public override Box GetAttackBox() => Debug_NoClip ? Box.Empty : base.GetAttackBox();
    public override Box GetVulnerabilityBox() => Debug_NoClip ? Box.Empty : base.GetVulnerabilityBox();
    public override Box GetDetectionBox() => Debug_NoClip ? Box.Empty : base.GetDetectionBox();
    public override Box GetActionBox() => Debug_NoClip ? Box.Empty : base.GetActionBox();

    private void EnableCheats()
    {
        for (int i = 0; i < 3; i++)
        {
            Cheat cheat = (Cheat)(1 << i);

            if ((GameInfo.Cheats & cheat) != 0)
                GameInfo.EnableCheat(Scene, cheat);
        }
    }

    private GbaInput ReverseControls(GbaInput input) => 
        (GbaInput)((input & (GbaInput.Left | GbaInput.Down)) != 0 ? (ushort)input >> 1 : (ushort)input << 1);

    private bool IsDirectionalButtonPressed(GbaInput input)
    {
        if (RSMultiplayer.IsActive)
        {
            if (ReverseControlsTimer != 0)
                input = ReverseControls(input);

            SimpleJoyPad joyPad = MultiJoyPad.GetSimpleJoyPadForCurrentFrame(InstanceId);
            return joyPad.IsButtonPressed(input);
        }
        else
        {
            return JoyPad.IsButtonPressed(input);
        }
    }

    private bool IsDirectionalButtonButtonReleased(GbaInput input)
    {
        if (RSMultiplayer.IsActive)
        {
            if (ReverseControlsTimer != 0)
                input = ReverseControls(input);

            return MultiJoyPad.GetSimpleJoyPadForCurrentFrame(InstanceId).IsButtonReleased(input);
        }
        else
        {
            return JoyPad.IsButtonReleased(input);
        }
    }

    private bool IsDirectionalButtonJustPressed(GbaInput input)
    {
        if (RSMultiplayer.IsActive)
        {
            if (ReverseControlsTimer != 0)
                input = ReverseControls(input);

            return MultiJoyPad.GetSimpleJoyPadForCurrentFrame(InstanceId).IsButtonJustPressed(input);
        }
        else
        {
            return JoyPad.IsButtonJustPressed(input);
        }
    }

    private void PlaySound(Rayman3SoundEvent soundEventId)
    {
        if (Scene.Camera.LinkedObject == this)
            SoundEventsManager.ProcessEvent(soundEventId);
    }

    private bool IsBossFight()
    {
        // Ly levels use the map id of the previous map, so don't count this as a boss fight then
        if (SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__lyfree))
            return false;

        return GameInfo.MapId is MapId.BossMachine or MapId.BossBadDreams or MapId.BossRockAndLava or MapId.BossScaleMan or MapId.BossFinal_M1;
    }

    private bool CanAttackWithFist(int punchCount)
    {
        if (RSMultiplayer.IsActive)
        {
            // TODO: Call FUN_0802aae4 to perform some multiplayer specific check
        }

        if (ActiveBodyParts[(int)RaymanBody.RaymanBodyPartType.Fist] == null)
            return true;

        if (ActiveBodyParts[(int)RaymanBody.RaymanBodyPartType.SecondFist] == null && punchCount == 2 && HasPower(Power.DoubleFist))
            return true;

        return false;
    }

    private bool CanAttackWithFoot()
    {
        if (RSMultiplayer.IsActive)
        {
            // TODO: Call FUN_0802aae4 to perform some multiplayer specific check
        }

        return ActiveBodyParts[(int)RaymanBody.RaymanBodyPartType.Foot] == null;
    }

    private bool CanAttackWithBody()
    {
        if (RSMultiplayer.IsActive)
        {
            // TODO: Call FUN_0802aae4 to perform some multiplayer specific check
        }

        return ActiveBodyParts[(int)RaymanBody.RaymanBodyPartType.Torso] == null;
    }

    private bool HasPower(Power power)
    {
        if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive && MultiplayerInfo.GameType == MultiplayerGameType.CaptureTheFlag)
        {
            return (MultiplayerData.Powers & power) != 0;
        }
        else
        {
            return (GameInfo.Powers & power) != 0;
        }
    }

    private void Attack(uint chargePower, RaymanBody.RaymanBodyPartType type, Vector2 offset, bool hasCharged)
    {
        RaymanBody bodyPart = Scene.CreateProjectile<RaymanBody>(ActorType.RaymanBody);

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
            ActiveBodyParts[(int)RaymanBody.RaymanBodyPartType.Fist] = bodyPart;
            PlaySound(Rayman3SoundEvent.Play__SuprFist_Mix01);
        }
        else if (type == RaymanBody.RaymanBodyPartType.SecondSuperFist)
        {
            ActiveBodyParts[(int)RaymanBody.RaymanBodyPartType.SecondFist] = bodyPart;
            PlaySound(Rayman3SoundEvent.Play__SuprFist_Mix01);
        }
        else
        {
            ActiveBodyParts[(int)type] = bodyPart;

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
                pos -= new Vector2(0, Tile.Size);
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

        if (IsDirectionalButtonPressed(GbaInput.Left))
        {
            if (PreviousXSpeed > -3)
                PreviousXSpeed -= 0.12109375f;
        }
        else if (IsDirectionalButtonPressed(GbaInput.Right))
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

            if (IsDirectionalButtonPressed(GbaInput.Right))
                PreviousXSpeed -= 0.015625f;
        }
        else if (SlideType?.Value is PhysicalTypeValue.SlideAngle30Right1 or PhysicalTypeValue.SlideAngle30Right2)
        {
            PreviousXSpeed += 0.12109375f;

            if (IsDirectionalButtonPressed(GbaInput.Left))
                PreviousXSpeed += 0.015625f;
        }
    }

    private void MoveInTheAir(float speedX)
    {
        if (IsDirectionalButtonPressed(GbaInput.Left))
        {
            if (IsFacingRight)
                AnimatedObject.FlipX = true;

            speedX -= 1.79998779297f;

            if (speedX <= -3)
                speedX = -3;
        }
        else if (IsDirectionalButtonPressed(GbaInput.Right))
        {
            if (IsFacingLeft)
                AnimatedObject.FlipX = false;

            speedX += 1.79998779297f;

            if (speedX > 3)
                speedX = 3;
        }

        MechModel.Speed = MechModel.Speed with { X = speedX };
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
        PhysicalType type = Scene.GetPhysicalType(detectionBox.BottomRight);
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
        type = Scene.GetPhysicalType(detectionBox.BottomLeft);
        if (type.IsSolid)
        {
            PrevSpeedY = 0;
            return true;
        }

        // Check bottom middle
        type = Scene.GetPhysicalType(detectionBox.BottomCenter);
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

        if (MultiJoyPad.IsButtonJustPressed(InstanceId, GbaInput.B))
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
        box = new Box(box.MinX, box.MinY, box.MaxX, box.MaxY - Tile.Size);

        if (Scene.GetPhysicalType(box.BottomRight) == PhysicalTypeValue.Damage ||
            Scene.GetPhysicalType(box.MiddleRight) == PhysicalTypeValue.Damage ||
            Scene.GetPhysicalType(box.TopRight) == PhysicalTypeValue.Damage ||
            Scene.GetPhysicalType(box.TopCenter) == PhysicalTypeValue.Damage ||
            Scene.GetPhysicalType(box.TopLeft) == PhysicalTypeValue.Damage ||
            Scene.GetPhysicalType(box.MiddleLeft) == PhysicalTypeValue.Damage ||
            Scene.GetPhysicalType(box.BottomLeft) == PhysicalTypeValue.Damage ||
            Scene.GetPhysicalType(box.BottomCenter) == PhysicalTypeValue.Damage)
        {
            ReceiveDamage(1);
        }
    }

    private void OffsetCarryingObject()
    {
        if (AttachedObject != null && AttachedObject.Type != (int)ActorType.Keg)
        {
            if (AttachedObject.Type == (int)ActorType.Caterpillar)
                AttachedObject.Position -= new Vector2(0, 16);

            if (IsFacingLeft)
                AttachedObject.Position -= new Vector2(8, 0);
        }
    }

    private bool ManageHit()
    {
        if (RSMultiplayer.IsActive || (GameInfo.Cheats & Cheat.Invulnerable) != 0)
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
                if ((ActorType)AttachedObject.Type is ActorType.Keg or ActorType.Caterpillar or ActorType.Sphere)
                    AttachedObject.ProcessMessage(this, Message.DropObject);
                AttachedObject = null;
            }

            takenDamage = true;

            if (!SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__OnoRcvH1_Mix04))
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

    private bool ShouldAutoJump()
    {
        // Make sure you're sliding
        if (SlideType == null)
            return false;

        // Make sure you're moving fast enough
        if (Math.Abs(Speed.X) <= 2)
            return false;

        Vector2 topPos = Position;
        Vector2 bottomPos = Position + new Vector2(0, Tile.Size);

        if (Speed.X < 0)
        {
            topPos -= new Vector2(Tile.Size * 2, 0);
            bottomPos -= new Vector2(Tile.Size, 0);
        }
        else
        {
            topPos += new Vector2(Tile.Size * 2, 0);
            bottomPos += new Vector2(Tile.Size, 0);
        }

        PhysicalType topType = Scene.GetPhysicalType(topPos);
        PhysicalType bottomType = Scene.GetPhysicalType(bottomPos);

        if (topType == PhysicalTypeValue.SlideJump)
            return bottomType.IsSolid;

        topPos += new Vector2(Tile.Size, 0);
        bottomPos += new Vector2(Tile.Size, 0);

        topType = Scene.GetPhysicalType(topPos);
        bottomType = Scene.GetPhysicalType(bottomPos);

        if (topType == PhysicalTypeValue.SlideJump) 
            return bottomType.IsSolid;
        
        return false;
    }

    private void SlidingOnSlippery()
    {
        if (!SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__SkiLoop1))
            PlaySound(Rayman3SoundEvent.Play__SkiLoop1);

        // TODO: Not on N-Gage
        SoundEventsManager.SetSoundPitch(Rayman3SoundEvent.Play__SkiLoop1, Math.Abs(Speed.X));

        if (PreviousXSpeed < -1.5f)
        {
            if (IsFacingRight)
            {
                if (IsDirectionalButtonPressed(GbaInput.Down))
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
                if (IsDirectionalButtonPressed(GbaInput.Down))
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
                if (IsDirectionalButtonPressed(GbaInput.Down))
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
                if (IsDirectionalButtonPressed(GbaInput.Down))
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

    private void CreateSwingProjectiles()
    {
        SwingSparkle swingSparkle;

        int index = 0;
        while (true)
        {
            float v = index * 16;
            if (PreviousXSpeed < 80)
                v = 64 - v;
            else
                v = PreviousXSpeed - v - 16;

            if (v <= 2)
                break;

            swingSparkle = Scene.CreateProjectile<SwingSparkle>(ActorType.SwingSparkle);
            swingSparkle.Value = v;
            
            index++;
            if (index > 8)
                return;
        }

        swingSparkle = Scene.CreateProjectile<SwingSparkle>(ActorType.SwingSparkle);
        swingSparkle.Value = PreviousXSpeed - 30;
        swingSparkle.AnimatedObject.CurrentAnimation = 1;
    }

    // 0 = false, 1 = right, 2 = left
    private int IsNearEdge()
    {
        Box detectionBox = GetDetectionBox();
        
        PhysicalType centerType = Scene.GetPhysicalType(Position);
        PhysicalType rightType = Scene.GetPhysicalType(detectionBox.BottomRight);
        PhysicalType leftType = Scene.GetPhysicalType(detectionBox.BottomLeft);

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
                        type = Scene.GetPhysicalType(pos - new Vector2(Tile.Size, 0));
                        
                        // Make sure it's not solid
                        if (!type.IsSolid)
                        {
                            Position = new Vector2(
                                x: pos.X - MathHelpers.Mod(pos.X, Tile.Size) - 17, 
                                y: Position.Y - MathHelpers.Mod(Position.Y, Tile.Size) + y * Tile.Size);
                            return true;
                        }
                    }

                    pos += new Vector2(Tile.Size, 0);
                }

                pos += new Vector2(0, Tile.Size);
                pos.X = Position.X;
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
                        type = Scene.GetPhysicalType(pos + new Vector2(Tile.Size, 0));

                        // Make sure it's not solid
                        if (!type.IsSolid)
                        {
                            Position = new Vector2(
                                x: pos.X - MathHelpers.Mod(pos.X, Tile.Size) + 24,
                                y: Position.Y - MathHelpers.Mod(Position.Y, Tile.Size) + y * Tile.Size);
                            return true;
                        }
                    }

                    pos -= new Vector2(Tile.Size, 0);
                }

                pos += new Vector2(0, Tile.Size);
                pos.X = Position.X;
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
            Position = Position with { Y = AttachedObject.Position.Y + 58 };
            AttachedObject = null;
        }
        else
        {
            Position += new Vector2(0, Tile.Size - MathHelpers.Mod(Position.Y, Tile.Size) - 1);
            PlaySound(Rayman3SoundEvent.Play__HandTap1_Mix04);
        }
    }

    private bool IsOnWallJumpable()
    {
        if (!HasPower(Power.WallJump))
            return false;

        if (Flag1_3)
            return false;

        return Scene.GetPhysicalType(Position) == PhysicalTypeValue.WallJump;
    }

    private void BeginWallJump()
    {
        Vector2 pos = Position;

        for (int i = 0; i < 6; i++)
        {
            pos.X += Tile.Size;

            if (Scene.GetPhysicalType(pos) != PhysicalTypeValue.WallJump)
            {
                Position = Position with { X = pos.X - MathHelpers.Mod(pos.X, Tile.Size) - 26 };
                return;
            }
        }
    }

    private void SetRandomIdleAction()
    {
        PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);

        int rand = Random.GetNumber(41);

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
            MapId.MissileRace1 => false,
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
            MapId.MissileRace2 => true,
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
            type = Scene.GetPhysicalType(detectionBox.BottomLeft + new Vector2(16 * i, -1));

            if (type is PhysicalTypeValue.InstaKill or PhysicalTypeValue.Lava or PhysicalTypeValue.Water or PhysicalTypeValue.MoltenLava)
                break;
        }

        if (HitPoints == 0 || type is PhysicalTypeValue.InstaKill or PhysicalTypeValue.Lava or PhysicalTypeValue.Water or PhysicalTypeValue.MoltenLava)
        {
            if (State == FUN_080284ac && type is PhysicalTypeValue.InstaKill or PhysicalTypeValue.MoltenLava)
                return false;

            if (AttachedObject != null)
            {
                if ((ActorType)AttachedObject.Type is ActorType.Keg or ActorType.Caterpillar or ActorType.Sphere)
                    AttachedObject.ProcessMessage(this, Message.DropObject);
                AttachedObject = null;
            }

            // Handle drowning
            if (IsLavaInLevel() && type is PhysicalTypeValue.Lava or PhysicalTypeValue.MoltenLava)
            {
                LavaSplash lavaSplash = Scene.CreateProjectile<LavaSplash>(ActorType.LavaSplash);
                if (lavaSplash != null)
                {
                    lavaSplash.Position = Position;
                    lavaSplash.ActionId = LavaSplash.Action.MainActorDrownSplash;
                    lavaSplash.ChangeAction();
                }

                ActionId = IsFacingRight ? Action.Drown_Right : Action.Drown_Left;
            }
            else if (type == PhysicalTypeValue.Water)
            {
                WaterSplash waterSplash = Scene.CreateProjectile<WaterSplash>(ActorType.WaterSplash);
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
        if (FinishedMap)
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
        }

        if (GameInfo.MapId is not (MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4 or MapId.WorldMap))
        {
            GameInfo.PersistentInfo.LastPlayedLevel = (byte)GameInfo.MapId;
            GameInfo.Save(GameInfo.CurrentSlot);
        }
    }

    private void ToggleNoClip()
    {
        if (InputManager.IsButtonJustPressed(Keys.Z)) // TODO: Do not hard-code this key
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
                State.MoveTo(Fsm_Fall);
                ChangeAction();
            }
        }
    }

    private void DoNoClipBehavior()
    {
        int speed = JoyPad.IsButtonPressed(GbaInput.A) ? 7 : 4;

        if (JoyPad.IsButtonPressed(GbaInput.Up))
            Position -= new Vector2(0, speed);
        else if (JoyPad.IsButtonPressed(GbaInput.Down))
            Position += new Vector2(0, speed);

        if (JoyPad.IsButtonPressed(GbaInput.Left))
            Position -= new Vector2(speed, 0);
        else if (JoyPad.IsButtonPressed(GbaInput.Right))
            Position += new Vector2(speed, 0);
    }

    protected override bool ProcessMessageImpl(object sender, Message message, object param)
    {
        if (base.ProcessMessageImpl(sender, message, param))
            return false;

        // TODO: Implement remaining messages
        switch (message)
        {
            case Message.RaymanBody_FinishedAttack:
                RaymanBody.RaymanBodyPartType bodyPartType = (RaymanBody.RaymanBodyPartType)param;
                ActiveBodyParts[(int)bodyPartType] = null;

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
                if (State != Fsm_Dying)
                {
                    if (State == Fsm_Jump && Speed.Y < 1)
                        return false;

                    MovableActor actorToLink = ((MovableActor)param);
                    Box actorToLinkBox = actorToLink.GetDetectionBox();

                    if (Position.Y < actorToLinkBox.MinY + 7)
                    {
                        LinkedMovementActor = actorToLink;
                        Position = Position with { Y = actorToLinkBox.MinY };
                    }

                    if (State == Fsm_HangOnEdge)
                        State.MoveTo(Fsm_Default);
                }
                return false;

            case Message.Main_UnlinkMovement:
                LinkedMovementActor = null;
                Flag2_2 = true;
                return false;

            case Message.Main_BeginBounce:
                if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                {
                    if (State == Fsm_Swing || 
                        State == Fsm_Dying || 
                        State == FUN_1005dea0 || 
                        State == FUN_1005dfa4 || 
                        State == FUN_1005e04c)
                        return false;
                }
                else
                {
                    if (State == Fsm_Swing || 
                        State == Fsm_Dying)
                        return false;
                }

                State.MoveTo(Fsm_Bounce);
                return false;

            case Message.Main_Bounce:
                if (State == Fsm_Bounce)
                {
                    Flag1_5 = true;
                    return false;
                }

                if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                {
                    if (State == FUN_1005dea0 ||
                        State == FUN_1005dfa4 ||
                        State == FUN_1005e04c)
                        return false;
                }

                ActionId = IsFacingRight ? Action.BouncyJump_Right : Action.BouncyJump_Left;

                State.MoveTo(Fsm_Jump);
                return false;

            case Message.Main_CollectedYellowLum:
                ((FrameSideScroller)Frame.Current).UserInfo.AddLums(1);
                return false;

            case Message.Main_CollectedRedLum:
                if (HitPoints < 5)
                    HitPoints++;

                ((FrameSideScroller)Frame.Current).UserInfo.UpdateLife();
                return false;

            case Message.Main_CollectedBlueLum:
                // TODO: Implement
                return false;

            case Message.Main_CollectedWhiteLum:
                if (!RSMultiplayer.IsActive)
                    GameInfo.ModifyLives(1);
                return false;

            case Message.Main_CollectedBigYellowLum:
                ((FrameSideScroller)Frame.Current).UserInfo.AddLums(10);
                return false;

            case Message.Main_CollectedBigBlueLum:
                // TODO: Implement
                return false;

            case Message.Main_LevelEnd:
                FinishedMap = true;
                State.MoveTo(Fsm_EndMap);
                return false;

            case Message.Main_PickUpObject:
                if (State == Fsm_Walk || State == Fsm_Crawl)
                {
                    AttachedObject = (BaseActor)param;
                    State.MoveTo(Fsm_PickUpObject);
                }
                return false;

            case Message.Main_CatchObject:
                if (State == Fsm_Default || State == Fsm_Walk)
                {
                    AttachedObject = (BaseActor)param;
                    State.MoveTo(Fsm_CatchObject);
                }
                return false;

            case Message.Damaged:
            case Message.Main_Damaged2:
            case Message.Main_Damaged3:
            case Message.Main_Damaged4:
                if (State == Fsm_HitKnockback || State == Fsm_Dying || State == Fsm_EndMap || InvulnerabilityDuration != 0)
                    return false;

                if (LinkedMovementActor != null)
                {
                    LinkedMovementActor = null;
                    Position -= new Vector2(0, 7);
                }

                if (AttachedObject is { Type: (int)ActorType.Plum })
                {
                    Box box = ((ActionActor)AttachedObject).GetActionBox(); // TODO: Cast to Plum when we create a class for it
                    LinkedMovementActor = null; // TODO: Huh? Isn't this meant to be setting attached object to null?
                    Position = Position with { Y = box.MinY - 16 };
                }

                if (((BaseActor)sender).Type == (int)ActorType.SpikyFlyingBomb && !IsInvulnerable)
                    InvulnerabilityDuration = 60;

                if (message == Message.Main_Damaged3)
                    CheckAgainstObjectCollision = false;
                else if (message == Message.Main_Damaged4)
                    Flag1_C = true;

                if (AttachedObject != null && (ActorType)AttachedObject.Type is ActorType.Keg or ActorType.Caterpillar or ActorType.Sphere)
                    AttachedObject.ProcessMessage(this, Message.DropObject);

                AttachedObject = (BaseActor)sender;

                if (State == Fsm_Climb)
                    Flag2_1 = true;

                State.MoveTo(Fsm_HitKnockback);
                return false;

            case Message.Main_BeginHang:
                IsHanging = true;
                AttachedObject = (BaseActor)param;
                return false;

            case Message.Main_EndHang:
                IsHanging = false;
                return false;

            case Message.Main_LevelExit:
                State.MoveTo(Fsm_EndMap);
                return false;

            case Message.Main_CollectedCage:
                ((FrameSideScroller)Frame.Current).UserInfo.AddCages(1);
                return false;

            case Message.Main_BeginSwing:
                if (!HasPower(Power.Grab))
                {
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__LumVioNP_SkulShak_Mix01);
                    return false;
                }

                AttachedObject = (BaseActor)param;
                BaseActor senderObj = (BaseActor)sender;

                if (State == Fsm_Swing)
                    return false;

                if (Position.X < senderObj.Position.X)
                {
                    if (senderObj.Position.X - 200 > Position.X)
                        return false;
                }
                else
                {
                    if (senderObj.Position.X + 200 <= Position.X)
                        return false;
                }

                State.MoveTo(Fsm_Swing);
                return false;

            case Message.Main_AllowCoyoteJump:
                if (State != Fsm_Jump && State != Fsm_JumpSlide)
                    CanSafetyJump = true;
                return false;

            case Message.Main_QuickFinishBodyShotAttack:
                if (State == Fsm_BodyShotAttack)
                    State.MoveTo(Fsm_QuickFinishBodyShotAttack);
                return false;

            case Message.Main_1056:
                Flag1_0 = true;
                return false;

            case Message.Main_Stop:
                State.MoveTo(Fsm_Stop);
                return false;

            case Message.Main_ExitStopOrCutscene:
                if (State == Fsm_Stop || State == Fsm_Cutscene)
                {
                    if (IsOnClimbableVertical() != 0)
                        State.MoveTo(Fsm_Climb);
                    else
                        State.MoveTo(Fsm_Default);
                }
                return false;

            case Message.Main_EnterLevelCurtain:
                if (State != Fsm_EnterLevelCurtain)
                    State.MoveTo(Fsm_EnterLevelCurtain);
                return false;

            case Message.Main_BeginInFrontOfLevelCurtain:
                IsInFrontOfLevelCurtain = true;
                return false;

            case Message.Main_EndInFrontOfLevelCurtain:
                IsInFrontOfLevelCurtain = false;
                return false;

            case Message.Main_EnterCutscene:
                State.MoveTo(Fsm_Cutscene);
                return false;

            case Message.Main_LockedLevelCurtain:
                if (State != Fsm_LockedLevelCurtain)
                    State.MoveTo(Fsm_LockedLevelCurtain);
                return false;

            default:
                return false;
        }
    }

    public void SetPowers(Power powers)
    {
        GameInfo.Powers |= powers;
    }

    public override void Init(ActorResource actorResource)
    {
        AnimatedObject.YPriority = IsLocalPlayer ? 16 : 17;

        Timer = 0;
        InvulnerabilityStartTime = 0;
        //field11_0x88 = 0;
        NextActionId = null;
        Array.Clear(ActiveBodyParts);
        Flag1_0 = false;
        Flag1_1 = false;
        IsHanging = false;
        Flag1_3 = false;
        Flag1_4 = false;
        Flag1_5 = false;
        IsInFrontOfLevelCurtain = false;
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
        CameraTargetY = 0;
        //field13_0x8c = 0;
        ReverseControlsTimer = 0;
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

        CheckAgainstMapCollision = true;
        CheckAgainstObjectCollision = true;

        ActionId = (Action)Resource.FirstActionId;
        ChangeAction();

        if (GameInfo.LastGreenLumAlive == 0)
        {
            // Start facing left when returning from certain levels
            if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4 &&
                GameInfo.field12_0xf == 0)
            {
                if ((MapId)GameInfo.PersistentInfo.LastPlayedLevel is 
                    MapId.MarshAwakening1 or 
                    MapId.BossMachine or 
                    MapId.MissileRace1 or 
                    MapId.MarshAwakening2 or 
                    MapId.BeneathTheSanctuary_M1 or 
                    MapId.BeneathTheSanctuary_M2 or 
                    MapId.BossRockAndLava or 
                    MapId.SanctuaryOfRockAndLava_M1 or 
                    MapId.SanctuaryOfRockAndLava_M3 or 
                    MapId.BossScaleMan or 
                    MapId.Bonus4 or 
                    MapId.Power4)
                {
                    ActionId = Action.Idle_Left;
                    ChangeAction();
                }
            }
        }
        else
        {
            Position = GameInfo.CheckpointPosition;
            ActionId = Action.Idle_Right;
            ChangeAction();
        }

        GameInfo.field12_0xf = 0;

        EnableCheats();
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
                (GameInfo.Cheats & Cheat.Invulnerable) == 0 && 
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

    public class NGageMultiplayerData
    {
        // TODO: Name these properties
        public BaseActor field_00 { get; set; }
        public AnimatedObject[] FlagArrows { get; } = new AnimatedObject[RSMultiplayer.MaxPlayersCount - 1];
        public uint field_ac { get; set; }
        public uint field_b0 { get; set; }
        public uint field_b4 { get; set; }
        public byte field_b8 { get; set; }
        public byte field_b9 { get; set; }
        public byte PlayerPaletteId { get; set; } // TODO: Probably don't need this
        public Power Powers { get; set; }
        public byte field_bc { get; set; }
    }
}