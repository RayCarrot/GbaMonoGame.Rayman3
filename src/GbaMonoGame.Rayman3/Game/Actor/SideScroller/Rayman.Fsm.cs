using System;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class Rayman
{
    private bool FsmStep_Inlined_FUN_1004c544()
    {
        if (!FsmStep_Inlined_FUN_1004c1f4())
            return false;

        CheckSlide();
        ManageSlide();

        if (ManageHit())
        {
            Fsm.ChangeAction(Fsm_Hit);
            return false;
        }

        if (CheckSingleInput(GbaInput.A) && SlideType != null)
        {
            Fsm.ChangeAction(Fsm_JumpSlide);
            return false;
        }

        if (ShouldAutoJump())
        {
            PlaySound(Rayman3SoundEvent.Stop__SkiLoop1);

            Fsm.ChangeAction(Fsm_JumpSlide);
            return false;
        }

        return true;
    }

    // FUN_08020bd4
    private bool FsmStep_Inlined_FUN_1004c1f4()
    {
        CameraSideScroller cam = (CameraSideScroller)Scene.Camera;

        if (Flag1_D)
        {
            field16_0x91++;

            if (field16_0x91 > 60)
            {
                cam.HorizontalOffset = Engine.Settings.Platform switch
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
            !IsInFrontOfLevelCurtain)
        {
            Message message;

            if (!Fsm.EqualsAction(FUN_0802ddac) &&
                CheckInput(GbaInput.Down) &&
                (Speed.Y > 0 || Fsm.EqualsAction(Fsm_Crouch)) &&
                !Fsm.EqualsAction(Fsm_Climb))
            {
                field18_0x93 = 70;
                message = Message.Cam_1040;
            }
            else if (CheckInput(GbaInput.Up) && (Fsm.EqualsAction(Fsm_Default) || Fsm.EqualsAction(Fsm_HangOnEdge)))
            {
                field18_0x93 = 160;
                message = Message.Cam_1040;
            }
            else if (Fsm.EqualsAction(Fsm_Helico) && field27_0x9c == 0)
            {
                message = Message.Cam_1039;
            }
            else if (Fsm.EqualsAction(Fsm_Swing))
            {
                field18_0x93 = 65;
                message = Message.Cam_1040;
            }
            else if (Fsm.EqualsAction(Fsm_Climb) || Fsm.EqualsAction(FUN_0802ddac))
            {
                field18_0x93 = 112;
                message = Message.Cam_1040;
            }
            else
            {
                field18_0x93 = 120;
                message = Message.Cam_1040;
            }

            cam.ProcessMessage(message, field18_0x93);
        }

        if (field23_0x98 != 0)
            field23_0x98--;

        if (IsDead())
        {
            if (!RSMultiplayer.IsActive)
                Fsm.ChangeAction(Fsm_Dying);
            else
                Fsm.ChangeAction(FUN_08033228);

            return false;
        }

        return true;
    }

    private bool FsmStep_DoInTheAir()
    {
        if (ManageHit() &&
            (Fsm.EqualsAction(Fsm_StopHelico) ||
             Fsm.EqualsAction(Fsm_Helico) ||
             Fsm.EqualsAction(Fsm_Jump) ||
             Fsm.EqualsAction(Fsm_JumpSlide)) &&
            ActionId is not (
                Action.Damage_Knockback_Right or
                Action.Damage_Knockback_Left or
                Action.Damage_Shock_Right or
                Action.Damage_Shock_Left))
        {
            ActionId = IsFacingRight ? Action.Damage_Knockback_Right : Action.Damage_Knockback_Left;
        }

        return FsmStep_Inlined_FUN_1004c1f4();
    }

    // TODO: Name
    private bool FsmStep_FUN_08020e8c()
    {
        if (Engine.Settings.Platform == Platform.GBA)
        {
            CameraSideScroller cam = (CameraSideScroller)Scene.Camera;
            cam.HorizontalOffset = 120;
        }

        if (field23_0x98 != 0)
            field23_0x98--;

        ManageHit();

        if (HitPoints == 0)
        {
            Fsm.ChangeAction(Fsm_Dying);
            return false;
        }

        return true;
    }

    // TODO: Name
    private bool FsmStep_FUN_08020ee0()
    {
        if (!FsmStep_Inlined_FUN_1004c1f4())
            return false;

        CheckSlide();
        ManageSlide();

        if (ManageHit())
        {
            Fsm.ChangeAction(Fsm_Hit);
            return false;
        }

        if (CheckSingleInput(GbaInput.A) && SlideType != null)
        {
            Fsm.ChangeAction(Fsm_JumpSlide);
            return false;
        }

        if (ShouldAutoJump())
        {
            PlaySound(Rayman3SoundEvent.Stop__SldGreen_SkiLoop1);
            Fsm.ChangeAction(Fsm_JumpSlide);
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
                    cam.HorizontalOffset = Engine.Settings.Platform switch
                    {
                        Platform.GBA => 120,
                        Platform.NGage => 88,
                        _ => throw new UnsupportedPlatformException()
                    };
                }
                else
                {
                    if (!RSMultiplayer.IsActive)
                        cam.HorizontalOffset = Engine.Settings.Platform switch
                        {
                            Platform.GBA => 40,
                            Platform.NGage => 25,
                            _ => throw new UnsupportedPlatformException()
                        };
                    else if (IsLocalPlayer)
                        cam.HorizontalOffset = 95;
                }

                if (IsLocalPlayer)
                    cam.ProcessMessage(Message.Cam_1027);

                if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4)
                    cam.HorizontalOffset = Engine.Settings.Platform switch
                    {
                        Platform.GBA => 120,
                        Platform.NGage => 88,
                        _ => throw new UnsupportedPlatformException()
                    };
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
                    Fsm.ChangeAction(Fsm_Default);
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
                if (CheckInput(GbaInput.Up))
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

                if (CheckInput(GbaInput.Left) && IsFacingRight)
                {
                    ActionId = Action.Walk_Left;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }
                else if (CheckInput(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Walk_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }

                // Jump
                if (CheckSingleInput(GbaInput.A) && Flag2_2)
                {
                    PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    Fsm.ChangeAction(Fsm_Jump);
                    return;
                }
                
                // Crouch
                if (CheckInput(GbaInput.Down))
                {
                    NextActionId = IsFacingRight ? Action.CrouchDown_Right : Action.CrouchDown_Left;
                    PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    Fsm.ChangeAction(Fsm_Crouch);
                    return;
                }
                
                // Fall
                if (Speed.Y > 1)
                {
                    PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }
                
                // Walk
                if (CheckInput(GbaInput.Left) || CheckInput(GbaInput.Right))
                {
                    PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    Fsm.ChangeAction(Fsm_Walk);
                    return;
                }
                
                // Punch
                if (field23_0x98 == 0 && CheckSingleInput(GbaInput.B) && CanAttackWithFist(2))
                {
                    PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    Fsm.ChangeAction(Fsm_ChargeAttack);
                    return;
                }

                // Walking off edge
                if (PreviousXSpeed != 0 && IsNearEdge() != 0 && !Flag1_1)
                {
                    PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    Position += new Vector2(PreviousXSpeed < 0 ? -16 : 16, 0);
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }

                // Standing near edge
                if (PreviousXSpeed == 0 && IsNearEdge() != 0 && !Flag1_1)
                {
                    PlaySound(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    Fsm.ChangeAction(Fsm_StandingNearEdge);
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
                    Fsm.ChangeAction(Fsm_Default);
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

                    switch (Engine.Settings.Platform)
                    {
                        case Platform.GBA:
                            if (cam.HorizontalOffset == 120)
                                cam.HorizontalOffset = 40;
                            break;

                        case Platform.NGage:
                            if (cam.HorizontalOffset == 88)
                                cam.HorizontalOffset = 25;
                            break;

                        default:
                            throw new UnsupportedPlatformException();
                    }
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
                if (CheckInput(GbaInput.Left) && IsFacingRight)
                {
                    ActionId = Action.Walk_Left;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }
                else if (CheckInput(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Walk_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }

                // Walk
                if (CheckInput(GbaInput.Left) || CheckInput(GbaInput.Right))
                {
                    Fsm.ChangeAction(Fsm_Walk);
                    return;
                }
                
                // Crouch
                if (CheckInput(GbaInput.Down))
                {
                    Fsm.ChangeAction(Fsm_Crouch);
                    return;
                }

                // Jump
                if (CheckSingleInput(GbaInput.A))
                {
                    Fsm.ChangeAction(Fsm_Jump);
                    return;
                }

                // Fall
                if (Speed.Y > 1)
                {
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }

                // Punch
                if (CheckInput2(GbaInput.B) && CanAttackWithFist(2))
                {
                    Fsm.ChangeAction(Fsm_ChargeAttack);
                    return;
                }

                // Default if no longer near edge
                if (IsNearEdge() == 0)
                {
                    Fsm.ChangeAction(Fsm_Default);
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
                    if (CheckInput(GbaInput.Left) && IsFacingRight)
                    {
                        ActionId = Action.Walk_LookAround_Left;
                        ChangeAction();

                        if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                            throw new NotImplementedException();
                    }
                    else if (CheckInput(GbaInput.Right) && IsFacingLeft)
                    {
                        ActionId = Action.Walk_LookAround_Right;
                        ChangeAction();

                        if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    if (CheckInput(GbaInput.Left) && IsFacingRight)
                    {
                        ActionId = Action.Walk_Left;
                        ChangeAction();

                        if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                            throw new NotImplementedException();
                    }
                    else if (CheckInput(GbaInput.Right) && IsFacingLeft)
                    {
                        ActionId = Action.Walk_Right;
                        ChangeAction();

                        if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                            throw new NotImplementedException();
                    }
                }

                if (CheckInput2(GbaInput.B))
                {
                    Charge++;
                }
                else if (CheckSingleReleasedInput(GbaInput.B) && field23_0x98 == 0)
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
                        !AnimatedObject.HasExecutedFrame)
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
                if (CheckReleasedInput(GbaInput.Left) && CheckReleasedInput(GbaInput.Right) &&
                    ActionId is
                        Action.Walk_Right or Action.Walk_Left or
                        Action.Walk_Multiplayer_Right or Action.Walk_Multiplayer_Left)
                {
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                // Return and shout for Globox if looking for him when released the left and right inputs
                if (CheckReleasedInput(GbaInput.Left) && CheckReleasedInput(GbaInput.Right) &&
                    ActionId is Action.Walk_LookAround_Right or Action.Walk_LookAround_Left)
                {
                    NextActionId = IsFacingRight ? Action.Idle_Shout_Right : Action.Idle_Shout_Left;
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                // Crawl
                if (CheckInput(GbaInput.Down))
                {
                    Fsm.ChangeAction(Fsm_Crawl);
                    return;
                }

                // Jump
                if (CheckSingleInput(GbaInput.A))
                {
                    Fsm.ChangeAction(Fsm_Jump);
                    return;
                }

                // Fall
                if (PreviousXSpeed != 0 && Speed.Y > 1)
                {
                    Position += new Vector2(IsFacingLeft ? -16 : 16, 0);
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }

                // Fall
                if (Speed.Y > 1 && Timer >= 8)
                {
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }

                // Charge punch
                if (field23_0x98 == 0 && Charge > 10 && CheckInput(GbaInput.B) && CanAttackWithFist(2))
                {
                    Fsm.ChangeAction(Fsm_ChargeAttack);
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
                field18_0x93 = 70;

                if (IsLocalPlayer)
                    cam.ProcessMessage(Message.Cam_1039, field18_0x93);

                Timer = GameTime.ElapsedFrames;
                SlideType = null;
                LinkedMovementActor = null;
                break;

            case FsmAction.Step:
                if (!FsmStep_DoInTheAir())
                    return;

                if (IsLocalPlayer)
                    cam.ProcessMessage(Message.Cam_1039, (byte)130);

                if (ActionId is Action.Jump_Right or Action.Jump_Left &&
                    CheckReleasedInput2(GbaInput.A) && 
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
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                if (GameTime.ElapsedFrames - Timer > 50)
                {
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }

                if (IsNearHangableEdge())
                {
                    Fsm.ChangeAction(Fsm_HangOnEdge);
                    return;
                }

                if (CheckSingleInput(GbaInput.A) && field27_0x9c == 0)
                {
                    Fsm.ChangeAction(Fsm_Helico);
                    return;
                }

                // TODO: Implement

                if (IsOnHangable())
                {
                    BeginHang();
                    Fsm.ChangeAction(Fsm_Hang);
                    return;
                }

                if (GameTime.ElapsedFrames - Timer > 10 && IsOnClimbableVertical() == 1)
                {
                    Fsm.ChangeAction(Fsm_Climb);
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
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                // Fall
                if (speedY > 3.4375)
                {
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }

                // Hang on edge
                if (IsNearHangableEdge())
                {
                    PreviousXSpeed = 0;
                    Fsm.ChangeAction(Fsm_HangOnEdge);
                    return;
                }

                // Helico
                if (CheckSingleInput(GbaInput.A) && field27_0x9c == 0 && GameTime.ElapsedFrames - Timer >= 6)
                {
                    Fsm.ChangeAction(Fsm_Helico);
                    return;
                }

                if (CheckSingleInput(GbaInput.A) && field27_0x9c != 0 && GameTime.ElapsedFrames - Timer >= 6)
                {
                    Fsm.ChangeAction(FUN_0802ddac);
                    return;
                }

                // Hang
                if (GameTime.ElapsedFrames - Timer >= 11 && IsOnHangable())
                {
                    PreviousXSpeed = 0;
                    BeginHang();
                    Fsm.ChangeAction(Fsm_Hang);
                }

                if (IsOnClimbableVertical() == 1)
                {
                    PreviousXSpeed = 0;
                    Fsm.ChangeAction(Fsm_Climb);
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
                if (CheckInput(GbaInput.Down))
                {
                    HangOnEdgeDelay = 30;
                    PlaySound(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }

                // Jump
                if (CheckSingleInput(GbaInput.A))
                {
                    HangOnEdgeDelay = 30;
                    Fsm.ChangeAction(Fsm_Jump);
                    return;
                }

                // Attack
                if (CheckInput2(GbaInput.B) && CanAttackWithFoot())
                {
                    Fsm.ChangeAction(Fsm_ChargeAttack);
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

                if (CheckSingleInput(GbaInput.A) && CanSafetyJump)
                {
                    Fsm.ChangeAction(Fsm_Jump);
                    return;
                }
                
                if (CheckSingleInput(GbaInput.A) && field27_0x9c == 0)
                {
                    Fsm.ChangeAction(Fsm_Helico);
                    return;
                }

                if (CheckSingleInput(GbaInput.A) && field27_0x9c != 0)
                {
                    Fsm.ChangeAction(FUN_0802ddac);
                    return;
                }

                if (IsNearHangableEdge())
                {
                    Fsm.ChangeAction(Fsm_HangOnEdge);
                    return;
                }

                if (HasLanded())
                {
                    NextActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                if (IsOnHangable())
                {
                    BeginHang();
                    Fsm.ChangeAction(Fsm_Hang);
                    return;
                }

                if (IsOnClimbableVertical() == 1)
                {
                    Fsm.ChangeAction(Fsm_Climb);
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
                    Fsm.ChangeAction(Fsm_HangOnEdge);
                    return;
                }

                if (HasLanded())
                {
                    NextActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                if (CheckSingleInput(GbaInput.A) || CheckSingleInput(GbaInput.B))
                {
                    Fsm.ChangeAction(Fsm_StopHelico);
                    return;
                }

                if (GameTime.ElapsedFrames - Timer > 40)
                {
                    Fsm.ChangeAction(Fsm_TimeoutHelico);
                    return;
                }

                if (IsOnHangable())
                {
                    BeginHang();
                    Fsm.ChangeAction(Fsm_Hang);
                    return;
                }

                if (IsOnClimbableVertical() == 1)
                {
                    Fsm.ChangeAction(Fsm_Climb);
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
                    cam.ProcessMessage(Message.Cam_1027);

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
                    Fsm.ChangeAction(Fsm_HangOnEdge);
                    return;
                }

                if (HasLanded())
                {
                    NextActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                if (CheckSingleInput(GbaInput.A) && field27_0x9c != 0)
                {
                    Fsm.ChangeAction(FUN_0802ddac);
                    return;
                }

                if (IsOnClimbableVertical() == 1)
                {
                    Fsm.ChangeAction(Fsm_Climb);
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
                    Fsm.ChangeAction(Fsm_HangOnEdge);
                    return;
                }

                if (HasLanded())
                {
                    NextActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                    PlaySound(Rayman3SoundEvent.Play__HeliCut_Mix01);
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                if (CheckSingleInput(GbaInput.A) || CheckSingleInput(GbaInput.B) || Timer > 50)
                {
                    Fsm.ChangeAction(Fsm_StopHelico);
                    return;
                }

                if (IsOnHangable())
                {
                    BeginHang();
                    Fsm.ChangeAction(Fsm_Hang);
                    return;
                }

                if (IsOnClimbableVertical() == 1)
                {
                    Fsm.ChangeAction(Fsm_Climb);
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
                    Fsm.ChangeAction(FUN_0802ddac);
                    return;
                }
                break;

            case FsmAction.UnInit:
                PlaySound(Rayman3SoundEvent.Stop__HeliStop_Mix06);
                PreviousXSpeed = 0;

                if (IsLocalPlayer)
                    cam.ProcessMessage(Message.Cam_1027);

                if (Timer > 50)
                {
                    Vector2 pos = Position;

                    pos += new Vector2(0, Constants.TileSize);
                    if (Scene.GetPhysicalType(pos) != PhysicalTypeValue.None)
                        return;

                    pos += new Vector2(0, Constants.TileSize);
                    if (Scene.GetPhysicalType(pos) != PhysicalTypeValue.None)
                        return;

                    pos += new Vector2(0, Constants.TileSize);
                    if (Scene.GetPhysicalType(pos) != PhysicalTypeValue.None)
                        return;

                    pos += new Vector2(0, Constants.TileSize);
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

                PhysicalType topType = Scene.GetPhysicalType(new Vector2(detectionBox.MinX + 1, detectionBox.MinY - 8));

                if (!topType.IsSolid)
                    topType = Scene.GetPhysicalType(new Vector2(detectionBox.MaxX - 1, detectionBox.MinY - 8));

                // Change direction
                if (CheckInput(GbaInput.Left) && IsFacingRight)
                {
                    ActionId = Action.Crawl_Left;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }
                else if (CheckInput(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Crawl_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }

                // Let go of down and stop crouching
                if (CheckReleasedInput(GbaInput.Down) && !topType.IsSolid)
                {
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                // Crawl
                if (CheckInput(GbaInput.Left) || CheckInput(GbaInput.Right))
                {
                    Fsm.ChangeAction(Fsm_Crawl);
                    return;
                }

                // Fall
                if (Speed.Y > 1)
                {
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }

                // Jump
                if (CheckSingleInput(GbaInput.A) && !topType.IsSolid && Flag2_2)
                {
                    Fsm.ChangeAction(Fsm_Jump);
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

                PhysicalType topType = Scene.GetPhysicalType(new Vector2(detectionBox.MinX + 1, detectionBox.MinY - 8));

                if (!topType.IsSolid)
                    topType = Scene.GetPhysicalType(new Vector2(detectionBox.MaxX - 1, detectionBox.MinY - 8));

                // Change direction
                if (CheckInput(GbaInput.Left) && IsFacingRight)
                {
                    ActionId = Action.Crawl_Left;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        throw new NotImplementedException();
                }
                else if (CheckInput(GbaInput.Right) && IsFacingLeft)
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
                if (CheckReleasedInput(GbaInput.Down) && (CheckInput(GbaInput.Left) || CheckInput(GbaInput.Right)) && !topType.IsSolid)
                {
                    Fsm.ChangeAction(Fsm_Walk);
                    return;
                }

                // Stopped crouching/crawling
                if (CheckReleasedInput(GbaInput.Down) && CheckReleasedInput(GbaInput.Left) && CheckReleasedInput(GbaInput.Right) && !topType.IsSolid)
                {
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                // Crouch
                if (CheckReleasedInput(GbaInput.Right) && CheckReleasedInput(GbaInput.Left))
                {
                    Fsm.ChangeAction(Fsm_Crouch);
                    return;
                }

                // Jump
                if (CheckSingleInput(GbaInput.A) && !topType.IsSolid)
                {
                    Fsm.ChangeAction(Fsm_Jump);
                    return;
                }

                // Fall
                if (Speed.Y > 1)
                {
                    Fsm.ChangeAction(Fsm_Fall);
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
                        {
                            if (Speed.X < 0)
                                cam.HorizontalOffset = 151;
                            else
                                cam.HorizontalOffset = 25;
                        }
                        else
                        {
                            if (Speed.X < 0)
                                cam.HorizontalOffset = 25;
                            else
                                cam.HorizontalOffset = 151;
                        }
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
                    if (CheckInput(GbaInput.Left))
                    {
                        if (IsFacingRight)
                            AnimatedObject.FlipX = true;
                    }
                    else if (CheckInput(GbaInput.Right))
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
                        cam.HorizontalOffset = 45;
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
                if (CheckReleasedInput2(GbaInput.B))
                {
                    if (Timer == 0)
                        Timer = GameTime.ElapsedFrames;

                    uint chargePower = GameTime.ElapsedFrames - Timer;

                    // Move plum
                    if (AttachedObject?.Type == (int)ActorType.Plum)
                    {
                        // TODO: Implement and handle message to move plum
                        AttachedObject.ProcessMessage(IsFacingRight ? (Message)0x40c : (Message)0x40d, chargePower);
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
                    Fsm.ChangeAction(Fsm_StopHelico);
                    return;
                }

                if (type == 2)
                {
                    Fsm.ChangeAction(Fsm_Hang);
                    return;
                }

                if (type == 4)
                {
                    Fsm.ChangeAction(Fsm_Climb);
                    return;
                }

                if (type == 3)
                {
                    Fsm.ChangeAction(Fsm_HangOnEdge);
                    return;
                }

                if (type == 1 && AttachedObject?.Type == (int)ActorType.Plum)
                {
                    Fsm.ChangeAction(FUN_080224f4);
                    return;
                }

                if (type == 1)
                {
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                // TODO: Why is this not on GBA?
                if (Engine.Settings.Platform == Platform.NGage &&
                    ActionId is Action.Damage_Shock_Right or Action.Damage_Shock_Left)
                {
                    ActionId = IsFacingRight ? Action.Damage_Hit_Right : Action.Damage_Hit_Left;
                    Fsm.ChangeAction(FUN_1005bf7c);
                    return;
                }

                if (Speed.Y > 1 && AttachedObject?.Type != (int)ActorType.Plum)
                {
                    Fsm.ChangeAction(Fsm_Fall);
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
                        cam.HorizontalOffset = 95;
                    }
                    else
                    {
                        cam.HorizontalOffset = 25;
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
                bool jump = CheckSingleInput(GbaInput.A);

                int climbHoriontal = IsOnClimbableHorizontal();
                int climbVertical = IsOnClimbableVertical();

                PhysicalType type = PhysicalTypeValue.None;

                MechModel.Speed = new Vector2(0, MechModel.Speed.Y);

                if (CheckInput(GbaInput.Left))
                {
                    if (CheckSingleInput2(GbaInput.Left))
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
                        cam.HorizontalOffset = Engine.Settings.Platform switch
                        {
                            Platform.GBA => 40,
                            Platform.NGage => 25,
                            _ => throw new UnsupportedPlatformException()
                        };
                        Timer = 0;
                    }
                }
                else if (CheckInput(GbaInput.Right))
                {
                    if (CheckSingleInput2(GbaInput.Right))
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
                        cam.HorizontalOffset = Engine.Settings.Platform switch
                        {
                            Platform.GBA => 40,
                            Platform.NGage => 25,
                            _ => throw new UnsupportedPlatformException()
                        };
                        Timer = 0;
                    }
                }
                else if (Timer > 50 && !RSMultiplayer.IsActive)
                {
                    // Center camera, only on GBA
                    if (Engine.Settings.Platform == Platform.GBA)
                        cam.HorizontalOffset = 120;

                    Timer = 0;
                }

                if (CheckInput(GbaInput.Up) && climbVertical is 1 or 2)
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
                else if (CheckInput(GbaInput.Down) && climbVertical is 1 or 3)
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

                    if (CheckInput(GbaInput.Left) && ActionId != Action.Climb_Side_Left && Speed.X != 0)
                    {
                        ActionId = Action.Climb_Side_Left;
                        AnimatedObject.CurrentFrame = animFrame;
                    }
                    else if (CheckInput(GbaInput.Right) && ActionId != Action.Climb_Side_Right && Speed.X != 0)
                    {
                        ActionId = Action.Climb_Side_Right;
                        AnimatedObject.CurrentFrame = animFrame;
                    }

                    if (CheckInput(GbaInput.Down) && climbVertical is not (1 or 3))
                    {
                        type = Scene.GetPhysicalType(Position + new Vector2(0, 32));
                    }
                }

                if (Speed == Vector2.Zero)
                {
                    if (CheckInput(GbaInput.Left) && IsFacingRight)
                        AnimatedObject.FlipX = true;
                    else if (CheckInput(GbaInput.Right) && IsFacingLeft)
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
                if (CheckInput2(GbaInput.B) && CanAttackWithFist(1))
                {
                    ActionId = IsFacingRight ? Action.Climb_BeginChargeFist_Right : Action.Climb_BeginChargeFist_Left;
                    Fsm.ChangeAction(Fsm_ChargeAttack);
                    return;
                }

                // Jump left
                if (jump && CheckInput(GbaInput.Left) && climbHoriontal is not (4 or 6))
                {
                    Fsm.ChangeAction(Fsm_Jump);
                    return;
                }

                // Jump right
                if (jump && CheckInput(GbaInput.Right) && climbHoriontal is not (4 or 5))
                {
                    Fsm.ChangeAction(Fsm_Jump);
                    return;
                }

                // Jump up
                if (jump && CheckInput(GbaInput.Up) && climbVertical is not (1 or 2))
                {
                    Fsm.ChangeAction(Fsm_Jump);
                    return;
                }

                // Move down
                if (type.IsSolid && CheckInput(GbaInput.Down) && climbVertical is not (1 or 3))
                {
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }

                // Jump down
                if (jump && CheckInput(GbaInput.Down) && climbVertical is not (1 or 3))
                {
                    PlaySound(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }
                break;

            case FsmAction.UnInit:
                if (!RSMultiplayer.IsActive)
                    cam.HorizontalOffset = Engine.Settings.Platform switch
                    {
                        Platform.GBA => 40,
                        Platform.NGage => 25,
                        _ => throw new UnsupportedPlatformException()
                    };

                if (!CheckSingleInput(GbaInput.B) && IsLocalPlayer)
                    cam.ProcessMessage(Message.Cam_1027);

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
                if (CheckInput(GbaInput.Left) && IsFacingRight)
                {
                    ActionId = Action.Hang_Move_Left;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                    {
                        throw new NotImplementedException();
                    }
                }
                else if (CheckInput(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Hang_Move_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                    {
                        throw new NotImplementedException();
                    }
                }

                // Move
                if (CheckInput(GbaInput.Left) || CheckInput(GbaInput.Right))
                {
                    Fsm.ChangeAction(Fsm_HangMove);
                    return;
                }

                // Move down
                if (CheckInput(GbaInput.Down))
                {
                    Position += new Vector2(0, Constants.TileSize);
                    IsHanging = false;
                    PlaySound(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }

                // No longer hanging
                if (!IsOnHangable())
                {
                    IsHanging = false;
                    PlaySound(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                // Attack
                if (CheckSingleInput(GbaInput.B) && CanAttackWithFoot())
                {
                    Fsm.ChangeAction(Fsm_ChargeAttack);
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
                if (CheckInput(GbaInput.Left) && IsFacingRight)
                {
                    ActionId = Action.Hang_Move_Left;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                    {
                        throw new NotImplementedException();
                    }
                }
                else if (CheckInput(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Hang_Move_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                    {
                        throw new NotImplementedException();
                    }
                }

                // Stop moving
                if (!CheckInput(GbaInput.Left) && !CheckInput(GbaInput.Right))
                {
                    NextActionId = IsFacingRight ? Action.Hang_EndMove_Right : Action.Hang_EndMove_Left;
                    Fsm.ChangeAction(Fsm_Hang);
                    return;
                }

                // Move down
                if (CheckInput(GbaInput.Down))
                {
                    Position += new Vector2(0, Constants.TileSize);
                    IsHanging = false;
                    PlaySound(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }

                // No longer hanging
                if (!IsOnHangable())
                {
                    IsHanging = false;
                    PlaySound(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                // Attack
                if (CheckSingleInput(GbaInput.B) && CanAttackWithFoot())
                {
                    Fsm.ChangeAction(Fsm_ChargeAttack);
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

                Timer = (uint)(MathF.Atan2(Position.X - AttachedObject.Position.X, Position.Y - AttachedObject.Position.Y) / 2 * MathF.PI * 256);
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

                if (AnimatedObject.CurrentFrame < 19)
                {
                    cam.HorizontalOffset = Engine.Settings.Platform switch
                    {
                        Platform.GBA => 40,
                        Platform.NGage => 25,
                        _ => throw new UnsupportedPlatformException()
                    };
                }
                else
                {
                    cam.HorizontalOffset = Engine.Settings.Platform switch
                    {
                        Platform.GBA => 200,
                        Platform.NGage => 151,
                        _ => throw new UnsupportedPlatformException()
                    };
                }

                CreateSwingProjectiles();
                PlaySound(Rayman3SoundEvent.Play__LumMauve_Mix02);
                break;

            case FsmAction.Step:
                if (!FsmStep_Inlined_FUN_1004c1f4())
                    return;

                if (AnimatedObject.CurrentFrame == 19 && Timer == 0)
                {
                    cam.HorizontalOffset = Engine.Settings.Platform switch
                    {
                        Platform.GBA => 200,
                        Platform.NGage => 151,
                        _ => throw new UnsupportedPlatformException()
                    };
                }
                else if (AnimatedObject.CurrentFrame == 39 && Timer == 128)
                {
                    cam.HorizontalOffset = Engine.Settings.Platform switch
                    {
                        Platform.GBA => 40,
                        Platform.NGage => 25,
                        _ => throw new UnsupportedPlatformException()
                    };
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

                if (CheckSingleInput(GbaInput.A) && !Scene.GetPhysicalType(Position).IsSolid)
                    Fsm.ChangeAction(Fsm_Jump);
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

                if (GameInfo.MapId == MapId.TheCanopy_M2)
                {
                    cam.HorizontalOffset = Engine.Settings.Platform switch
                    {
                        Platform.GBA => 120,
                        Platform.NGage => 88,
                        _ => throw new UnsupportedPlatformException()
                    };
                }
                else
                {
                    cam.HorizontalOffset = Engine.Settings.Platform switch
                    {
                        Platform.GBA => 40,
                        Platform.NGage => 25,
                        _ => throw new UnsupportedPlatformException()
                    };
                }
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

                    Fsm.ChangeAction(Fsm_Jump);
                }
                break;

            case FsmAction.UnInit:
                if (Engine.Settings.Platform != Platform.NGage)
                    ActionId = IsFacingRight ? Action.BouncyJump_Right : Action.BouncyJump_Left;
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
                            ((World)Frame.Current).InitTransitionOut();
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
                    Fsm.ChangeAction(Fsm_Default);
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

                if (CheckInput(GbaInput.Left))
                {
                    if (IsFacingRight)
                        AnimatedObject.FlipX = true;
                }
                else if (CheckInput(GbaInput.Right))
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
                    Fsm.ChangeAction(Fsm_Dying);
                    return;
                }

                if (HitPoints != 0 && Timer > 90)
                {
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }

                if (HitPoints != 0 && CheckSingleInput(GbaInput.A) && field27_0x9c == 0)
                {
                    Fsm.ChangeAction(Fsm_Helico);
                    return;
                }

                if (HitPoints != 0 && CheckSingleInput(GbaInput.A) && field27_0x9c != 0)
                {
                    Fsm.ChangeAction(FUN_0802ddac);
                    return;
                }

                if (HitPoints != 0 && IsNearHangableEdge())
                {
                    Fsm.ChangeAction(Fsm_HangOnEdge);
                    return;
                }

                if (HitPoints != 0 && HasLanded())
                {
                    NextActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                if (HitPoints != 0 && Timer > 10 && IsOnHangable())
                {
                    BeginHang();
                    Fsm.ChangeAction(Fsm_Hang);
                    return;
                }

                if (HitPoints != 0 && !Flag2_1 && IsOnClimbableVertical() == 1)
                {
                    Fsm.ChangeAction(Fsm_Climb);
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
                    AttachedObject.ProcessMessage((Message)0x400); // TODO: Implement
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
                    field18_0x93 = 120;
                    Scene.Camera.ProcessMessage(Message.Cam_1040, field18_0x93);
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
                    field18_0x93 = 120;
                    Scene.Camera.ProcessMessage(Message.Cam_1040, field18_0x93);
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
                cam.HorizontalOffset = Engine.Settings.Platform switch
                {
                    Platform.GBA => 120,
                    Platform.NGage => 88,
                    _ => throw new UnsupportedPlatformException()
                };
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
                    Fsm.ChangeAction(Fsm_Default);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    // TODO: Implement all of these
    private void FUN_0802ce54(FsmAction action) { }
    private void FUN_080284ac(FsmAction action) { }
    private void FUN_08033b34(FsmAction action) { }
    private void FUN_080287d8(FsmAction action) { }
    private void FUN_0802ddac(FsmAction action) { }
    private void FUN_08033228(FsmAction action) { }
    private void FUN_08031554(FsmAction action) { }
    private void FUN_080224f4(FsmAction action) { }
    private void FUN_1005bf7c(FsmAction action) { }
    private void FUN_08027b80(FsmAction action) { }
    private void FUN_1005dea0(FsmAction action) { }
    private void FUN_1005dfa4(FsmAction action) { }
    private void FUN_1005e04c(FsmAction action) { }
}