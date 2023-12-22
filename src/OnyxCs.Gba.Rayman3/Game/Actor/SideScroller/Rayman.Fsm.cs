using System;
using BinarySerializer.Onyx.Gba;
using BinarySerializer.Onyx.Gba.Rayman3;
using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public partial class Rayman
{
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
                    if (!MultiplayerManager.IsInMultiplayer)
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
                if (Flag1_6)
                {
                    // Hide while fading and then show spawn animation
                    if (!((FrameSideScroller)Frame.Current).TransitionsFX.IsFading)
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

                if (!MultiplayerManager.IsInMultiplayer || Timer >= 210)
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
                UpdatePhysicalType();

                if (IsSliding)
                {
                    SlidingOnSlippery();
                }
                else
                {
                    PlaySoundEvent(Rayman3SoundEvent.Stop__SkiLoop1);

                    if (PhysicalType == 32)
                        MechSpeedX = 0;

                    if (!IsBossFight())
                    {
                        if (NextActionId == null)
                        {
                            // Randomly show Rayman being bored
                            if (Random.Shared.Next(11) < 6)
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
                if (!Inlined_FUN_1004c544())
                    return;

                Timer++;

                // Look up when pressing up
                if (CheckInput(GbaInput.Up))
                {
                    if (ActionId is not (Action.LookUp_Right or Action.LookUp_Left) && PhysicalType == 32)
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
                        Action.EnterCurtain_Right or Action.EnterCurtain_Left)) &&
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

                    PlaySoundEvent(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                }

                if (IsSliding)
                {
                    SlidingOnSlippery();
                }
                else
                {
                    PlaySoundEvent(Rayman3SoundEvent.Stop__SkiLoop1);

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

                    if (Engine.Settings.Platform == Platform.NGage && MultiplayerManager.IsInMultiplayer)
                        throw new NotImplementedException();
                }
                else if (CheckInput(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Walk_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && MultiplayerManager.IsInMultiplayer)
                        throw new NotImplementedException();
                }

                // Jump
                if (CheckSingleInput(GbaInput.A) && Flag2_2)
                {
                    PlaySoundEvent(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    Fsm.ChangeAction(Fsm_Jump);
                    return;
                }
                
                // Crouch
                if (CheckInput(GbaInput.Down))
                {
                    NextActionId = IsFacingRight ? Action.CrouchDown_Right : Action.CrouchDown_Left;
                    PlaySoundEvent(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    Fsm.ChangeAction(Fsm_Crouch);
                    return;
                }
                
                // Fall
                if (Speed.Y > 1)
                {
                    PlaySoundEvent(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }
                
                // Walk
                if (CheckInput(GbaInput.Left) || CheckInput(GbaInput.Right))
                {
                    PlaySoundEvent(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    Fsm.ChangeAction(Fsm_Walk);
                    return;
                }
                
                // Punch
                if (field23_0x98 == 0 && CheckSingleInput(GbaInput.B) && CanAttackWithFist(2))
                {
                    PlaySoundEvent(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    Fsm.ChangeAction(Fsm_ChargeAttack);
                    return;
                }

                // Walking off edge
                if (MechSpeedX != 0 && IsNearEdge() != 0 && !Flag1_1)
                {
                    PlaySoundEvent(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    Position += new Vector2(MechSpeedX < 0 ? -16 : 16, 0);
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }

                // Standing near edge
                if (MechSpeedX == 0 && IsNearEdge() != 0 && !Flag1_1)
                {
                    PlaySoundEvent(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                    Fsm.ChangeAction(Fsm_StandingNearEdge);
                    return;
                }

                // Restart default state
                if ((IsActionFinished && ActionId is not (
                         Action.LookUp_Right or Action.LookUp_Left or
                         Action.Idle_Bored_Right or Action.Idle_Bored_Left or
                         Action.EnterCurtain_Right or Action.EnterCurtain_Left) &&
                     360 < Timer) ||
                    (IsActionFinished && ActionId is
                         Action.Idle_Bored_Right or Action.Idle_Bored_Left or
                         Action.EnterCurtain_Right or Action.EnterCurtain_Left &&
                     720 < Timer))
                {
                    FUN_0802a65c();
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                Flag1_1 = false;

                break;

            case FsmAction.UnInit:
                if (ActionId is Action.Idle_SpinBody_Right or Action.Idle_SpinBody_Left)
                    PlaySoundEvent(Rayman3SoundEvent.Stop__RaySpin_Mix06);

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
                if (!SoundManager.IsPlaying(Rayman3SoundEvent.Play__OnoEquil_Mix03))
                    PlaySoundEvent(Rayman3SoundEvent.Play__OnoEquil_Mix03);

                Timer = 120;
                NextActionId = null;

                Box detectionBox = GetDetectionBox();
                PhysicalType rightType = Scene.GetPhysicalType(new Vector2(detectionBox.MaxX, detectionBox.MaxY));

                if (rightType.IsSolid)
                    ActionId = IsFacingRight ? Action.NearEdgeBehind_Right : Action.NearEdgeFront_Left;
                else
                    ActionId = IsFacingRight ? Action.NearEdgeFront_Right : Action.NearEdgeBehind_Right;
                break;

            case FsmAction.Step:
                if (!Inlined_FUN_1004c544())
                    return;

                Timer--;

                // Play sound every 2 seconds
                if (Timer == 0)
                {
                    Timer = 120;

                    if (!SoundManager.IsPlaying(Rayman3SoundEvent.Play__OnoEquil_Mix03))
                        PlaySoundEvent(Rayman3SoundEvent.Play__OnoEquil_Mix03);
                }

                // Change direction
                if (CheckInput(GbaInput.Left) && IsFacingRight)
                {
                    ActionId = Action.Walk_Left;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && MultiplayerManager.IsInMultiplayer)
                        throw new NotImplementedException();
                }
                else if (CheckInput(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Walk_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && MultiplayerManager.IsInMultiplayer)
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
                if (CheckSingleReleasedInput(GbaInput.B) && CanAttackWithFist(2))
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
                    PlaySoundEvent(Rayman3SoundEvent.Stop__SkiLoop1);

                    if (PhysicalType == 32)
                        MechSpeedX = 0;

                    if (!MultiplayerManager.IsInMultiplayer)
                    {
                        // Randomly look around for Globox in the first level
                        if (GameInfo.MapId == MapId.WoodLight_M1 && GameInfo.LastGreenLumAlive == 0)
                        {
                            if (Random.Shared.Next(501) >= 401)
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
                if (!Inlined_FUN_1004c544())
                    return;

                if (Speed.Y > 1 && MechSpeedX == 0)
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
                            field22_0x97 > Random.Shared.Next(121) + 120)
                        {
                            ActionId = IsFacingRight ? Action.Walk_LookAround_Right : Action.Walk_LookAround_Left;
                            field22_0x97 = 0;
                        }
                        else if (ActionId is Action.Walk_LookAround_Right or Action.Walk_LookAround_Left && 
                                 field22_0x97 > Random.Shared.Next(121) + 60)
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

                        if (Engine.Settings.Platform == Platform.NGage && MultiplayerManager.IsInMultiplayer)
                            throw new NotImplementedException();
                    }
                    else if (CheckInput(GbaInput.Right) && IsFacingLeft)
                    {
                        ActionId = Action.Walk_LookAround_Right;
                        ChangeAction();

                        if (Engine.Settings.Platform == Platform.NGage && MultiplayerManager.IsInMultiplayer)
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    if (CheckInput(GbaInput.Left) && IsFacingRight)
                    {
                        ActionId = Action.Walk_Left;
                        ChangeAction();

                        if (Engine.Settings.Platform == Platform.NGage && MultiplayerManager.IsInMultiplayer)
                            throw new NotImplementedException();
                    }
                    else if (CheckInput(GbaInput.Right) && IsFacingLeft)
                    {
                        ActionId = Action.Walk_Right;
                        ChangeAction();

                        if (Engine.Settings.Platform == Platform.NGage && MultiplayerManager.IsInMultiplayer)
                            throw new NotImplementedException();
                    }
                }

                if (CheckInput(GbaInput.B))
                {
                    Charge++;
                }
                else if (CheckSingleReleasedInput(GbaInput.B) && field23_0x98 == 0)
                {
                    Charge = 0;

                    if (CanAttackWithFist(1))
                    {
                        Attack(0, 0, new Vector2(16, -16), 0);
                        field23_0x98 = 0;
                    }
                    else if (CanAttackWithFist(2))
                    {
                        Attack(0, 1, new Vector2(16, -16), 0);

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
                    if (PhysicalType != 32 && 
                        ActionId is
                            Action.Walk_Right or Action.Walk_Left or
                            Action.Walk_LookAround_Right or Action.Walk_LookAround_Left &&
                        AnimatedObject.CurrentFrame is 2 or 10 &&
                        !AnimatedObject.HasExecutedFrame)
                    {
                        PlaySoundEvent(Rayman3SoundEvent.Play__PlumSnd2_Mix03);
                    }

                    PlaySoundEvent(Rayman3SoundEvent.Stop__SkiLoop1);

                    if (!MultiplayerManager.IsInMultiplayer)
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
                if (MechSpeedX != 0 && Speed.Y > 1)
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
                PlaySoundEvent(Rayman3SoundEvent.Stop__SkiLoop1);

                if (ActionId is not (Action.UnknownJump_Right or Action.UnknownJump_Left))
                {
                    ActionId = IsFacingRight ? Action.Jump_Right : Action.Jump_Left;
                    PlaySoundEvent(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                }

                NextActionId = null;
                field18_0x93 = 70;

                if (IsLocalPlayer)
                    cam.ProcessMessage(Message.Cam_1039, field18_0x93);

                Timer = (uint)GameTime.ElapsedFrames;
                PhysicalType = 32;
                LinkedMovementActor = null;
                break;

            case FsmAction.Step:
                if (!DoInTheAir())
                    return;

                if (IsLocalPlayer)
                    cam.ProcessMessage(Message.Cam_1039, (byte)130);

                if (ActionId is Action.Jump_Right or Action.Jump_Left &&
                    CheckReleasedInput(GbaInput.A) && 
                    Mechanic.Speed.Y < -4 && 
                    !Flag2_0)
                {
                    Mechanic.Speed = new Vector2(Mechanic.Speed.X, -4);
                    Flag2_0 = true;
                }

                if (Speed.Y == 0 && Mechanic.Speed.Y < 0)
                    Mechanic.Speed = new Vector2(Mechanic.Speed.X, 0);

                MoveInTheAir(MechSpeedX);
                FUN_0802c3c8();
                AttackInTheAir();

                if (HasLanded())
                {
                    NextActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                if (GameTime.ElapsedFrames - Timer >= 51)
                {
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }

                if (IsNearHangableEdge())
                {
                    Fsm.ChangeAction(Fsm_Hang);
                    return;
                }

                // TODO: Implement
                break;

            case FsmAction.UnInit:
                Flag2_0 = false;
                break;
        }
    }

    private void Fsm_Hang(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                PlaySoundEvent(Rayman3SoundEvent.Play__HandTap1_Mix04);
                MechSpeedX = 0;

                if (NextActionId is Action.UnknownBeginHang_Right or Action.UnknownBeginHang_Left)
                    ActionId = IsFacingRight ? Action.UnknownBeginHang_Right : Action.UnknownBeginHang_Left;
                else
                    ActionId = IsFacingRight ? Action.BeginHang_Right : Action.BeginHang_Left;

                SetDetectionBox(new Box(
                    minX: ActorModel.DetectionBox.MinX,
                    minY: ActorModel.DetectionBox.MaxY - 16,
                    maxX: ActorModel.DetectionBox.MaxX,
                    maxY: ActorModel.DetectionBox.MaxY + 16));
                break;

            case FsmAction.Step:
                if (!DoInTheAir())
                    return;

                if (IsActionFinished && ActionId is not (Action.Hang_Right or Action.Hang_Left))
                {
                    ActionId = IsFacingRight ? Action.Hang_Right : Action.Hang_Left;
                    NextActionId = null;
                }

                // Move down
                if (CheckInput(GbaInput.Down))
                {
                    HangDelay = 30;
                    PlaySoundEvent(Rayman3SoundEvent.Play__OnoJump1__or__OnoJump3_Mix01__or__OnoJump4_Mix01__or__OnoJump5_Mix01__or__OnoJump6_Mix01);
                    Fsm.ChangeAction(Fsm_Fall);
                    return;
                }

                // Jump
                if (CheckSingleInput(GbaInput.A))
                {
                    HangDelay = 30;
                    Fsm.ChangeAction(Fsm_Jump);
                    return;
                }

                // Attack
                if (CheckSingleReleasedInput(GbaInput.B) && CanAttackWithFeet())
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
                PlaySoundEvent(Rayman3SoundEvent.Stop__SkiLoop1);
                ActionId = IsFacingRight ? Action.Fall_Right : Action.Fall_Left;
                NextActionId = null;
                Timer = 0;
                break;

            case FsmAction.Step:
                if (!DoInTheAir())
                    return;
                
                Timer++;

                if (Flag2_4 && Timer > 15)
                    Flag2_4 = false;

                MoveInTheAir(MechSpeedX);
                FUN_0802c3c8();
                AttackInTheAir();

                if (CheckSingleInput(GbaInput.A) && Flag2_4)
                {
                    Fsm.ChangeAction(Fsm_Jump);
                    return;
                }
                
                if (CheckSingleInput(GbaInput.A) && field27_0x9c == 0)
                {
                    Fsm.ChangeAction(FUN_0802d44c);
                    return;
                }

                if (CheckSingleInput(GbaInput.A) && field27_0x9c != 0)
                {
                    Fsm.ChangeAction(FUN_0802ddac);
                    return;
                }

                if (IsNearHangableEdge())
                {
                    Fsm.ChangeAction(Fsm_Hang);
                    return;
                }

                if (HasLanded())
                {
                    NextActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                    Fsm.ChangeAction(Fsm_Default);
                    return;
                }

                // TODO: Implement
                //if (FUN_080299d4())
                //{
                //    FUN_08029c84();
                //    Fsm.ChangeAction(FUN_0802ee60);
                //    return;
                //}

                //if (FUN_08029a78())
                //{
                //    Fsm.ChangeAction(FUN_08026cd4);
                //    return;
                //}

                //if (CheckInput(GbaInput.L) && FUN_0802a420())
                //{
                //    FUN_0802a4c0();
                //    Fsm.ChangeAction(FUN_08031554);
                //}
                break;

            case FsmAction.UnInit:
                Flag2_4 = false;
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
                    PlaySoundEvent(Rayman3SoundEvent.Stop__SkiLoop1);
                    
                    if (PhysicalType == 32)
                        MechSpeedX = 0;

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
                if (!Inlined_FUN_1004c544())
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
                    PlaySoundEvent(Rayman3SoundEvent.Stop__SkiLoop1);

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

                    if (Engine.Settings.Platform == Platform.NGage && MultiplayerManager.IsInMultiplayer)
                        throw new NotImplementedException();
                }
                else if (CheckInput(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Crawl_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && MultiplayerManager.IsInMultiplayer)
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
                    PlaySoundEvent(Rayman3SoundEvent.Stop__SkiLoop1);

                    if (PhysicalType == 32)
                        MechSpeedX = 0;

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
                if (!Inlined_FUN_1004c544())
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

                    if (Engine.Settings.Platform == Platform.NGage && MultiplayerManager.IsInMultiplayer)
                        throw new NotImplementedException();
                }
                else if (CheckInput(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Crawl_Right;
                    ChangeAction();

                    if (Engine.Settings.Platform == Platform.NGage && MultiplayerManager.IsInMultiplayer)
                        throw new NotImplementedException();
                }

                if (IsSliding)
                {
                    SlidingOnSlippery();
                }
                else
                {
                    PlaySoundEvent(Rayman3SoundEvent.Stop__SkiLoop1);

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

    private void Fsm_EndMap(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ((FrameSideScroller)Frame.Current).CanPause = false;
                Flag2_1 = false;
                NextActionId = null;
                MechSpeedX = 0;
                if (HasLanded())
                {
                    if (FinishedMap)
                    {
                        Timer = 0;
                        ActionId = IsFacingRight ? Action.Victory_Right : Action.Victory_Left;
                        PlaySoundEvent(Rayman3SoundEvent.Play__OnoWin_Mix02__or__OnoWinRM_Mix02);

                        if (GameInfo.MapId != MapId.BossBadDreams &&
                            GameInfo.MapId != MapId.BossScaleMan &&
                            GameInfo.MapId != MapId.BossFinal_M1 &&
                            GameInfo.MapId != MapId.BossFinal_M2 &&
                            GameInfo.MapId != MapId.BossMachine &&
                            GameInfo.MapId != MapId.BossRockAndLava)
                        {
                            SoundManager.FUN_080abe44(Rayman3SoundEvent.Play__win3, 0);
                        }
                        else
                        {
                            SoundManager.FUN_08001954(Rayman3SoundEvent.Play__Win_BOSS);
                        }
                    }
                    else
                    {
                        ActionId = IsFacingRight ? Action.ReturnFromLevel_Right : Action.ReturnFromLevel_Left;
                        Timer = 0;
                    }

                    ((FrameSideScroller)Frame.Current).IsTimed = false;
                }
                else
                {
                    ActionId = IsFacingRight ? Action.Fall_Right : Action.Fall_Left;
                }
                break;

            case FsmAction.Step:
                if (SoundManager.IsPlaying(Rayman3SoundEvent.Play__win3))
                    SoundManager.Play(Rayman3SoundEvent.Stop__canopy);

                // Don't allow horizontal movement while falling
                if (ActionId is Action.Fall_Right or Action.Fall_Left)
                    Mechanic.Speed = new Vector2(0, Mechanic.Speed.Y);

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
                        PlaySoundEvent(Rayman3SoundEvent.Play__OnoWin_Mix02__or__OnoWinRM_Mix02);

                        if (GameInfo.MapId != MapId.BossBadDreams &&
                            GameInfo.MapId != MapId.BossScaleMan &&
                            GameInfo.MapId != MapId.BossFinal_M1 &&
                            GameInfo.MapId != MapId.BossFinal_M2 &&
                            GameInfo.MapId != MapId.BossMachine &&
                            GameInfo.MapId != MapId.BossRockAndLava)
                        {
                            SoundManager.FUN_080abe44(Rayman3SoundEvent.Play__win3, 0);
                        }
                        else
                        {
                            SoundManager.FUN_08001954(Rayman3SoundEvent.Play__Win_BOSS);
                        }
                    }
                    else
                    {
                        ActionId = IsFacingRight ? Action.ReturnFromLevel_Right : Action.ReturnFromLevel_Left;
                        Timer = 0;
                    }

                    ((FrameSideScroller)Frame.Current).IsTimed = false;
                    return;
                }

                if (ActionId is Action.Idle_Right or Action.Idle_Left or Action.ReturnFromLevel_Right or Action.ReturnFromLevel_Left &&
                    ((!FinishedMap && Timer == 150) || (FinishedMap && Timer == 100)))
                {
                    if (SoundManager.IsPlaying(Rayman3SoundEvent.Play__Win_BOSS))
                    {
                        Timer -= 2;
                        return;
                    }

                    // TODO: N-Gage already returns here - why?
                    if (Engine.Settings.Platform == Platform.GBA)
                    {
                        if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4)
                            SoundManager.StopAll();

                        if (FinishedMap)
                            SoundManager.StopAll();
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
                    {
                        // TODO: Call function in GameCube frame class
                    }

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
                            // TODO: Implement
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
                        // TODO: Load GCN menu
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
                                UpdateLastCompletedLevel();
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
                        // TODO: Load GCN menu
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

    // TODO: Implement all of these
    private void FUN_0802ce54(FsmAction action) { }
    private void FUN_080284ac(FsmAction action) { }
    private void FUN_08033b34(FsmAction action) { }
    private void FUN_080287d8(FsmAction action) { }
    private void FUN_0802ddac(FsmAction action) { }
    private void FUN_08026cd4(FsmAction action) { }
    private void FUN_0802d44c(FsmAction action) { }
    private void FUN_0803283c(FsmAction action) { }
    private void FUN_08031d24(FsmAction action) { }
    private void FUN_0802cb38(FsmAction action) { }
    private void FUN_08032650(FsmAction action) { }
    private void FUN_08033228(FsmAction action) { }
    private void Fsm_ChargeAttack(FsmAction action) { }
    private void FUN_0802e770(FsmAction action) { }
    private void FUN_0802ee60(FsmAction action) { }
    private void FUN_08031554(FsmAction action) { }
}