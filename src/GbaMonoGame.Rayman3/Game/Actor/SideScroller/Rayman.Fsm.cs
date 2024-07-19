using System;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class Rayman
{
    // FUN_08020ee0
    // TODO: Name
    private bool FsmStep_Inlined_FUN_1004c544()
    {
        if (!FsmStep_CheckDeath())
            return false;

        CheckSlide();
        ManageSlide();

        if (ManageHit())
        {
            State.MoveTo(Fsm_Hit);
            return false;
        }

        if (IsButtonJustPressed(GbaInput.A) && SlideType != null)
        {
            State.MoveTo(Fsm_JumpSlide);
            return false;
        }

        if (ShouldAutoJump())
        {
            PlaySound(Rayman3SoundEvent.Stop__SkiLoop1);

            State.MoveTo(Fsm_JumpSlide);
            return false;
        }

        return true;
    }

    private bool FsmStep_CheckDeath()
    {
        CameraSideScroller cam = (CameraSideScroller)Scene.Camera;

        if (Flag1_D)
        {
            field16_0x91++;

            if (field16_0x91 > 60)
            {
                cam.HorizontalOffset = CameraOffset.Default;
                Flag1_D = false;
            }
        }

        if (IsLocalPlayer &&
            State != Fsm_Jump &&
            State != FUN_0802ce54 &&
            State != FUN_080284ac &&
            State != Fsm_EnterLevelCurtain &&
            State != Fsm_LockedLevelCurtain &&
            !IsInFrontOfLevelCurtain)
        {
            Message message;

            if (State != FUN_0802ddac &&
                IsButtonPressed(GbaInput.Down) &&
                (Speed.Y > 0 || State == Fsm_Crouch) &&
                State != Fsm_Climb)
            {
                CameraTargetY = 70;
                message = Message.Cam_FollowPositionY;
            }
            else if (IsButtonPressed(GbaInput.Up) && (State == Fsm_Default || State == Fsm_HangOnEdge))
            {
                CameraTargetY = 160;
                message = Message.Cam_FollowPositionY;
            }
            else if (State == Fsm_Helico && field27_0x9c == 0)
            {
                message = Message.Cam_DoNotFollowPositionY;
            }
            else if (State == Fsm_Swing)
            {
                CameraTargetY = 65;
                message = Message.Cam_FollowPositionY;
            }
            else if (State == Fsm_Climb || State == FUN_0802ddac)
            {
                CameraTargetY = 112;
                message = Message.Cam_FollowPositionY;
            }
            else
            {
                CameraTargetY = 120;
                message = Message.Cam_FollowPositionY;
            }

            cam.ProcessMessage(this, message, CameraTargetY);
        }

        if (field23_0x98 != 0)
            field23_0x98--;

        if (IsDead())
        {
            if (!RSMultiplayer.IsActive)
                State.MoveTo(Fsm_Dying);
            else
                State.MoveTo(FUN_08033228);

            return false;
        }

        return true;
    }

    private bool FsmStep_DoInTheAir()
    {
        if (ManageHit() &&
            (State == Fsm_StopHelico ||
             State == Fsm_Helico ||
             State == Fsm_Jump ||
             State == Fsm_JumpSlide) &&
            ActionId is not (
                Action.Damage_Knockback_Right or
                Action.Damage_Knockback_Left or
                Action.Damage_Shock_Right or
                Action.Damage_Shock_Left))
        {
            ActionId = IsFacingRight ? Action.Damage_Knockback_Right : Action.Damage_Knockback_Left;
        }

        return FsmStep_CheckDeath();
    }

    // TODO: Name
    private bool FsmStep_FUN_08020e8c()
    {
        if (Engine.Settings.Platform == Platform.GBA)
        {
            CameraSideScroller cam = (CameraSideScroller)Scene.Camera;
            cam.HorizontalOffset = CameraOffset.Center;
        }

        if (field23_0x98 != 0)
            field23_0x98--;

        ManageHit();

        if (HitPoints == 0)
        {
            State.MoveTo(Fsm_Dying);
            return false;
        }

        return true;
    }

    // TODO: Name
    private bool FsmStep_FUN_08020ee0()
    {
        if (!FsmStep_CheckDeath())
            return false;

        CheckSlide();
        ManageSlide();

        if (ManageHit())
        {
            State.MoveTo(Fsm_Hit);
            return false;
        }

        if (IsButtonJustPressed(GbaInput.A) && SlideType != null)
        {
            State.MoveTo(Fsm_JumpSlide);
            return false;
        }

        if (ShouldAutoJump())
        {
            PlaySound(Rayman3SoundEvent.Stop__SldGreen_SkiLoop1);
            State.MoveTo(Fsm_JumpSlide);
            return false;
        }

        return true;
    }

    private void Fsm_LevelStart(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // This never actually has a chance to play due to the Init function being called afterwards and overriding this
                ActionId = IsFacingRight ? Action.Spawn_Right : Action.Spawn_Left;
                ChangeAction();

                Timer = 0;

                CameraSideScroller cam = (CameraSideScroller)Scene.Camera;
                if (GameInfo.MapId == MapId.TheCanopy_M2)
                {
                    cam.HorizontalOffset = CameraOffset.Center;
                }
                else
                {
                    if (!RSMultiplayer.IsActive)
                        cam.HorizontalOffset = CameraOffset.Default;
                    else if (IsLocalPlayer)
                        cam.HorizontalOffset = CameraOffset.Multiplayer;
                }

                if (IsLocalPlayer)
                    cam.ProcessMessage(this, Message.Cam_ResetPositionX);

                if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4)
                    cam.HorizontalOffset = CameraOffset.Center;
                break;

            case FsmAction.Step:
                // Check if we're spawning at a curtain
                if (IsInFrontOfLevelCurtain)
                {
                    // Hide while fading and then show spawn animation
                    if (((FrameWorldSideScroller)Frame.Current).TransitionsFX.IsFadeInFinished)
                    {
                        if (ActionId is not (Action.Spawn_Curtain_Right or Action.Spawn_Curtain_Left))
                            ActionId = IsFacingRight ? Action.Spawn_Curtain_Right : Action.Spawn_Curtain_Left;
                    }
                    else
                    {
                        ActionId = IsFacingRight ? Action.Hidden_Right : Action.Hidden_Left;
                    }
                }

                Timer++;

                if (IsActionFinished && IsBossFight())
                    NextActionId = IsFacingRight ? Action.Idle_Determined_Right : Action.Idle_Determined_Left;

                if (!IsActionFinished)
                    return;

                if (!RSMultiplayer.IsActive || Timer >= 210)
                    State.MoveTo(Fsm_Default);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Default(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                CheckSlide();

                if (IsSliding)
                {
                    SlidingOnSlippery();
                }
                else
                {
                    PlaySound(Rayman3SoundEvent.Stop__SkiLoop1);

                    if (SlideType == null)
                        PreviousXSpeed = 0;

                    if (!IsBossFight())
                    {
                        if (NextActionId == null)
                        {
                            // Randomly show Rayman being bored
                            if (Random.GetNumber(11) < 6)
                                ActionId = IsFacingRight ? Action.Idle_Bored_Right : Action.Idle_Bored_Left;
                            else
                                ActionId = IsFacingRight ? Action.Idle_Right : Action.Idle_Left;
                        }
                        else
                        {
                            ActionId = NextActionId.Value;
                        }
                    }
                    else if (NextActionId is Action.Idle_Determined_Right or Action.Idle_Determined_Left)
                    {
                        ActionId = NextActionId.Value;
                    }
                    else
                    {
                        ActionId = IsFacingRight ? Action.Idle_ReadyToFight_Right : Action.Idle_ReadyToFight_Left;
                    }
                }

                Timer = 0;
                break;

            case FsmAction.Step:
                if (!FsmStep_Inlined_FUN_1004c544())
                    return;

                Timer++;

                // Look up when pressing up
                if (IsButtonPressed(GbaInput.Up))
                {
                    if (ActionId is not (Action.LookUp_Right or Action.LookUp_Left) && SlideType == null)
                    {
                        ActionId = IsFacingRight ? Action.LookUp_Right : Action.LookUp_Left;
                        NextActionId = null;
                    }
                }
                else
                {
                    if (ActionId is Action.LookUp_Right or Action.LookUp_Left)
                        ActionId = IsFacingRight ? Action.Idle_Right : Action.Idle_Left;
                }

                // Play idle animation
                if (IsActionFinished && 
                    (ActionId == NextActionId && ActionId is not (
                        Action.Idle_Bored_Right or Action.Idle_Bored_Left or
                        Action.Idle_LookAround_Right or Action.Idle_LookAround_Left)) &&
                    (ActionId is not (
                         Action.Idle_BasketBall_Right or Action.Idle_BasketBall_Left or
                         Action.Idle_Grimace_Right or Action.Idle_Grimace_Left) ||
                     Timer > 180))
                {
                    if (!IsBossFight())
                        ActionId = IsFacingRight ? Action.Idle_Right : Action.Idle_Left;
                    else
                        ActionId = IsFacingRight ? Action.Idle_ReadyToFight_Right : Action.Idle_ReadyToFight_Left;

                    NextActionId = null;

                    PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                }

                if (IsSliding)
                {
                    SlidingOnSlippery();
                }
                else
                {
                    PlaySound(Rayman3SoundEvent.Stop__SkiLoop1);

                    if (NextActionId != null && NextActionId != ActionId)
                    {
                        ActionId = NextActionId.Value;
                    }
                    else if (ActionId is not (
                                 Action.LookUp_Right or Action.LookUp_Left or
                                 Action.Idle_Left or Action.Idle_Right or
                                 Action.Idle_ReadyToFight_Right or Action.Idle_ReadyToFight_Left) &&
                             ActionId != NextActionId)
                    {
                        if (!IsBossFight())
                            ActionId = IsFacingRight ? Action.Idle_Right : Action.Idle_Left;
                        else
                            ActionId = IsFacingRight ? Action.Idle_ReadyToFight_Right : Action.Idle_ReadyToFight_Left;
                    }
                }

                if (IsButtonPressed(GbaInput.Left) && IsFacingRight)
                {
                    ActionId = Action.Walk_Left;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }
                else if (IsButtonPressed(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Walk_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }

                // Jump
                if (IsButtonJustPressed(GbaInput.A) && Flag2_2)
                {
                    PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    State.MoveTo(Fsm_Jump);
                    return;
                }
                
                // Crouch
                if (IsButtonPressed(GbaInput.Down))
                {
                    NextActionId = IsFacingRight ? Action.CrouchDown_Right : Action.CrouchDown_Left;
                    PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    State.MoveTo(Fsm_Crouch);
                    return;
                }
                
                // Fall
                if (Speed.Y > 1)
                {
                    PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    State.MoveTo(Fsm_Fall);
                    return;
                }
                
                // Walk
                if (IsButtonPressed(GbaInput.Left) || IsButtonPressed(GbaInput.Right))
                {
                    PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    State.MoveTo(Fsm_Walk);
                    return;
                }
                
                // Punch
                if (field23_0x98 == 0 && IsButtonJustPressed(GbaInput.B) && CanAttackWithFist(2))
                {
                    PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    State.MoveTo(Fsm_ChargeAttack);
                    return;
                }

                // Walking off edge
                if (PreviousXSpeed != 0 && IsNearEdge() != 0 && !Flag1_1)
                {
                    PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    Position += new Vector2(PreviousXSpeed < 0 ? -16 : 16, 0);
                    State.MoveTo(Fsm_Fall);
                    return;
                }

                // Standing near edge
                if (PreviousXSpeed == 0 && IsNearEdge() != 0 && !Flag1_1)
                {
                    PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    State.MoveTo(Fsm_StandingNearEdge);
                    return;
                }

                // Restart default state
                if ((IsActionFinished && ActionId is not (
                         Action.LookUp_Right or Action.LookUp_Left or
                         Action.Idle_Bored_Right or Action.Idle_Bored_Left or
                         Action.Idle_LookAround_Right or Action.Idle_LookAround_Left) &&
                     360 < Timer) ||
                    (IsActionFinished && ActionId is
                         Action.Idle_Bored_Right or Action.Idle_Bored_Left or
                         Action.Idle_LookAround_Right or Action.Idle_LookAround_Left &&
                     720 < Timer))
                {
                    SetRandomIdleAction();
                    State.MoveTo(Fsm_Default);
                    return;
                }

                Flag1_1 = false;
                break;

            case FsmAction.UnInit:
                if (ActionId is Action.Idle_SpinBody_Right or Action.Idle_SpinBody_Left)
                    PlaySound(Rayman3SoundEvent.Stop__RaySpin_Mix06);

                if (ActionId == NextActionId || ActionId is Action.Walk_Right or Action.Walk_Left or Action.Walk_Multiplayer_Right or Action.Walk_Multiplayer_Left)
                    NextActionId = null;

                if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4)
                {
                    CameraSideScroller cam = (CameraSideScroller)Scene.Camera;

                    if (cam.HorizontalOffset == CameraOffset.Center)
                        cam.HorizontalOffset = CameraOffset.Default;
                }
                break;
        }
    }

    private void Fsm_StandingNearEdge(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (!SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__OnoEquil_Mix03))
                    PlaySound(Rayman3SoundEvent.Play__OnoEquil_Mix03);

                Timer = 120;
                NextActionId = null;

                Box detectionBox = GetDetectionBox();
                PhysicalType rightType = Scene.GetPhysicalType(new Vector2(detectionBox.MaxX, detectionBox.MaxY));

                if (rightType.IsSolid)
                    ActionId = IsFacingRight ? Action.NearEdgeBehind_Right : Action.NearEdgeFront_Left;
                else
                    ActionId = IsFacingRight ? Action.NearEdgeFront_Right : Action.NearEdgeBehind_Left;
                break;

            case FsmAction.Step:
                if (!FsmStep_Inlined_FUN_1004c544())
                    return;

                Timer--;

                // Play sound every 2 seconds
                if (Timer == 0)
                {
                    Timer = 120;

                    if (!SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__OnoEquil_Mix03))
                        PlaySound(Rayman3SoundEvent.Play__OnoEquil_Mix03);
                }

                // Change direction
                if (IsButtonPressed(GbaInput.Left) && IsFacingRight)
                {
                    ActionId = Action.Walk_Left;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }
                else if (IsButtonPressed(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Walk_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }

                // Walk
                if (IsButtonPressed(GbaInput.Left) || IsButtonPressed(GbaInput.Right))
                {
                    State.MoveTo(Fsm_Walk);
                    return;
                }
                
                // Crouch
                if (IsButtonPressed(GbaInput.Down))
                {
                    State.MoveTo(Fsm_Crouch);
                    return;
                }

                // Jump
                if (IsButtonJustPressed(GbaInput.A))
                {
                    State.MoveTo(Fsm_Jump);
                    return;
                }

                // Fall
                if (Speed.Y > 1)
                {
                    State.MoveTo(Fsm_Fall);
                    return;
                }

                // Punch
                if (IsButtonPressed2(GbaInput.B) && CanAttackWithFist(2))
                {
                    State.MoveTo(Fsm_ChargeAttack);
                    return;
                }

                // Default if no longer near edge
                if (IsNearEdge() == 0)
                {
                    State.MoveTo(Fsm_Default);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Walk(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (IsSliding)
                {
                    SlidingOnSlippery();
                }
                else
                {
                    PlaySound(Rayman3SoundEvent.Stop__SkiLoop1);

                    if (SlideType == null)
                        PreviousXSpeed = 0;

                    if (!RSMultiplayer.IsActive)
                    {
                        // Randomly look around for Globox in the first level
                        if (GameInfo.MapId == MapId.WoodLight_M1 && GameInfo.LastGreenLumAlive == 0)
                        {
                            if (Random.GetNumber(501) >= 401)
                                ActionId = IsFacingRight ? Action.Walk_LookAround_Right : Action.Walk_LookAround_Left;
                            else
                                ActionId = IsFacingRight ? Action.Walk_Right : Action.Walk_Left;

                            field22_0x97 = 0;
                        }
                        else
                        {
                            ActionId = IsFacingRight ? Action.Walk_Right : Action.Walk_Left;
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                Timer = 0;
                Charge = 0;
                break;

            case FsmAction.Step:
                if (!FsmStep_Inlined_FUN_1004c544())
                    return;

                if (Speed.Y > 1 && PreviousXSpeed == 0)
                {
                    Timer++;
                    Position -= new Vector2(0, Speed.Y);
                }

                // Randomly look around for Globox in the first level
                if (GameInfo.MapId == MapId.WoodLight_M1 && GameInfo.LastGreenLumAlive == 0)
                {
                    field22_0x97++;

                    if (IsActionFinished)
                    {
                        if (ActionId is Action.Walk_Right or Action.Walk_Left &&
                            field22_0x97 > Random.GetNumber(121) + 120)
                        {
                            ActionId = IsFacingRight ? Action.Walk_LookAround_Right : Action.Walk_LookAround_Left;
                            field22_0x97 = 0;
                        }
                        else if (ActionId is Action.Walk_LookAround_Right or Action.Walk_LookAround_Left && 
                                 field22_0x97 > Random.GetNumber(121) + 60)
                        {
                            ActionId = IsFacingRight ? Action.Walk_Right : Action.Walk_Left;
                            field22_0x97 = 0;
                        }
                    }
                }

                // Change walking direction
                if (ActionId is Action.Walk_LookAround_Right or Action.Walk_LookAround_Left)
                {
                    if (IsButtonPressed(GbaInput.Left) && IsFacingRight)
                    {
                        ActionId = Action.Walk_LookAround_Left;
                        ChangeAction();

                        if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                            throw new NotImplementedException();
                    }
                    else if (IsButtonPressed(GbaInput.Right) && IsFacingLeft)
                    {
                        ActionId = Action.Walk_LookAround_Right;
                        ChangeAction();

                        if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    if (IsButtonPressed(GbaInput.Left) && IsFacingRight)
                    {
                        ActionId = Action.Walk_Left;
                        ChangeAction();

                        if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                            throw new NotImplementedException();
                    }
                    else if (IsButtonPressed(GbaInput.Right) && IsFacingLeft)
                    {
                        ActionId = Action.Walk_Right;
                        ChangeAction();

                        if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                            throw new NotImplementedException();
                    }
                }

                if (IsButtonPressed2(GbaInput.B))
                {
                    Charge++;
                }
                else if (IsButtonJustReleased(GbaInput.B) && field23_0x98 == 0)
                {
                    Charge = 0;

                    if (CanAttackWithFist(1))
                    {
                        Attack(0, RaymanBody.RaymanBodyPartType.Fist, new Vector2(16, -16), false);
                        field23_0x98 = 0;
                    }
                    else if (CanAttackWithFist(2))
                    {
                        Attack(0, RaymanBody.RaymanBodyPartType.SecondFist, new Vector2(16, -16), false);

                        if ((GameInfo.Powers & Power.DoubleFist) == 0)
                            field23_0x98 = 0;
                    }
                }

                if (IsSliding)
                {
                    SlidingOnSlippery();
                }
                else
                {
                    if (SlideType != null && 
                        ActionId is
                            Action.Walk_Right or Action.Walk_Left or
                            Action.Walk_LookAround_Right or Action.Walk_LookAround_Left &&
                        AnimatedObject.CurrentFrame is 2 or 10 &&
                        !AnimatedObject.IsDelayMode)
                    {
                        PlaySound(Rayman3SoundEvent.Play__PlumSnd2_Mix03);
                    }

                    PlaySound(Rayman3SoundEvent.Stop__SkiLoop1);

                    if (!RSMultiplayer.IsActive)
                    {
                        if (ActionId is not (
                            Action.Walk_Right or Action.Walk_Left or
                            Action.Walk_LookAround_Right or Action.Walk_LookAround_Left))
                        {
                            if (GameInfo.MapId == MapId.WoodLight_M1 && GameInfo.LastGreenLumAlive == 0)
                                ActionId = IsFacingRight ? Action.Walk_LookAround_Right : Action.Walk_LookAround_Left;
                            else
                                ActionId = IsFacingRight ? Action.Walk_Right : Action.Walk_Left;
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                // Return if released the left or right inputs
                if (IsButtonReleased(GbaInput.Left) && IsButtonReleased(GbaInput.Right) &&
                    ActionId is
                        Action.Walk_Right or Action.Walk_Left or
                        Action.Walk_Multiplayer_Right or Action.Walk_Multiplayer_Left)
                {
                    State.MoveTo(Fsm_Default);
                    return;
                }

                // Return and shout for Globox if looking for him when released the left and right inputs
                if (IsButtonReleased(GbaInput.Left) && IsButtonReleased(GbaInput.Right) &&
                    ActionId is Action.Walk_LookAround_Right or Action.Walk_LookAround_Left)
                {
                    NextActionId = IsFacingRight ? Action.Idle_Shout_Right : Action.Idle_Shout_Left;
                    State.MoveTo(Fsm_Default);
                    return;
                }

                // Crawl
                if (IsButtonPressed(GbaInput.Down))
                {
                    State.MoveTo(Fsm_Crawl);
                    return;
                }

                // Jump
                if (IsButtonJustPressed(GbaInput.A))
                {
                    State.MoveTo(Fsm_Jump);
                    return;
                }

                // Fall
                if (PreviousXSpeed != 0 && Speed.Y > 1)
                {
                    Position += new Vector2(IsFacingLeft ? -16 : 16, 0);
                    State.MoveTo(Fsm_Fall);
                    return;
                }

                // Fall
                if (Speed.Y > 1 && Timer >= 8)
                {
                    State.MoveTo(Fsm_Fall);
                    return;
                }

                // Charge punch
                if (field23_0x98 == 0 && Charge > 10 && IsButtonPressed(GbaInput.B) && CanAttackWithFist(2))
                {
                    State.MoveTo(Fsm_ChargeAttack);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Jump(FsmAction action)
    {
        CameraSideScroller cam = (CameraSideScroller)Scene.Camera;

        switch (action)
        {
            case FsmAction.Init:
                PlaySound(Rayman3SoundEvent.Stop__SkiLoop1);

                if (ActionId is not (Action.BouncyJump_Right or Action.BouncyJump_Left))
                {
                    ActionId = IsFacingRight ? Action.Jump_Right : Action.Jump_Left;
                    PlaySound(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                }

                NextActionId = null;
                CameraTargetY = 70;

                if (IsLocalPlayer)
                    cam.ProcessMessage(this, Message.Cam_DoNotFollowPositionY, CameraTargetY);

                Timer = GameTime.ElapsedFrames;
                SlideType = null;
                LinkedMovementActor = null;
                break;

            case FsmAction.Step:
                if (!FsmStep_DoInTheAir())
                    return;

                if (IsLocalPlayer)
                    cam.ProcessMessage(this, Message.Cam_DoNotFollowPositionY, 130);

                if (ActionId is Action.Jump_Right or Action.Jump_Left &&
                    IsButtonReleased2(GbaInput.A) && 
                    MechModel.Speed.Y < -4 && 
                    !Flag2_0)
                {
                    MechModel.Speed = new Vector2(MechModel.Speed.X, -4);
                    Flag2_0 = true;
                }

                if (Speed.Y == 0 && MechModel.Speed.Y < 0)
                    MechModel.Speed = new Vector2(MechModel.Speed.X, 0);

                MoveInTheAir(PreviousXSpeed);
                SlowdownAirSpeed();
                AttackInTheAir();

                if (HasLanded())
                {
                    NextActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                    State.MoveTo(Fsm_Default);
                    return;
                }

                if (GameTime.ElapsedFrames - Timer > 50)
                {
                    State.MoveTo(Fsm_Fall);
                    return;
                }

                if (IsNearHangableEdge())
                {
                    State.MoveTo(Fsm_HangOnEdge);
                    return;
                }

                if (IsButtonJustPressed(GbaInput.A) && field27_0x9c == 0)
                {
                    State.MoveTo(Fsm_Helico);
                    return;
                }

                // TODO: Implement

                if (IsOnHangable())
                {
                    BeginHang();
                    State.MoveTo(Fsm_Hang);
                    return;
                }

                if (GameTime.ElapsedFrames - Timer > 10 && IsOnClimbableVertical() == 1)
                {
                    State.MoveTo(Fsm_Climb);
                    return;
                }

                // TODO: Implement
                break;

            case FsmAction.UnInit:
                Flag2_0 = false;
                break;
        }
    }

    private void Fsm_JumpSlide(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                PlaySound(Rayman3SoundEvent.Stop__SldGreen_SkiLoop1);
                ActionId = IsFacingRight ? Action.Sliding_Jump_Right : Action.Sliding_Jump_Left;
                NextActionId = null;
                Flag2_0 = false;
                Timer = GameTime.ElapsedFrames;
                SlideType = null;
                PreviousXSpeed /= 2;
                break;

            case FsmAction.Step:
                if (!FsmStep_DoInTheAir())
                    return;

                float speedY = Speed.Y;

                AttackInTheAir();
                MoveInTheAir(PreviousXSpeed);

                // Land
                if (HasLanded())
                {
                    PreviousXSpeed = 0;
                    NextActionId = IsFacingRight ? Action.Sliding_Land_Right : Action.Sliding_Land_Left;
                    State.MoveTo(Fsm_Default);
                    return;
                }

                // Fall
                if (speedY > 3.4375)
                {
                    State.MoveTo(Fsm_Fall);
                    return;
                }

                // Hang on edge
                if (IsNearHangableEdge())
                {
                    PreviousXSpeed = 0;
                    State.MoveTo(Fsm_HangOnEdge);
                    return;
                }

                // Helico
                if (IsButtonJustPressed(GbaInput.A) && field27_0x9c == 0 && GameTime.ElapsedFrames - Timer >= 6)
                {
                    State.MoveTo(Fsm_Helico);
                    return;
                }

                if (IsButtonJustPressed(GbaInput.A) && field27_0x9c != 0 && GameTime.ElapsedFrames - Timer >= 6)
                {
                    State.MoveTo(FUN_0802ddac);
                    return;
                }

                // Hang
                if (GameTime.ElapsedFrames - Timer >= 11 && IsOnHangable())
                {
                    PreviousXSpeed = 0;
                    BeginHang();
                    State.MoveTo(Fsm_Hang);
                }

                if (IsOnClimbableVertical() == 1)
                {
                    PreviousXSpeed = 0;
                    State.MoveTo(Fsm_Climb);
                    return;
                }
                break;

            case FsmAction.UnInit:
                Flag2_0 = false;
                break;
        }
    }

    private void Fsm_HangOnEdge(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                PlaySound(Rayman3SoundEvent.Play__HandTap1_Mix04);
                PreviousXSpeed = 0;

                if (NextActionId is Action.HangOnEdge_EndAttack_Right or Action.HangOnEdge_EndAttack_Left)
                    ActionId = IsFacingRight ? Action.HangOnEdge_EndAttack_Right : Action.HangOnEdge_EndAttack_Left;
                else
                    ActionId = IsFacingRight ? Action.HangOnEdge_Begin_Right : Action.HangOnEdge_Begin_Left;

                SetDetectionBox(new Box(
                    minX: ActorModel.DetectionBox.MinX,
                    minY: ActorModel.DetectionBox.MaxY - 16,
                    maxX: ActorModel.DetectionBox.MaxX,
                    maxY: ActorModel.DetectionBox.MaxY + 16));
                break;

            case FsmAction.Step:
                if (!FsmStep_DoInTheAir())
                    return;

                if (IsActionFinished && ActionId is not (Action.HangOnEdge_Idle_Right or Action.HangOnEdge_Idle_Left))
                {
                    ActionId = IsFacingRight ? Action.HangOnEdge_Idle_Right : Action.HangOnEdge_Idle_Left;
                    NextActionId = null;
                }

                // Move down
                if (IsButtonPressed(GbaInput.Down))
                {
                    HangOnEdgeDelay = 30;
                    PlaySound(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                    State.MoveTo(Fsm_Fall);
                    return;
                }

                // Jump
                if (IsButtonJustPressed(GbaInput.A))
                {
                    HangOnEdgeDelay = 30;
                    State.MoveTo(Fsm_Jump);
                    return;
                }

                // Attack
                if (IsButtonPressed2(GbaInput.B) && CanAttackWithFoot())
                {
                    State.MoveTo(Fsm_ChargeAttack);
                    return;
                }
                break;

            case FsmAction.UnInit:
                SetDetectionBox(new Box(ActorModel.DetectionBox));
                break;
        }
    }

    private void Fsm_Fall(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                PlaySound(Rayman3SoundEvent.Stop__SkiLoop1);
                ActionId = IsFacingRight ? Action.Fall_Right : Action.Fall_Left;
                NextActionId = null;
                Timer = 0;
                break;

            case FsmAction.Step:
                if (!FsmStep_DoInTheAir())
                    return;
                
                Timer++;

                if (CanSafetyJump && Timer > 15)
                    CanSafetyJump = false;

                MoveInTheAir(PreviousXSpeed);
                SlowdownAirSpeed();
                AttackInTheAir();

                if (IsButtonJustPressed(GbaInput.A) && CanSafetyJump)
                {
                    State.MoveTo(Fsm_Jump);
                    return;
                }
                
                if (IsButtonJustPressed(GbaInput.A) && field27_0x9c == 0)
                {
                    State.MoveTo(Fsm_Helico);
                    return;
                }

                if (IsButtonJustPressed(GbaInput.A) && field27_0x9c != 0)
                {
                    State.MoveTo(FUN_0802ddac);
                    return;
                }

                if (IsNearHangableEdge())
                {
                    State.MoveTo(Fsm_HangOnEdge);
                    return;
                }

                if (HasLanded())
                {
                    NextActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                    State.MoveTo(Fsm_Default);
                    return;
                }

                if (IsOnHangable())
                {
                    BeginHang();
                    State.MoveTo(Fsm_Hang);
                    return;
                }

                if (IsOnClimbableVertical() == 1)
                {
                    State.MoveTo(Fsm_Climb);
                    return;
                }

                // TODO: Implement
                //if (CheckInput(GbaInput.L) && IsOnWallJumpable())
                //{
                //    FUN_0802a4c0();
                //    Fsm.ChangeAction(FUN_08031554);
                //}
                break;

            case FsmAction.UnInit:
                CanSafetyJump = false;
                break;
        }
    }

    private void Fsm_Helico(FsmAction action)
    {
        CameraSideScroller cam = (CameraSideScroller)Scene.Camera;

        switch (action)
        {
            case FsmAction.Init:
                PlaySound(Rayman3SoundEvent.Play__Helico01_Mix10);

                if (ActionId is Action.BouncyJump_Right or Action.BouncyJump_Left)
                    ActionId = IsFacingRight ? Action.BouncyHelico_Right : Action.BouncyHelico_Left;
                else
                    ActionId = IsFacingRight ? Action.Helico_Right : Action.Helico_Left;

                NextActionId = null;
                Timer = GameTime.ElapsedFrames;
                break;

            case FsmAction.Step:
                if (!FsmStep_DoInTheAir())
                    return;
                
                AttackInTheAir();
                SlowdownAirSpeed();
                MoveInTheAir(PreviousXSpeed);

                if (IsNearHangableEdge())
                {
                    State.MoveTo(Fsm_HangOnEdge);
                    return;
                }

                if (HasLanded())
                {
                    NextActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                    State.MoveTo(Fsm_Default);
                    return;
                }

                if (IsButtonJustPressed(GbaInput.A) || IsButtonJustPressed(GbaInput.B))
                {
                    State.MoveTo(Fsm_StopHelico);
                    return;
                }

                if (GameTime.ElapsedFrames - Timer > 40)
                {
                    State.MoveTo(Fsm_TimeoutHelico);
                    return;
                }

                if (IsOnHangable())
                {
                    BeginHang();
                    State.MoveTo(Fsm_Hang);
                    return;
                }

                if (IsOnClimbableVertical() == 1)
                {
                    State.MoveTo(Fsm_Climb);
                    return;
                }

                // TODO: Implement
                //if (CheckInput(GbaInput.L) && IsOnWallJumpable())
                //{
                //    FUN_0802a4c0();
                //    Fsm.ChangeAction(FUN_08031554);
                //}


                // TODO: Implement

                break;
            
            case FsmAction.UnInit:
                PreviousXSpeed = 0;
                
                if (IsLocalPlayer)
                    cam.ProcessMessage(this, Message.Cam_ResetPositionX);

                PlaySound(Rayman3SoundEvent.Stop__Helico01_Mix10);

                if (GameTime.ElapsedFrames - Timer <= 40)
                    PlaySound(Rayman3SoundEvent.Play__HeliCut_Mix01);
                break;
        }
    }

    private void Fsm_StopHelico(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                NextActionId = null;
                ActionId = IsFacingRight ? Action.Fall_Right : Action.Fall_Left;
                break;

            case FsmAction.Step:
                if (!FsmStep_DoInTheAir())
                    return;

                MoveInTheAir(PreviousXSpeed);
                AttackInTheAir();

                if (IsNearHangableEdge())
                {
                    State.MoveTo(Fsm_HangOnEdge);
                    return;
                }

                if (HasLanded())
                {
                    NextActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                    State.MoveTo(Fsm_Default);
                    return;
                }

                if (IsButtonJustPressed(GbaInput.A) && field27_0x9c != 0)
                {
                    State.MoveTo(FUN_0802ddac);
                    return;
                }

                if (IsOnClimbableVertical() == 1)
                {
                    State.MoveTo(Fsm_Climb);
                    return;
                }

                // TODO: Implement
                //if (CheckInput(GbaInput.L) && IsOnWallJumpable())
                //{
                //    FUN_0802a4c0();
                //    Fsm.ChangeAction(FUN_08031554);
                //}
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_TimeoutHelico(FsmAction action)
    {
        CameraSideScroller cam = (CameraSideScroller)Scene.Camera;

        switch (action)
        {
            case FsmAction.Init:
                NextActionId = null;
                ActionId = IsFacingRight ? Action.HelicoTimeout_Right : Action.HelicoTimeout_Left;
                Timer = 0;
                PlaySound(Rayman3SoundEvent.Play__HeliStop_Mix06);
                break;

            case FsmAction.Step:
                if (!FsmStep_DoInTheAir())
                    return;

                Timer++;
                AttackInTheAir();
                SlowdownAirSpeed();
                MoveInTheAir(PreviousXSpeed);

                if (IsNearHangableEdge())
                {
                    State.MoveTo(Fsm_HangOnEdge);
                    return;
                }

                if (HasLanded())
                {
                    NextActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                    PlaySound(Rayman3SoundEvent.Play__HeliCut_Mix01);
                    State.MoveTo(Fsm_Default);
                    return;
                }

                if (IsButtonJustPressed(GbaInput.A) || IsButtonJustPressed(GbaInput.B) || Timer > 50)
                {
                    State.MoveTo(Fsm_StopHelico);
                    return;
                }

                if (IsOnHangable())
                {
                    BeginHang();
                    State.MoveTo(Fsm_Hang);
                    return;
                }

                if (IsOnClimbableVertical() == 1)
                {
                    State.MoveTo(Fsm_Climb);
                    return;
                }

                // TODO: Implement
                //if (CheckInput(GbaInput.L) && IsOnWallJumpable())
                //{
                //    FUN_0802a4c0();
                //    Fsm.ChangeAction(FUN_08031554);
                //}

                if (field27_0x9c != 0)
                {
                    State.MoveTo(FUN_0802ddac);
                    return;
                }
                break;

            case FsmAction.UnInit:
                PlaySound(Rayman3SoundEvent.Stop__HeliStop_Mix06);
                PreviousXSpeed = 0;

                if (IsLocalPlayer)
                    cam.ProcessMessage(this, Message.Cam_ResetPositionX);

                if (Timer > 50)
                {
                    Vector2 pos = Position;

                    pos += new Vector2(0, Tile.Size);
                    if (Scene.GetPhysicalType(pos) != PhysicalTypeValue.None)
                        return;

                    pos += new Vector2(0, Tile.Size);
                    if (Scene.GetPhysicalType(pos) != PhysicalTypeValue.None)
                        return;

                    pos += new Vector2(0, Tile.Size);
                    if (Scene.GetPhysicalType(pos) != PhysicalTypeValue.None)
                        return;

                    pos += new Vector2(0, Tile.Size);
                    if (Scene.GetPhysicalType(pos) != PhysicalTypeValue.None)
                        return;

                    PlaySound(Rayman3SoundEvent.Play__OnoPeur1_Mix03);
                }
                break;
        }
    }

    private void Fsm_Crouch(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (IsSliding)
                {
                    ActionId = IsFacingRight ? Action.Sliding_Crouch_Right : Action.Sliding_Crouch_Left;
                }
                else
                {
                    PlaySound(Rayman3SoundEvent.Stop__SkiLoop1);
                    
                    if (SlideType == null)
                        PreviousXSpeed = 0;

                    if (NextActionId == null)
                        ActionId = IsFacingRight ? Action.Crouch_Right : Action.Crouch_Left;
                    else
                        ActionId = NextActionId.Value;
                }

                // Custom detection box for when crouching
                SetDetectionBox(new Box(
                    minX: ActorModel.DetectionBox.MinX,
                    minY: ActorModel.DetectionBox.MaxY - 16,
                    maxX: ActorModel.DetectionBox.MaxX,
                    maxY: ActorModel.DetectionBox.MaxY));
                break;

            case FsmAction.Step:
                if (!FsmStep_Inlined_FUN_1004c544())
                    return;

                if (IsActionFinished && ActionId is Action.CrouchDown_Right or Action.CrouchDown_Left)
                {
                    ActionId = IsFacingRight ? Action.Crouch_Right : Action.Crouch_Left;
                    NextActionId = null;
                }

                if (IsSliding)
                {
                    SlidingOnSlippery();
                }
                else
                {
                    PlaySound(Rayman3SoundEvent.Stop__SkiLoop1);

                    if (ActionId is not (Action.Crouch_Right or Action.Crouch_Left or Action.CrouchDown_Right or Action.CrouchDown_Left))
                        ActionId = IsFacingRight ? Action.Crouch_Right : Action.Crouch_Left;
                }

                Box detectionBox = GetDetectionBox();

                PhysicalType topType = Scene.GetPhysicalType(detectionBox.TopLeft + new Vector2(1, -Tile.Size));

                if (!topType.IsSolid)
                    topType = Scene.GetPhysicalType(detectionBox.TopRight + new Vector2(-1, -Tile.Size));

                // Change direction
                if (IsButtonPressed(GbaInput.Left) && IsFacingRight)
                {
                    ActionId = Action.Crawl_Left;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }
                else if (IsButtonPressed(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Crawl_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }

                // Let go of down and stop crouching
                if (IsButtonReleased(GbaInput.Down) && !topType.IsSolid)
                {
                    State.MoveTo(Fsm_Default);
                    return;
                }

                // Crawl
                if (IsButtonPressed(GbaInput.Left) || IsButtonPressed(GbaInput.Right))
                {
                    State.MoveTo(Fsm_Crawl);
                    return;
                }

                // Fall
                if (Speed.Y > 1)
                {
                    State.MoveTo(Fsm_Fall);
                    return;
                }

                // Jump
                if (IsButtonJustPressed(GbaInput.A) && !topType.IsSolid && Flag2_2)
                {
                    State.MoveTo(Fsm_Jump);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Restore detection box
                SetDetectionBox(new Box(ActorModel.DetectionBox));

                if (NextActionId == ActionId || ActionId is Action.Crawl_Right or Action.Crawl_Left)
                    NextActionId = null;
                break;
        }
    }

    private void Fsm_Crawl(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (IsSliding)
                {
                    ActionId = IsFacingRight ? Action.Sliding_Crouch_Right : Action.Sliding_Crouch_Left;
                }
                else
                {
                    PlaySound(Rayman3SoundEvent.Stop__SkiLoop1);

                    if (SlideType == null)
                        PreviousXSpeed = 0;

                    if (NextActionId == null)
                        ActionId = IsFacingRight ? Action.Crawl_Right : Action.Crawl_Left;
                    else
                        ActionId = NextActionId.Value;
                }

                NextActionId = null;

                // Custom detection box for when crouching
                SetDetectionBox(new Box(
                    minX: ActorModel.DetectionBox.MinX,
                    minY: ActorModel.DetectionBox.MaxY - 16,
                    maxX: ActorModel.DetectionBox.MaxX,
                    maxY: ActorModel.DetectionBox.MaxY));
                break;

            case FsmAction.Step:
                if (!FsmStep_Inlined_FUN_1004c544())
                    return;

                Box detectionBox = GetDetectionBox();

                PhysicalType topType = Scene.GetPhysicalType(detectionBox.TopLeft + new Vector2(1, -Tile.Size));

                if (!topType.IsSolid)
                    topType = Scene.GetPhysicalType(detectionBox.TopRight + new Vector2(-1, -Tile.Size));

                // Change direction
                if (IsButtonPressed(GbaInput.Left) && IsFacingRight)
                {
                    ActionId = Action.Crawl_Left;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }
                else if (IsButtonPressed(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Crawl_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }

                if (IsSliding)
                {
                    SlidingOnSlippery();
                }
                else
                {
                    PlaySound(Rayman3SoundEvent.Stop__SkiLoop1);

                    if (ActionId is not (Action.Crawl_Right or Action.Crawl_Left))
                        ActionId = IsFacingRight ? Action.Crawl_Right : Action.Crawl_Left;
                }

                // Walk
                if (IsButtonReleased(GbaInput.Down) && (IsButtonPressed(GbaInput.Left) || IsButtonPressed(GbaInput.Right)) && !topType.IsSolid)
                {
                    State.MoveTo(Fsm_Walk);
                    return;
                }

                // Stopped crouching/crawling
                if (IsButtonReleased(GbaInput.Down) && IsButtonReleased(GbaInput.Left) && IsButtonReleased(GbaInput.Right) && !topType.IsSolid)
                {
                    State.MoveTo(Fsm_Default);
                    return;
                }

                // Crouch
                if (IsButtonReleased(GbaInput.Right) && IsButtonReleased(GbaInput.Left))
                {
                    State.MoveTo(Fsm_Crouch);
                    return;
                }

                // Jump
                if (IsButtonJustPressed(GbaInput.A) && !topType.IsSolid)
                {
                    State.MoveTo(Fsm_Jump);
                    return;
                }

                // Fall
                if (Speed.Y > 1)
                {
                    State.MoveTo(Fsm_Fall);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Restore detection box
                SetDetectionBox(new Box(ActorModel.DetectionBox));
                break;
        }
    }

    private void Fsm_ChargeAttack(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                PreviousXSpeed = 0;
                NextActionId = null;

                // Climb
                if (ActionId is Action.Climb_BeginChargeFist_Right or Action.Climb_BeginChargeFist_Left)
                {
                    Timer = 0;
                }
                // Hang
                else if (ActionId is
                         Action.Hang_Move_Right or Action.Hang_Move_Left or
                         Action.Hang_Idle_Right or Action.Hang_Idle_Left or
                         Action.Hang_Attack_Right or Action.Hang_Attack_Left or
                         Action.Hang_EndMove_Right or Action.Hang_EndMove_Left)
                {
                    // Probably a bug in the GBA code since this causes the sound to play twice. This was fixed for N-Gage.
                    if (Engine.Settings.Platform == Platform.GBA)
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Charge_Mix05);

                    ActionId = IsFacingRight ? Action.Hang_ChargeAttack_Right : Action.Hang_ChargeAttack_Left;
                    Timer = GameTime.ElapsedFrames;
                }
                // Hang on edge
                else if (ActionId is 
                         Action.HangOnEdge_Begin_Right or Action.HangOnEdge_Begin_Left or
                         Action.HangOnEdge_Idle_Right or Action.HangOnEdge_Idle_Left or
                         Action.HangOnEdge_EndAttack_Right or Action.HangOnEdge_EndAttack_Left)
                {
                    ActionId = IsFacingRight ? Action.HangOnEdge_BeginAttack_Right : Action.HangOnEdge_BeginAttack_Left;
                    Timer = 0;
                }
                // Normal fist attack
                else if (CanAttackWithFist(1))
                {
                    ActionId = IsFacingRight ? Action.BeginChargeFist_Right : Action.BeginChargeFist_Left;
                    Timer = 0;
                }
                // Second normal fist attack
                else
                {
                    ActionId = IsFacingRight ? Action.BeginChargeSecondFist_Right : Action.BeginChargeSecondFist_Left;
                    Timer = 0;
                }

                PlaySound(Rayman3SoundEvent.Play__Charge_Mix05);
                break;

            case FsmAction.Step:
                // Check for damage
                if (AttachedObject?.Type == (int)ActorType.Plum)
                {
                    if (!FsmStep_FUN_08020e8c())
                        return;

                    if (Engine.Settings.Platform == Platform.NGage)
                    {
                        CameraSideScroller cam = (CameraSideScroller)Scene.Camera;

                        if (IsFacingRight)
                            cam.HorizontalOffset = Speed.X < 0 ? CameraOffset.DefaultReversed : CameraOffset.Default;
                        else
                            cam.HorizontalOffset = Speed.X < 0 ? CameraOffset.Default : CameraOffset.DefaultReversed;
                    }
                }
                else
                {
                    if (!FsmStep_FUN_08020ee0())
                        return;
                }

                // Change direction (if not hanging on an edge)
                if (ActionId is not (
                    Action.HangOnEdge_ChargeAttack_Right or Action.HangOnEdge_ChargeAttack_Left or
                    Action.HangOnEdge_BeginAttack_Right or Action.HangOnEdge_BeginAttack_Left))
                {
                    if (IsButtonPressed(GbaInput.Left))
                    {
                        if (IsFacingRight)
                            AnimatedObject.FlipX = true;
                    }
                    else if (IsButtonPressed(GbaInput.Right))
                    {
                        if (IsFacingLeft)
                            AnimatedObject.FlipX = false;
                    }
                }

                // Update action
                if (IsActionFinished && Timer == 0)
                {
                    if (ActionId is Action.BeginChargeFist_Right or Action.BeginChargeFist_Left)
                    {
                        ActionId = IsFacingRight ? Action.ChargeFist_Right : Action.ChargeFist_Left;
                        Timer = GameTime.ElapsedFrames;
                    }
                    else if (ActionId is Action.BeginChargeSecondFist_Right or Action.BeginChargeSecondFist_Left)
                    {
                        ActionId = IsFacingRight ? Action.ChargeSecondFist_Right : Action.ChargeSecondFist_Left;
                        Timer = GameTime.ElapsedFrames;
                    }
                    else if (ActionId is Action.Climb_BeginChargeFist_Right or Action.Climb_BeginChargeFist_Left)
                    {
                        ActionId = IsFacingRight ? Action.Climb_ChargeFist_Right : Action.Climb_ChargeFist_Left;
                        Timer = GameTime.ElapsedFrames;
                    }
                    else if (ActionId is Action.HangOnEdge_BeginAttack_Right or Action.HangOnEdge_BeginAttack_Left)
                    {
                        ActionId = IsFacingRight ? Action.HangOnEdge_ChargeAttack_Right : Action.HangOnEdge_ChargeAttack_Left;
                        Timer = GameTime.ElapsedFrames;
                    }

                    if (Engine.Settings.Platform == Platform.NGage && AttachedObject?.Type == (int)ActorType.Plum && IsLocalPlayer)
                    {
                        CameraSideScroller cam = (CameraSideScroller)Scene.Camera;
                        cam.HorizontalOffset = CameraOffset.DefaultBigger;
                    }
                }

                // TODO: Sound loops wrong
                // Super fist after 20 frames
                if (HasPower(Power.SuperFist) && GameTime.ElapsedFrames - Timer == 20)
                {
                    if (ActionId is Action.ChargeFist_Right or Action.ChargeFist_Left)
                    {
                        ActionId = IsFacingRight ? Action.ChargeSuperFist_Right : Action.ChargeSuperFist_Left;
                        PlaySound(Rayman3SoundEvent.Stop__Charge_Mix05);
                        PlaySound(Rayman3SoundEvent.Play__Charge2_Mix04);
                    }
                    else if (ActionId is Action.ChargeSecondFist_Right or Action.ChargeSecondFist_Left)
                    {
                        ActionId = IsFacingRight ? Action.ChargeSecondSuperFist_Right : Action.ChargeSecondSuperFist_Left;
                        PlaySound(Rayman3SoundEvent.Stop__Charge_Mix05);
                        PlaySound(Rayman3SoundEvent.Play__Charge2_Mix04);
                    }
                    else if (ActionId is Action.Climb_ChargeFist_Right or Action.Climb_ChargeFist_Left)
                    {
                        ActionId = IsFacingRight ? Action.Climb_ChargeSuperFist_Right : Action.Climb_ChargeSuperFist_Left;
                        PlaySound(Rayman3SoundEvent.Stop__Charge_Mix05);
                        PlaySound(Rayman3SoundEvent.Play__Charge2_Mix04);
                    }
                }

                int type = 0;

                // Stop charging and perform attack
                if (IsButtonReleased2(GbaInput.B))
                {
                    if (Timer == 0)
                        Timer = GameTime.ElapsedFrames;

                    uint chargePower = GameTime.ElapsedFrames - Timer;

                    // Move plum
                    if (AttachedObject?.Type == (int)ActorType.Plum)
                    {
                        // TODO: Implement and handle message to move plum
                        AttachedObject.ProcessMessage(this, IsFacingRight ? (Message)0x40c : (Message)0x40d, chargePower);
                    }

                    if (ActionId is
                        Action.ChargeFist_Right or Action.ChargeFist_Left or
                        Action.BeginChargeFist_Right or Action.BeginChargeFist_Left)
                    {
                        Attack(chargePower, RaymanBody.RaymanBodyPartType.Fist, new Vector2(16, -16), ActionId is Action.ChargeFist_Right or Action.ChargeFist_Left);
                        NextActionId = IsFacingRight ? Action.EndChargeFist_Right : Action.EndChargeFist_Left;

                        if ((GameInfo.Powers & Power.DoubleFist) == 0)
                            field23_0x98 = 0;
                        type = 1;
                    }
                    else if (ActionId is 
                             Action.ChargeSecondFist_Right or Action.ChargeSecondFist_Left or
                             Action.BeginChargeSecondFist_Right or Action.BeginChargeSecondFist_Left)
                    {
                        Attack(chargePower, RaymanBody.RaymanBodyPartType.SecondFist, new Vector2(16, -16), ActionId is Action.ChargeSecondFist_Right or Action.ChargeSecondFist_Left);
                        NextActionId = IsFacingRight ? Action.EndChargeSecondFist_Right : Action.EndChargeSecondFist_Left;

                        field23_0x98 = 0;
                        type = 1;
                    }
                    else if (ActionId is Action.ChargeSuperFist_Right or Action.ChargeSuperFist_Left)
                    {
                        Attack(chargePower, RaymanBody.RaymanBodyPartType.SuperFist, new Vector2(16, -16), true);
                        NextActionId = IsFacingRight ? Action.EndChargeFist_Right : Action.EndChargeFist_Left;

                        type = 1;
                    }
                    else if (ActionId is Action.ChargeSecondSuperFist_Right or Action.ChargeSecondSuperFist_Left)
                    {
                        Attack(chargePower, RaymanBody.RaymanBodyPartType.SecondSuperFist, new Vector2(16, -16), true);
                        NextActionId = IsFacingRight ? Action.EndChargeFist_Right : Action.EndChargeFist_Left;

                        field23_0x98 = 0;
                        type = 1;
                    }
                    else if (ActionId is Action.Hang_ChargeAttack_Right or Action.Hang_ChargeAttack_Left)
                    {
                        Attack(chargePower, RaymanBody.RaymanBodyPartType.Foot, new Vector2(16, 0), true);
                        NextActionId = IsFacingRight ? Action.Hang_Attack_Right : Action.Hang_Attack_Left;

                        type = 2;
                    }
                    else if (ActionId is 
                             Action.HangOnEdge_ChargeAttack_Right or Action.HangOnEdge_ChargeAttack_Left or
                             Action.HangOnEdge_BeginAttack_Right or Action.HangOnEdge_BeginAttack_Left)
                    {
                        Attack(chargePower, RaymanBody.RaymanBodyPartType.Foot, new Vector2(16, 16), true);
                        NextActionId = IsFacingRight ? Action.HangOnEdge_EndAttack_Right : Action.HangOnEdge_EndAttack_Left;

                        type = 3;
                    }
                    else if (ActionId is Action.Climb_ChargeSuperFist_Right or Action.Climb_ChargeSuperFist_Left)
                    {
                        Attack(chargePower, RaymanBody.RaymanBodyPartType.SuperFist, new Vector2(16, -32), true);
                        NextActionId = IsFacingRight ? Action.Climb_EndChargeFist_Right : Action.Climb_EndChargeFist_Left;

                        type = 4;
                    }
                    else if (ActionId is 
                             Action.Climb_ChargeFist_Right or Action.Climb_ChargeFist_Left or
                             Action.Climb_BeginChargeFist_Right or Action.Climb_BeginChargeFist_Left)
                    {
                        Attack(chargePower, RaymanBody.RaymanBodyPartType.Fist, new Vector2(16, -32), true);
                        NextActionId = IsFacingRight ? Action.Climb_EndChargeFist_Right : Action.Climb_EndChargeFist_Left;

                        type = 4;
                    }
                }

                if (ActionId is Action.Hang_ChargeAttack_Right or Action.Hang_ChargeAttack_Left && !IsOnHangable())
                {
                    IsHanging = false;
                    PlaySound(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                    State.MoveTo(Fsm_StopHelico);
                    return;
                }

                if (type == 2)
                {
                    State.MoveTo(Fsm_Hang);
                    return;
                }

                if (type == 4)
                {
                    State.MoveTo(Fsm_Climb);
                    return;
                }

                if (type == 3)
                {
                    State.MoveTo(Fsm_HangOnEdge);
                    return;
                }

                if (type == 1 && AttachedObject?.Type == (int)ActorType.Plum)
                {
                    State.MoveTo(FUN_080224f4);
                    return;
                }

                if (type == 1)
                {
                    State.MoveTo(Fsm_Default);
                    return;
                }

                // TODO: Why is this not on GBA?
                if (Engine.Settings.Platform == Platform.NGage &&
                    ActionId is Action.Damage_Shock_Right or Action.Damage_Shock_Left)
                {
                    ActionId = IsFacingRight ? Action.Damage_Hit_Right : Action.Damage_Hit_Left;
                    State.MoveTo(Fsm_Hit);
                    return;
                }

                if (Speed.Y > 1 && AttachedObject?.Type != (int)ActorType.Plum)
                {
                    State.MoveTo(Fsm_Fall);
                    return;
                }
                break;

            case FsmAction.UnInit:
                PlaySound(Rayman3SoundEvent.Stop__Charge_Mix05);
                PlaySound(Rayman3SoundEvent.Stop__Charge2_Mix04);

                if (IsLocalPlayer && Engine.Settings.Platform == Platform.NGage)
                {
                    CameraSideScroller cam = (CameraSideScroller)Scene.Camera;

                    if (RSMultiplayer.IsActive)
                    {
                        cam.HorizontalOffset = CameraOffset.Multiplayer;
                    }
                    else
                    {
                        cam.HorizontalOffset = CameraOffset.Default;
                        Flag1_D = true;
                        field16_0x91 = 0;
                    }
                }
                break;
        }
    }

    private void Fsm_Climb(FsmAction action)
    {
        CameraSideScroller cam = (CameraSideScroller)Scene.Camera;

        switch (action)
        {
            case FsmAction.Init:
                if (NextActionId == null)
                    ActionId = IsFacingRight ? Action.Climb_Idle_Right : Action.Climb_Idle_Left;
                else
                    ActionId = NextActionId.Value;

                PreviousXSpeed = 0;
                MechModel.Speed = Vector2.Zero;
                Timer = 0;
                break;

            case FsmAction.Step:
                if (!FsmStep_DoInTheAir())
                    return;

                Timer++;

                // Keep the same frame across all climbing animations
                int animFrame = AnimatedObject.CurrentFrame;
                bool jump = IsButtonJustPressed(GbaInput.A);

                int climbHoriontal = IsOnClimbableHorizontal();
                int climbVertical = IsOnClimbableVertical();

                PhysicalType type = PhysicalTypeValue.None;

                MechModel.Speed = new Vector2(0, MechModel.Speed.Y);

                if (IsButtonPressed(GbaInput.Left))
                {
                    if (IsButtonJustPressed2(GbaInput.Left))
                        Timer = 0;

                    if (climbHoriontal is 4 or 6)
                    {
                        if (!RSMultiplayer.IsActive)
                        {
                            MechModel.Speed = new Vector2(-1.5f, MechModel.Speed.Y);

                            if (Timer > 50)
                                Timer = 0;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else if (Timer > 50 && !RSMultiplayer.IsActive)
                    {
                        cam.HorizontalOffset = CameraOffset.Default;
                        Timer = 0;
                    }
                }
                else if (IsButtonPressed(GbaInput.Right))
                {
                    if (IsButtonJustPressed2(GbaInput.Right))
                        Timer = 0;

                    if (climbHoriontal is 4 or 5)
                    {
                        if (!RSMultiplayer.IsActive)
                        {
                            MechModel.Speed = new Vector2(1.5f, MechModel.Speed.Y);

                            if (Timer > 50)
                                Timer = 0;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else if (Timer > 50 && !RSMultiplayer.IsActive)
                    {
                        cam.HorizontalOffset = CameraOffset.Default;
                        Timer = 0;
                    }
                }
                else if (Timer > 50 && !RSMultiplayer.IsActive)
                {
                    // Center camera, only on GBA
                    if (Engine.Settings.Platform == Platform.GBA)
                        cam.HorizontalOffset = CameraOffset.Center;

                    Timer = 0;
                }

                if (IsButtonPressed(GbaInput.Up) && climbVertical is 1 or 2)
                {
                    if (!RSMultiplayer.IsActive)
                    {
                        MechModel.Speed = new Vector2(MechModel.Speed.X, -1.5f);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    if (ActionId is not (Action.Climb_Up_Right or Action.Climb_Up_Left))
                    {
                        ActionId = IsFacingRight ? Action.Climb_Up_Right : Action.Climb_Up_Left;
                        AnimatedObject.CurrentFrame = animFrame;
                    }
                }
                else if (IsButtonPressed(GbaInput.Down) && climbVertical is 1 or 3)
                {
                    if (!RSMultiplayer.IsActive)
                    {
                        MechModel.Speed = new Vector2(MechModel.Speed.X, 1.5f);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    if (ActionId is not (Action.Climb_Down_Right or Action.Climb_Down_Left))
                    {
                        ActionId = IsFacingRight ? Action.Climb_Down_Right : Action.Climb_Down_Left;
                        AnimatedObject.CurrentFrame = animFrame;
                    }
                }
                else
                {
                    MechModel.Speed = new Vector2(MechModel.Speed.X, 0);

                    if (IsButtonPressed(GbaInput.Left) && ActionId != Action.Climb_Side_Left && Speed.X != 0)
                    {
                        ActionId = Action.Climb_Side_Left;
                        AnimatedObject.CurrentFrame = animFrame;
                    }
                    else if (IsButtonPressed(GbaInput.Right) && ActionId != Action.Climb_Side_Right && Speed.X != 0)
                    {
                        ActionId = Action.Climb_Side_Right;
                        AnimatedObject.CurrentFrame = animFrame;
                    }

                    if (IsButtonPressed(GbaInput.Down) && climbVertical is not (1 or 3))
                    {
                        type = Scene.GetPhysicalType(Position + new Vector2(0, 32));
                    }
                }

                if (Speed == Vector2.Zero)
                {
                    if (IsButtonPressed(GbaInput.Left) && IsFacingRight)
                        AnimatedObject.FlipX = true;
                    else if (IsButtonPressed(GbaInput.Right) && IsFacingLeft)
                        AnimatedObject.FlipX = false;

                    if (ActionId == NextActionId)
                    {
                        if (IsActionFinished)
                        {
                            ActionId = IsFacingRight ? Action.Climb_Idle_Right : Action.Climb_Idle_Left;
                            NextActionId = null;
                        }
                    }
                    else
                    {
                        if (IsActionFinished && ActionId is (Action.Climb_BeginIdle_Right or Action.Climb_BeginIdle_Left))
                        {
                            ActionId = IsFacingRight ? Action.Climb_Idle_Right : Action.Climb_Idle_Left;
                        }
                        else if (ActionId is not (Action.Climb_Idle_Right or Action.Climb_Idle_Left or Action.Climb_BeginIdle_Right or Action.Climb_BeginIdle_Left))
                        {
                            ActionId = IsFacingRight ? Action.Climb_BeginIdle_Right : Action.Climb_BeginIdle_Left;
                        }
                    }
                }

                // Punch
                if (IsButtonPressed2(GbaInput.B) && CanAttackWithFist(1))
                {
                    ActionId = IsFacingRight ? Action.Climb_BeginChargeFist_Right : Action.Climb_BeginChargeFist_Left;
                    State.MoveTo(Fsm_ChargeAttack);
                    return;
                }

                // Jump left
                if (jump && IsButtonPressed(GbaInput.Left) && climbHoriontal is not (4 or 6))
                {
                    State.MoveTo(Fsm_Jump);
                    return;
                }

                // Jump right
                if (jump && IsButtonPressed(GbaInput.Right) && climbHoriontal is not (4 or 5))
                {
                    State.MoveTo(Fsm_Jump);
                    return;
                }

                // Jump up
                if (jump && IsButtonPressed(GbaInput.Up) && climbVertical is not (1 or 2))
                {
                    State.MoveTo(Fsm_Jump);
                    return;
                }

                // Move down
                if (type.IsSolid && IsButtonPressed(GbaInput.Down) && climbVertical is not (1 or 3))
                {
                    State.MoveTo(Fsm_Fall);
                    return;
                }

                // Jump down
                if (jump && IsButtonPressed(GbaInput.Down) && climbVertical is not (1 or 3))
                {
                    PlaySound(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                    State.MoveTo(Fsm_Fall);
                    return;
                }
                break;

            case FsmAction.UnInit:
                if (!RSMultiplayer.IsActive)
                    cam.HorizontalOffset = CameraOffset.Default;

                if (!IsButtonJustPressed(GbaInput.B) && IsLocalPlayer)
                    cam.ProcessMessage(this, Message.Cam_ResetPositionX);

                if (ActionId == NextActionId)
                    NextActionId = null;
                break;
        }
    }

    private void Fsm_Hang(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (NextActionId != null)
                    ActionId = NextActionId.Value;
                else
                    ActionId = IsFacingRight ? Action.Hang_Idle_Right : Action.Hang_Idle_Left;
                break;

            case FsmAction.Step:
                if (!FsmStep_DoInTheAir())
                    return;

                if (IsActionFinished && ActionId == NextActionId)
                {
                    ActionId = IsFacingRight ? Action.Hang_Idle_Right : Action.Hang_Idle_Left;
                    NextActionId = null;
                }

                // Change direction
                if (IsButtonPressed(GbaInput.Left) && IsFacingRight)
                {
                    ActionId = Action.Hang_Move_Left;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                    {
                        throw new NotImplementedException();
                    }
                }
                else if (IsButtonPressed(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Hang_Move_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                    {
                        throw new NotImplementedException();
                    }
                }

                // Move
                if (IsButtonPressed(GbaInput.Left) || IsButtonPressed(GbaInput.Right))
                {
                    State.MoveTo(Fsm_HangMove);
                    return;
                }

                // Move down
                if (IsButtonPressed(GbaInput.Down))
                {
                    Position += new Vector2(0, Tile.Size);
                    IsHanging = false;
                    PlaySound(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                    State.MoveTo(Fsm_Fall);
                    return;
                }

                // No longer hanging
                if (!IsOnHangable())
                {
                    IsHanging = false;
                    PlaySound(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                    State.MoveTo(Fsm_Default);
                    return;
                }

                // Attack
                if (IsButtonJustPressed(GbaInput.B) && CanAttackWithFoot())
                {
                    State.MoveTo(Fsm_ChargeAttack);
                    return;
                }
                break;

            case FsmAction.UnInit:
                if (ActionId == NextActionId || ActionId is Action.Hang_Move_Right or Action.Hang_Move_Left)
                    NextActionId = null;
                break;
        }
    }

    private void Fsm_HangMove(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsFacingRight ? Action.Hang_Move_Right : Action.Hang_Move_Left;
                NextActionId = null;
                break;

            case FsmAction.Step:
                if (!FsmStep_DoInTheAir())
                    return;

                if (IsActionFinished && ActionId == NextActionId)
                {
                    ActionId = IsFacingRight ? Action.Hang_Move_Right : Action.Hang_Move_Left;
                    NextActionId = null;
                }

                // Change direction
                if (IsButtonPressed(GbaInput.Left) && IsFacingRight)
                {
                    ActionId = Action.Hang_Move_Left;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                    {
                        throw new NotImplementedException();
                    }
                }
                else if (IsButtonPressed(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Hang_Move_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                    {
                        throw new NotImplementedException();
                    }
                }

                // Stop moving
                if (!IsButtonPressed(GbaInput.Left) && !IsButtonPressed(GbaInput.Right))
                {
                    NextActionId = IsFacingRight ? Action.Hang_EndMove_Right : Action.Hang_EndMove_Left;
                    State.MoveTo(Fsm_Hang);
                    return;
                }

                // Move down
                if (IsButtonPressed(GbaInput.Down))
                {
                    Position += new Vector2(0, Tile.Size);
                    IsHanging = false;
                    PlaySound(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                    State.MoveTo(Fsm_Fall);
                    return;
                }

                // No longer hanging
                if (!IsOnHangable())
                {
                    IsHanging = false;
                    PlaySound(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                    State.MoveTo(Fsm_Default);
                    return;
                }

                // Attack
                if (IsButtonJustPressed(GbaInput.B) && CanAttackWithFoot())
                {
                    State.MoveTo(Fsm_ChargeAttack);
                    return;
                }
                break;

            case FsmAction.UnInit:
                if (ActionId == NextActionId)
                    NextActionId = null;
                break;
        }
    }

    private void Fsm_Swing(FsmAction action)
    {
        CameraSideScroller cam = (CameraSideScroller)Scene.Camera;

        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.Swing;
                ChangeAction();
                NextActionId = null;

                Timer = (uint)(MathF.Atan2(Position.Y - AttachedObject.Position.Y, Position.X - AttachedObject.Position.X) / (2 * MathF.PI) * 256);
                PreviousXSpeed = (Position - AttachedObject.Position).Length();

                if (Position.X < AttachedObject.Position.X)
                {
                    Flag2_1 = true;

                    if (Timer > 128)
                    {
                        AnimatedObject.CurrentFrame = 0;
                        Timer = 128;
                    }
                    else
                    {
                        AnimatedObject.CurrentFrame = (int)(Timer / 40);
                    }
                }
                else
                {
                    Flag2_1 = false;

                    if (Timer > 128)
                    {
                        AnimatedObject.CurrentFrame = 19;
                        Timer = 0;
                    }
                    else
                    {
                        AnimatedObject.CurrentFrame = (int)(Timer / 12 + 19);
                    }
                }

                cam.HorizontalOffset = AnimatedObject.CurrentFrame < 19 ? CameraOffset.Default : CameraOffset.DefaultReversed;

                CreateSwingProjectiles();
                PlaySound(Rayman3SoundEvent.Play__LumMauve_Mix02);
                break;

            case FsmAction.Step:
                if (!FsmStep_CheckDeath())
                    return;

                if (AnimatedObject.CurrentFrame == 19 && Timer == 0)
                {
                    cam.HorizontalOffset = CameraOffset.DefaultReversed;
                }
                else if (AnimatedObject.CurrentFrame == 39 && Timer == 128)
                {
                    cam.HorizontalOffset = CameraOffset.Default;
                }

                if (Flag2_1)
                {
                    if (Timer == 0)
                    {
                        Flag2_1 = false;
                        Timer = 0;
                        AnimatedObject.CurrentFrame = 19;
                    }
                    else
                    {
                        float xPos = AttachedObject.Position.X + MathHelpers.Cos256(Timer) * PreviousXSpeed;
                        float yPos = AttachedObject.Position.Y + MathHelpers.Sin256(Timer) * PreviousXSpeed;
                        Position = new Vector2(xPos, yPos);

                        if (PreviousXSpeed < 80)
                        {
                            // TODO: This is a bug in the src code! It's supposed to use Timer, not ActionId. Fix?
                            int value = (int)ActionId;
                            if (value is (< 10 or > 14) and (< 27 or > 34))
                                PreviousXSpeed += 0.5f;
                            else
                                PreviousXSpeed += 4;

                            if (PreviousXSpeed > 80)
                                PreviousXSpeed = 80;
                        }
                        else if (PreviousXSpeed > 80)
                        {
                            PreviousXSpeed -= 1;

                            if (PreviousXSpeed < 80)
                                PreviousXSpeed = 80;
                        }

                        if (Timer is < 4 or >= 125)
                            Timer -= 1;
                        else if (Timer is < 25 or >= 103)
                            Timer -= 1;
                        else if (Timer is < 51 or >= 77)
                            Timer -= 2;
                        else
                            Timer -= 2;
                    }
                }
                else
                {
                    if (Timer >= 128)
                    {
                        Flag2_1 = true;
                        Timer = 128;
                        AnimatedObject.CurrentFrame = 39;
                    }
                    else
                    {
                        float xPos = AttachedObject.Position.X + MathHelpers.Cos256(Timer) * PreviousXSpeed;
                        float yPos = AttachedObject.Position.Y + MathHelpers.Sin256(Timer) * PreviousXSpeed;
                        Position = new Vector2(xPos, yPos);

                        if (PreviousXSpeed < 80)
                        {
                            // TODO: This is a bug in the src code! It's supposed to use Timer, not ActionId. Fix?
                            int value = (int)ActionId;
                            if (value is (< 10 or > 14) and (< 27 or > 34))
                                PreviousXSpeed += 0.5f;
                            else
                                PreviousXSpeed += 4;

                            if (PreviousXSpeed > 80)
                                PreviousXSpeed = 80;
                        }
                        else if (PreviousXSpeed > 80)
                        {
                            PreviousXSpeed -= 1;

                            if (PreviousXSpeed < 80)
                                PreviousXSpeed = 80;
                        }

                        if (Timer is < 4 or >= 125)
                            Timer += 1;
                        else if (Timer is < 25 or >= 103)
                            Timer += 1;
                        else if (Timer is < 51 or >= 77)
                            Timer += 2;
                        else
                            Timer += 2;
                    }
                }

                if (IsButtonJustPressed(GbaInput.A) && !Scene.GetPhysicalType(Position).IsSolid)
                    State.MoveTo(Fsm_Jump);
                break;

            case FsmAction.UnInit:
                AttachedObject = null;

                // Momentum and direction
                if (Flag2_1)
                {
                    PreviousXSpeed = 1;
                }
                else
                {
                    PreviousXSpeed = -1;
                    ActionId = Action.Jump_Left;
                    ChangeAction();
                }

                Flag2_1 = false;

                cam.HorizontalOffset = GameInfo.MapId == MapId.TheCanopy_M2 ? CameraOffset.Center : CameraOffset.Default;
                break;
        }
    }

    private void Fsm_Bounce(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                NextActionId = null;
                ActionId = IsFacingRight ? Action.BeginBounce_Right : Action.BeginBounce_Left;
                break;

            case FsmAction.Step:
                if (!FsmStep_DoInTheAir())
                    return;

                if (Flag1_5)
                {
                    Flag1_5 = false;

                    if (Engine.Settings.Platform == Platform.NGage)
                        ActionId = IsFacingRight ? Action.BouncyJump_Right : Action.BouncyJump_Left;

                    State.MoveTo(Fsm_Jump);
                }
                break;

            case FsmAction.UnInit:
                if (Engine.Settings.Platform != Platform.NGage)
                    ActionId = IsFacingRight ? Action.BouncyJump_Right : Action.BouncyJump_Left;
                break;
        }
    }

    private void Fsm_PickUpObject(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                PlaySound(Rayman3SoundEvent.Play__OnoEfor2_Mix03);
                ActionId = IsFacingRight ? Action.PickUpObject_Right : Action.PickUpObject_Left;
                NextActionId = null;
                break;

            case FsmAction.Step:
                if (!FsmStep_Inlined_FUN_1004c544())
                    return;

                // Sync the objects position with Rayman
                Vector2 objOffset = AnimatedObject.CurrentFrame switch
                {
                    0 => new Vector2(23, 19),
                    1 => new Vector2(23, 19),
                    2 => new Vector2(23, 19),
                    3 => new Vector2(23, 18),
                    4 => new Vector2(23, 18),
                    5 => new Vector2(25, 16),
                    6 => new Vector2(30, 4),
                    7 => new Vector2(23, -15),
                    8 => new Vector2(15, -15),
                    9 => new Vector2(11, -15),
                    10 => new Vector2(1, -15),
                    11 => new Vector2(-3, -1),
                    12 => new Vector2(-3, 3),
                    13 => new Vector2(-3, 2),
                    14 => new Vector2(-3, 0),
                    15 => new Vector2(-3, -2),
                    _ => throw new Exception("Invalid frame index")
                };

                if (IsFacingRight)
                    AttachedObject.Position = Position + new Vector2(objOffset.X + 6, objOffset.Y - 22);
                else
                    AttachedObject.Position = Position + new Vector2(-objOffset.X - 4, objOffset.Y - 22);

                OffsetCarryingObject();

                if (IsActionFinished)
                    State.MoveTo(Fsm_CarryObject);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_CatchObject(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                PlaySound(Rayman3SoundEvent.Play__OnoEfor2_Mix03);
                ActionId = IsFacingRight ? Action.CatchObject_Right : Action.CatchObject_Left;

                if (IsActionFinished)
                {
                    if (IsFacingRight)
                        AttachedObject.Position = new Vector2(Position.X + 8, AttachedObject.Position.Y);
                    else
                        AttachedObject.Position = new Vector2(Position.X - 8, AttachedObject.Position.Y);
                }

                NextActionId = null;
                break;

            case FsmAction.Step:
                if (!FsmStep_Inlined_FUN_1004c544())
                    return;

                // Sync the objects position with Rayman
                float objYOffset = AnimatedObject.CurrentFrame switch
                {
                    0 => -36,
                    1 => -27,
                    2 => 0,
                    3 => 7,
                    4 => 8,
                    5 => 8,
                    6 => 6,
                    7 => 0,
                    8 => 0,
                    9 => 0,
                    10 => 0,
                    _ => throw new Exception("Invalid frame index")
                };

                if (IsFacingRight)
                    AttachedObject.Position = Position + new Vector2(6, objYOffset - 20);
                else
                    AttachedObject.Position = Position + new Vector2(-4, objYOffset - 20);

                OffsetCarryingObject();

                if (IsActionFinished)
                    State.MoveTo(Fsm_CarryObject);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_CarryObject(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                NextActionId = null;
                ActionId = IsFacingRight ? Action.CarryObject_Right : Action.CarryObject_Left;

                if (IsFacingRight)
                    AttachedObject.Position = Position + new Vector2(4, -22);
                else
                    AttachedObject.Position = Position + new Vector2(-4, -22);

                OffsetCarryingObject();
                break;

            case FsmAction.Step:
                if (!FsmStep_FUN_08020ee0())
                    return;

                // Change direction
                if (IsButtonPressed(GbaInput.Left) && IsFacingRight)
                {
                    ActionId = Action.CarryObject_Left;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }
                else if (IsButtonPressed(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.CarryObject_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }

                if (IsFacingRight)
                    AttachedObject.Position = Position + new Vector2(6, -22);
                else
                    AttachedObject.Position = Position + new Vector2(-4, -22);

                // Sync the objects position with Rayman
                float objYOffset = AnimatedObject.CurrentFrame switch
                {
                    0 or 1 => -22,
                    2 or 3 or 4 or 5 => -23,
                    6 or 7 => -22,
                    8 or 9 or 10 => -21,
                    _ => -22
                };

                AttachedObject.Position = new Vector2(AttachedObject.Position.X, Position.Y + objYOffset);

                OffsetCarryingObject();

                // Walk
                if (IsButtonPressed(GbaInput.Left) || IsButtonPressed(GbaInput.Right))
                {
                    State.MoveTo(Fsm_WalkWithObject);
                    return;
                }

                // Throw
                if (IsButtonJustPressed(GbaInput.A) || IsButtonJustPressed(GbaInput.B))
                {
                    State.MoveTo(Fsm_ThrowObject);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_WalkWithObject(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsFacingRight ? Action.WalkWithObject_Right : Action.WalkWithObject_Left;
                NextActionId = null;
                break;

            case FsmAction.Step:
                if (!FsmStep_FUN_08020ee0())
                    return;

                // Change direction
                if (IsButtonPressed(GbaInput.Left) && IsFacingRight)
                {
                    ActionId = Action.WalkWithObject_Left;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }
                else if (IsButtonPressed(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.WalkWithObject_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }

                // Sync the objects position with Rayman
                Vector2 objOffset = AnimatedObject.CurrentFrame switch
                {
                    0 => new Vector2(0, 0),
                    1 => new Vector2(0, 2),
                    2 => new Vector2(-1, 4),
                    3 => new Vector2(-2, 4),
                    4 => new Vector2(-3, 2),
                    5 => new Vector2(-4, 0),
                    6 => new Vector2(-4, -2),
                    7 => new Vector2(-3, -3),
                    8 => new Vector2(-1, -3),
                    9 => new Vector2(0, -3),
                    10 => new Vector2(0, 0),
                    11 => new Vector2(0, 2),
                    12 => new Vector2(-1, 4),
                    13 => new Vector2(-2, 4),
                    14 => new Vector2(-3, 2),
                    15 => new Vector2(-4, 0),
                    16 => new Vector2(-4, -2),
                    17 => new Vector2(-3, -3),
                    18 => new Vector2(-1, -3),
                    19 => new Vector2(0, -3),
                    _ => throw new Exception("Invalid frame index")
                };

                if (IsFacingRight)
                    AttachedObject.Position = Position + new Vector2(objOffset.X + 8, objOffset.Y - 22);
                else
                    AttachedObject.Position = Position + new Vector2(-objOffset.X - 4, objOffset.Y - 22);

                if (Speed.Y > 1)
                {
                    AttachedObject.ProcessMessage(this, Message.DropObject);
                    AttachedObject = null;
                }

                OffsetCarryingObject();

                // TODO: There's a bug here which causes a crash. Same in the original game. If falling it might still enter this state with attached obj null.
                // Stop walking
                if (IsButtonReleased(GbaInput.Left) && IsButtonReleased(GbaInput.Right))
                {
                    State.MoveTo(Fsm_CarryObject);
                    return;
                }

                // Throw
                if (IsButtonJustPressed(GbaInput.A) || IsButtonJustPressed(GbaInput.B))
                {
                    State.MoveTo(Fsm_ThrowObject);
                    return;
                }

                // Falling
                if (Speed.Y > 1)
                {
                    State.MoveTo(Fsm_Fall);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_ThrowObject(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                PlaySound(Rayman3SoundEvent.Play__OnoThrow_Mix02);

                if (IsButtonJustPressed(GbaInput.A))
                    ActionId = IsFacingRight ? Action.ThrowObjectUp_Right : Action.ThrowObjectUp_Left;
                else
                    ActionId = IsFacingRight ? Action.ThrowObjectForward_Right : Action.ThrowObjectForward_Left;

                NextActionId = null;
                break;

            case FsmAction.Step:
                if (!FsmStep_FUN_08020ee0())
                    return;

                if (ActionId is Action.ThrowObjectForward_Right or Action.ThrowObjectForward_Left)
                {
                    if (AnimatedObject.CurrentFrame == 0)
                    {
                        if (AttachedObject != null) // This check is only on N-Gage, but we need it to avoid null exception when animation ends
                        {
                            if (IsFacingRight)
                                AttachedObject.Position = Position + new Vector2(6, -22);
                            else
                                AttachedObject.Position = Position + new Vector2(-4, -22);
                        }
                    }
                    else if (AnimatedObject.CurrentFrame is >= 1 and < 7)
                    {
                        // Sync the objects position with Rayman
                        Vector2 objOffset = AnimatedObject.CurrentFrame switch
                        {
                            0 => new Vector2(0, 0),
                            1 => new Vector2(-3, -20),
                            2 => new Vector2(-31, -12),
                            3 => new Vector2(-40, -10),
                            4 => new Vector2(-38, -10),
                            5 => new Vector2(-33, -10),
                            6 => new Vector2(-13, -20),
                            _ => throw new Exception("Invalid frame index")
                        };

                        if (IsFacingRight)
                            AttachedObject.Position = Position + new Vector2(objOffset.X, objOffset.Y - 20);
                        else
                            AttachedObject.Position = Position + new Vector2(-objOffset.X, objOffset.Y - 20);
                    }
                    else if (AttachedObject != null)
                    {
                        PlaySound(Rayman3SoundEvent.Play__GenWoosh_LumSwing_Mix03);
                        AttachedObject.ProcessMessage(this, Message.ThrowObjectForward);
                        AttachedObject = null;
                    }
                }
                else
                {
                    if (AnimatedObject.CurrentFrame < 7)
                    {
                        if (AttachedObject != null) // This check is only on N-Gage, but we need it to avoid null exception when animation ends
                        {
                            // Sync the objects position with Rayman
                            float objYOffset = AnimatedObject.CurrentFrame switch
                            {
                                0 => 0,
                                1 => 3,
                                2 => 4,
                                3 => 9,
                                4 => 2,
                                5 => -11,
                                6 => -40,
                                _ => throw new Exception("Invalid frame index")
                            };

                            if (IsFacingRight)
                                AttachedObject.Position = Position + new Vector2(6, objYOffset - 22);
                            else
                                AttachedObject.Position = Position + new Vector2(-4, objYOffset - 22);
                        }
                    }
                    else if (AttachedObject != null)
                    {
                        AttachedObject.ProcessMessage(this, Message.ThrowObjectUp);
                        AttachedObject = null;
                    }
                }

                OffsetCarryingObject();

                if (IsActionFinished)
                {
                    State.MoveTo(Fsm_Default);
                    return;
                }

                if (IsButtonPressed2(GbaInput.B) &&
                    CanAttackWithFist(2) &&
                    field23_0x98 == 0 &&
                    ActionId is Action.ThrowObjectUp_Right or Action.ThrowObjectUp_Left &&
                    AnimatedObject.CurrentFrame > 6)
                {
                    State.MoveTo(Fsm_ChargeAttack);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_EndMap(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // NOTE: The game doesn't check the class, and will end up writing to other memory if of type FrameWorldSideScroller
                if (Frame.Current is FrameSideScroller sideScroller)
                    sideScroller.CanPause = false;
                Flag2_1 = false;
                NextActionId = null;
                PreviousXSpeed = 0;
                if (HasLanded())
                {
                    if (FinishedMap)
                    {
                        Timer = 0;
                        ActionId = IsFacingRight ? Action.Victory_Right : Action.Victory_Left;
                        PlaySound(Rayman3SoundEvent.Play__OnoWin_Mix02__or__OnoWinRM_Mix02);

                        if (GameInfo.MapId != MapId.BossBadDreams &&
                            GameInfo.MapId != MapId.BossScaleMan &&
                            GameInfo.MapId != MapId.BossFinal_M1 &&
                            GameInfo.MapId != MapId.BossFinal_M2 &&
                            GameInfo.MapId != MapId.BossMachine &&
                            GameInfo.MapId != MapId.BossRockAndLava)
                        {
                            SoundEventsManager.ReplaceAllSongs(Rayman3SoundEvent.Play__win3, 0);
                        }
                        else
                        {
                            LevelMusicManager.OverrideLevelMusic(Rayman3SoundEvent.Play__Win_BOSS);
                        }
                    }
                    else
                    {
                        ActionId = IsFacingRight ? Action.ReturnFromLevel_Right : Action.ReturnFromLevel_Left;
                        Timer = 0;
                    }

                    // NOTE: The game doesn't check the class, and will end up writing to other memory if of type FrameWorldSideScroller
                    if (Frame.Current is FrameSideScroller sideScroller2)
                        sideScroller2.IsTimed = false;
                }
                else
                {
                    ActionId = IsFacingRight ? Action.Fall_Right : Action.Fall_Left;
                }
                break;

            case FsmAction.Step:
                if (SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__win3))
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__canopy);

                // Don't allow horizontal movement while falling
                if (ActionId is Action.Fall_Right or Action.Fall_Left)
                    MechModel.Speed = new Vector2(0, MechModel.Speed.Y);

                Timer++;

                if (HasLanded() && ActionId is Action.Fall_Right or Action.Fall_Left)
                {
                    ActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                    return;
                }

                if (ActionId is Action.Land_Right or Action.Land_Left && IsActionFinished)
                {
                    if (FinishedMap)
                    {
                        Timer = 0;
                        ActionId = IsFacingRight ? Action.Victory_Right : Action.Victory_Left;
                        PlaySound(Rayman3SoundEvent.Play__OnoWin_Mix02__or__OnoWinRM_Mix02);

                        if (GameInfo.MapId != MapId.BossBadDreams &&
                            GameInfo.MapId != MapId.BossScaleMan &&
                            GameInfo.MapId != MapId.BossFinal_M1 &&
                            GameInfo.MapId != MapId.BossFinal_M2 &&
                            GameInfo.MapId != MapId.BossMachine &&
                            GameInfo.MapId != MapId.BossRockAndLava)
                        {
                            SoundEventsManager.ReplaceAllSongs(Rayman3SoundEvent.Play__win3, 0);
                        }
                        else
                        {
                            LevelMusicManager.OverrideLevelMusic(Rayman3SoundEvent.Play__Win_BOSS);
                        }
                    }
                    else
                    {
                        ActionId = IsFacingRight ? Action.ReturnFromLevel_Right : Action.ReturnFromLevel_Left;
                        Timer = 0;
                    }

                    // NOTE: The game doesn't check the class, and will end up writing to other memory if of type FrameWorldSideScroller
                    if (Frame.Current is FrameSideScroller sideScroller2)
                        sideScroller2.IsTimed = false;
                    return;
                }

                if (ActionId is Action.Idle_Right or Action.Idle_Left or Action.ReturnFromLevel_Right or Action.ReturnFromLevel_Left &&
                    ((!FinishedMap && Timer == 150) || (FinishedMap && Timer == 100)))
                {
                    if (SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__Win_BOSS))
                    {
                        Timer -= 2;
                        return;
                    }

                    // TODO: N-Gage already returns here - why?
                    if (Engine.Settings.Platform == Platform.GBA)
                    {
                        if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4)
                            SoundEventsManager.StopAllSongs();

                        if (FinishedMap)
                            SoundEventsManager.StopAllSongs();
                    }

                    return;
                }

                if (ActionId is not (Action.Idle_Right or Action.Idle_Left or Action.ReturnFromLevel_Right or Action.ReturnFromLevel_Left) ||
                    ((FinishedMap || Timer <= 150) && (!FinishedMap || Timer <= 100)))
                {
                    if (!IsActionFinished)
                        return;

                    if (ActionId is (Action.Victory_Right or Action.Victory_Left))
                    {
                        ActionId = IsFacingRight ? Action.Idle_Right : Action.Idle_Left;
                        Timer = 0;
                    }

                    if (Flag2_1)
                        return;

                    Flag2_1 = true;

                    if (Engine.Settings.Platform == Platform.GBA && GameInfo.LevelType == LevelType.GameCube)
                        ((FrameSideScrollerGCN)Frame.Current).FUN_0808a9f4();

                    switch (GameInfo.MapId)
                    {
                        case MapId.CavesOfBadDreams_M1:
                        case MapId.CavesOfBadDreams_M2:
                            // TODO: Implement
                            break;

                        case MapId.SanctuaryOfRockAndLava_M1:
                        case MapId.SanctuaryOfRockAndLava_M2:
                        case MapId.SanctuaryOfRockAndLava_M3:
                            // TODO: Implement
                            break;

                        case MapId.Power6:
                        case MapId.World1:
                        case MapId.World2:
                        case MapId.World3:
                        case MapId.World4:
                            ((World)Frame.Current).InitExiting();
                            break;

                        case MapId.WorldMap:
                            // TODO: Implement
                            break;

                        default:
                            ((FrameSideScroller)Frame.Current).InitNewCircleFXTransition(false);
                            break;
                    }
                    return;
                }

                if (FinishedMap)
                {
                    if (Engine.Settings.Platform == Platform.GBA && GameInfo.LevelType == LevelType.GameCube)
                    {
                        ((FrameSideScrollerGCN)Frame.Current).RestoreMapAndPowers();
                        int gcnMapId = ((FrameSideScrollerGCN)Frame.Current).GcnMapId;

                        if (GameInfo.PersistentInfo.CompletedGCNBonusLevels < gcnMapId + 1)
                            GameInfo.PersistentInfo.CompletedGCNBonusLevels = (byte)(gcnMapId + 1);

                        FrameManager.SetNextFrame(new GameCubeMenu());
                        GameInfo.Save(GameInfo.CurrentSlot);
                    }
                    else if (GameInfo.MapId > (MapId)GameInfo.PersistentInfo.LastCompletedLevel)
                    {
                        switch (GameInfo.MapId)
                        {
                            case MapId.WoodLight_M2:
                                GameInfo.LoadLevel(MapId.Power1);
                                break;

                            case MapId.BossMachine:
                                GameInfo.LoadLevel(MapId.Power2);
                                break;

                            case MapId.EchoingCaves_M2:
                                GameInfo.LoadLevel(MapId.Power3);
                                break;

                            case MapId.SanctuaryOfStoneAndFire_M3:
                                GameInfo.LoadLevel(MapId.Power5);
                                break;

                            case MapId.BossRockAndLava:
                                GameInfo.LoadLevel(MapId.Power4);
                                break;

                            case MapId.BossScaleMan:
                                GameInfo.LoadLevel(MapId.Power6);
                                break;

                            default:
                                Frame.Current.EndOfFrame = true;
                                GameInfo.UpdateLastCompletedLevel();
                                break;
                        }
                    }
                    else
                    {
                        Frame.Current.EndOfFrame = true;
                    }
                }
                else
                {
                    if (Engine.Settings.Platform == Platform.GBA && GameInfo.LevelType == LevelType.GameCube)
                    {
                        ((FrameSideScrollerGCN)Frame.Current).RestoreMapAndPowers();
                        FrameManager.SetNextFrame(new GameCubeMenu());
                    }
                    else if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4)
                    {
                        // TODO: Load worldmap
                    }
                    else
                    {
                        GameInfo.LoadLevel(GameInfo.World + MapId.World1);
                    }
                }

                AutoSave();
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Hit(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                NextActionId = null;
                if (ActionId is not (Action.Damage_Shock_Right or Action.Damage_Shock_Left))
                    ActionId = IsFacingRight ? Action.Damage_Hit_Right : Action.Damage_Hit_Left;
                break;

            case FsmAction.Step:
                if (IsActionFinished)
                    State.MoveTo(Fsm_Default);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_HitKnockback(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                LinkedMovementActor = null;
                Timer = 0;

                if (Flag1_4)
                {
                    CheckAgainstMapCollision = false;
                    CheckAgainstObjectCollision = false;
                    ReceiveDamage(HitPoints);
                    Flag1_4 = false;
                }

                if (!Flag1_C)
                {
                    // TODO: N-Gage checks if AttachedObject is null and if so goes to the "else". However on GBA it'd
                    //       probably go to the first block if null? Check if this is ever an issue.
                    if (Position.X - AttachedObject.Position.X >= 0)
                        ActionId = IsFacingRight ? Action.KnockbackForwards_Right : Action.KnockbackBackwards_Left;
                    else
                        ActionId = IsFacingRight ? Action.KnockbackBackwards_Right : Action.KnockbackForwards_Left;
                }
                else
                {
                    if (Position.X - AttachedObject.Position.X >= 0)
                        ActionId = IsFacingRight ? Action.SmallKnockbackForwards_Right : Action.SmallKnockbackBackwards_Left;
                    else
                        ActionId = IsFacingRight ? Action.SmallKnockbackBackwards_Right : Action.SmallKnockbackForwards_Left;
                }

                NextActionId = null;
                AttachedObject = null;
                PlaySound(Rayman3SoundEvent.Stop__SldGreen_SkiLoop1);
                break;

            case FsmAction.Step:
                if (HitPoints != 0)
                {
                    if (!FsmStep_DoInTheAir())
                        return;
                }

                if (IsButtonPressed(GbaInput.Left))
                {
                    if (IsFacingRight)
                        AnimatedObject.FlipX = true;
                }
                else if (IsButtonPressed(GbaInput.Right))
                {
                    if (IsFacingLeft)
                        AnimatedObject.FlipX = false;
                }

                Timer++;

                // TODO: Seems to be a bug - fix? The flag is true if you started out climbing. This should probably set it to false after 25 frames.
                if (!Flag2_1 && Timer > 25)
                    Flag2_1 = false;

                if (HitPoints == 0 && Timer > 20)
                {
                    State.MoveTo(Fsm_Dying);
                    return;
                }

                if (HitPoints != 0 && Timer > 90)
                {
                    State.MoveTo(Fsm_Fall);
                    return;
                }

                if (HitPoints != 0 && IsButtonJustPressed(GbaInput.A) && field27_0x9c == 0)
                {
                    State.MoveTo(Fsm_Helico);
                    return;
                }

                if (HitPoints != 0 && IsButtonJustPressed(GbaInput.A) && field27_0x9c != 0)
                {
                    State.MoveTo(FUN_0802ddac);
                    return;
                }

                if (HitPoints != 0 && IsNearHangableEdge())
                {
                    State.MoveTo(Fsm_HangOnEdge);
                    return;
                }

                if (HitPoints != 0 && HasLanded())
                {
                    NextActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                    State.MoveTo(Fsm_Default);
                    return;
                }

                if (HitPoints != 0 && Timer > 10 && IsOnHangable())
                {
                    BeginHang();
                    State.MoveTo(Fsm_Hang);
                    return;
                }

                if (HitPoints != 0 && !Flag2_1 && IsOnClimbableVertical() == 1)
                {
                    State.MoveTo(Fsm_Climb);
                    return;
                }

                // TODO: Implement
                //if (HitPoints != 0 && CheckInput2(GbaInput.L) && IsOnWallJumpable())
                //{

                //}
                break;

            case FsmAction.UnInit:
                CheckAgainstMapCollision = true;
                CheckAgainstObjectCollision = true;
                Flag2_1 = false;
                break;
        }
    }

    private void Fsm_Dying(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // NOTE: The game doesn't check the class, and will end up writing to other memory if of type FrameWorldSideScroller
                if (Frame.Current is FrameSideScroller sideScroller)
                    sideScroller.CanPause = false;

                if (GameInfo.MapId is not (MapId.ChallengeLy1 or MapId.ChallengeLy2 or MapId.ChallengeLyGCN) &&
                    GameInfo.LevelType != LevelType.GameCube)
                {
                    GameInfo.ModifyLives(-1);
                }

                PlaySound(Rayman3SoundEvent.Play__RaDeath_Mix03);
                Timer = GameTime.ElapsedFrames;
                ReceiveDamage(5);

                if (ActionId is not (Action.Drown_Right or Action.Drown_Left))
                    ActionId = IsFacingRight ? Action.Dying_Right : Action.Dying_Left;

                NextActionId = null;

                if (GameInfo.LevelType == LevelType.GameCube)
                    ((FrameSideScrollerGCN)Frame.Current).FUN_0808a9f4();

                if (GameInfo.MapId is MapId.SanctuaryOfRockAndLava_M1 or MapId.SanctuaryOfRockAndLava_M2 or MapId.SanctuaryOfRockAndLava_M3)
                {
                    // TODO: Implement
                }
                else
                {
                    ((FrameSideScroller)Frame.Current).InitNewCircleFXTransition(false);
                }

                if (AttachedObject != null)
                {
                    AttachedObject.ProcessMessage(this, Message.DropObject);
                    AttachedObject = null;
                }
                break;

            case FsmAction.Step:
                if (IsActionFinished && GameTime.ElapsedFrames - Timer > 120)
                {
                    if (GameInfo.PersistentInfo.Lives == 0)
                    {
                        // Game over
                        FrameManager.SetNextFrame(new GameOver());
                    }
                    else
                    {
                        // Reload current map
                        FrameManager.ReloadCurrentFrame();
                    }
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Cutscene(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                NextActionId = null;
                PreviousXSpeed = 0;

                if (IsOnClimbableVertical() != 0)
                {
                    ActionId = IsFacingRight ? Action.Climb_Idle_Right : Action.Climb_Idle_Left;
                    MechModel.Speed = Vector2.Zero;
                }
                else if (Scene.GetPhysicalType(Position).IsSolid || Scene.MainActor.LinkedMovementActor != null || Speed.Y == 0)
                {
                    ActionId = IsFacingRight ? Action.Idle_BeginCutscene_Right : Action.Idle_BeginCutscene_Left;
                }
                else
                {
                    ActionId = IsFacingRight ? Action.Fall_Right : Action.Fall_Left;
                }
                break;

            case FsmAction.Step:
                if (ActionId is Action.Fall_Right or Action.Fall_Left)
                    MechModel.Speed = new Vector2(0, MechModel.Speed.Y);

                if (IsOnClimbableVertical() == 1)
                {
                    if (ActionId is not (Action.Climb_Idle_Right or Action.Climb_Idle_Left))
                        ActionId = IsFacingRight ? Action.Climb_Idle_Right : Action.Climb_Idle_Left;
                }
                else if (Scene.GetPhysicalType(Position).IsSolid && ActionId is Action.Fall_Right or Action.Fall_Left)
                {
                    ActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                    CameraTargetY = 120;
                    Scene.Camera.ProcessMessage(this, Message.Cam_FollowPositionY, CameraTargetY);
                }
                else if (ActionId is Action.Land_Right or Action.Land_Left && IsActionFinished)
                {
                    ActionId = IsFacingRight ? Action.Idle_BeginCutscene_Right : Action.Idle_BeginCutscene_Left;
                }
                else if (ActionId is Action.Idle_BeginCutscene_Right or Action.Idle_BeginCutscene_Left && IsActionFinished)
                {
                    ActionId = IsFacingRight ? Action.Idle_Cutscene_Right : Action.Idle_Cutscene_Left;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Stop(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                NextActionId = null;
                PreviousXSpeed = 0;

                if (IsOnClimbableVertical() != 0)
                {
                    ActionId = IsFacingRight ? Action.Climb_Idle_Right : Action.Climb_Idle_Left;
                    MechModel.Speed = Vector2.Zero;
                }
                else if (Scene.GetPhysicalType(Position).IsSolid || Scene.MainActor.LinkedMovementActor != null || Speed.Y == 0)
                {
                    ActionId = IsFacingRight ? Action.Idle_Right : Action.Idle_Left;
                }
                else
                {
                    ActionId = IsFacingRight ? Action.Fall_Right : Action.Fall_Left;
                }
                break;

            case FsmAction.Step:
                if (ActionId is Action.Fall_Right or Action.Fall_Left)
                    MechModel.Speed = new Vector2(0, MechModel.Speed.Y);

                if (IsOnClimbableVertical() == 1)
                {
                    if (ActionId is not (Action.Climb_Idle_Right or Action.Climb_Idle_Left))
                        ActionId = IsFacingRight ? Action.Climb_Idle_Right : Action.Climb_Idle_Left;
                }
                else if (Scene.GetPhysicalType(Position).IsSolid && ActionId is Action.Fall_Right or Action.Fall_Left)
                {
                    ActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                    CameraTargetY = 120;
                    Scene.Camera.ProcessMessage(this, Message.Cam_FollowPositionY, CameraTargetY);
                }
                else if (ActionId is Action.Land_Right or Action.Land_Left && IsActionFinished)
                {
                    ActionId = IsFacingRight ? Action.Idle_Right : Action.Idle_Left;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_EnterLevelCurtain(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                NextActionId = null;
                ActionId = Action.EnterCurtain_Right;

                CameraSideScroller cam = (CameraSideScroller)Scene.Camera;
                cam.HorizontalOffset = CameraOffset.Center;
                break;

            case FsmAction.Step:
                // Do nothing
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_LockedLevelCurtain(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsFacingRight ? Action.LockedLevelCurtain_Right : Action.LockedLevelCurtain_Left;
                NextActionId = null;

                if (!SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01))
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                break;

            case FsmAction.Step:
                if (IsActionFinished)
                    State.MoveTo(Fsm_Default);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    // TODO: Implement all of these
    private void FUN_0802ce54(FsmAction action) { }
    private void FUN_080284ac(FsmAction action) { }
    private void FUN_0802ddac(FsmAction action) { }
    private void FUN_08033228(FsmAction action) { }
    private void FUN_08031554(FsmAction action) { }
    private void FUN_080224f4(FsmAction action) { }
    private void FUN_1005dea0(FsmAction action) { }
    private void FUN_1005dfa4(FsmAction action) { }
    private void FUN_1005e04c(FsmAction action) { }
}