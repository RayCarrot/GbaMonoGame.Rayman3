using System;
using OnyxCs.Gba.Engine2d;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public partial class Rayman : MovableActor
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

                CameraSideScroller cam = (CameraSideScroller)Frame.GetComponent<Scene2D>().Camera;
                if (GameInfo.MapId == MapId.TheCanopy_M2)
                {
                    cam.HorizontalOffset = Gfx.Platform switch
                    {
                        Platform.GBA => 120,
                        Platform.NGage => 88,
                        _ => throw new UnsupportedPlatformException()
                    };
                }
                else
                {
                    if (!MultiplayerManager.IsInMultiplayer)
                        cam.HorizontalOffset = Gfx.Platform switch
                        {
                            Platform.GBA => 40,
                            Platform.NGage => 25,
                            _ => throw new UnsupportedPlatformException()
                        };
                    else if (IsLocalPlayer)
                        cam.HorizontalOffset = 95;
                }

                if (IsLocalPlayer)
                    // TODO: Name message
                    cam.ProcessMessage((Message)1027);

                if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4)
                    cam.HorizontalOffset = Gfx.Platform switch
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
                    if (TransitionsFX.FadeCoefficient == 0)
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

                if (PhysicalType != 32 && Math.Abs(MechSpeedX) > 1.5f)
                {
                    SlidingOnSlippery();
                }
                else
                {
                    PlaySound(208);

                    if (PhysicalType == 32)
                        MechSpeedX = 0;

                    if (!IsBossFight())
                    {
                        if (NextActionId == null)
                        {
                            // Randomly show Rayman being bored
                            if (Random.Shared.Next(11) < 6)
                                ActionId = IsFacingLeft ? Action.Idle_Bored_Left : Action.Idle_Bored_Right;
                            else
                                ActionId = IsFacingLeft ? Action.Idle_Left : Action.Idle_Right;
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
                        ActionId = IsFacingLeft ? Action.Idle_ReadyToFight_Left : Action.Idle_ReadyToFight_Right;
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

                    PlaySound(340);
                }

                if (PhysicalType != 32 && Math.Abs(MechSpeedX) > 1.5f)
                {
                    SlidingOnSlippery();
                }
                else
                {
                    PlaySound(208);

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

                    if (Gfx.Platform == Platform.NGage && MultiplayerManager.IsInMultiplayer)
                        throw new NotImplementedException();
                }
                else if (CheckInput(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Walk_Right;
                    ChangeAction();

                    if (Gfx.Platform == Platform.NGage && MultiplayerManager.IsInMultiplayer)
                        throw new NotImplementedException();
                }

                // Jump
                if (CheckSingleInput(GbaInput.A) && Flag2_2)
                {
                    PlaySound(340);
                    Fsm.ChangeAction(Fsm_Jump);
                    return;
                }
                
                // Crouch
                if (CheckInput(GbaInput.Down))
                {
                    NextActionId = IsFacingLeft ? Action.CrouchDown_Left : Action.CrouchDown_Right;
                    PlaySound(340);
                    Fsm.ChangeAction(FUN_080254d8);
                    return;
                }
                
                // Falling
                if (Speed.Y > 1)
                {
                    PlaySound(340);
                    Fsm.ChangeAction(FUN_0802cfec);
                    return;
                }
                
                // Walk
                if (CheckInput(GbaInput.Left) || CheckInput(GbaInput.Right))
                {
                    PlaySound(340);
                    Fsm.ChangeAction(Fsm_Walk);
                    return;
                }
                
                // Punch
                if (field23_0x98 == 0 && CheckSingleInput(GbaInput.B) && FUN_0802c414(2))
                {
                    PlaySound(340);
                    Fsm.ChangeAction(FUN_0802f5d8);
                    return;
                }

                if (MechSpeedX != 0 && FUN_0802986c() != 0 && !Flag1_1)
                {
                    PlaySound(340);
                    Position += new Vector2(MechSpeedX < 0 ? -16 : 16);
                    Fsm.ChangeAction(FUN_0802cfec);
                    return;
                }

                // Standing near edge
                if (MechSpeedX == 0 && FUN_0802986c() != 0 && !Flag1_1)
                {
                    PlaySound(340);
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
                    PlaySound(240);

                if (ActionId == NextActionId || ActionId is Action.Walk_Right or Action.Walk_Left or Action.Walk2_Right or Action.Walk2_Left)
                    NextActionId = null;

                if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4)
                {
                    CameraSideScroller cam = (CameraSideScroller)Frame.GetComponent<Scene2D>().Camera;

                    switch (Gfx.Platform)
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

    // TODO: Implement
    private void Fsm_StandingNearEdge(FsmAction action) { }
    private void Fsm_Walk(FsmAction action) { }
    private void Fsm_Jump(FsmAction action)
    {
        Scene2D scene = Frame.GetComponent<Scene2D>();
        CameraSideScroller cam = (CameraSideScroller)scene.Camera;

        switch (action)
        {
            case FsmAction.Init:
                if (scene.Camera.LinkedObject == this)
                    SoundManager.Play(208, -1);

                if (ActionId is not (Action.UnknownJump_Right or Action.UnknownJump_Left))
                {
                    ActionId = IsFacingLeft ? Action.Jump_Left : Action.Jump_Right;

                    if (scene.Camera.LinkedObject == this)
                        SoundManager.Play(108, -1);
                }

                NextActionId = null;
                field18_0x93 = 70;

                // TODO: Name message
                if (IsLocalPlayer)
                    cam.ProcessMessage((Message)1039, field18_0x93);

                Timer = (uint)GameTime.ElapsedFrames;
                PhysicalType = 32;
                LinkedMovementActor = null;
                break;

            case FsmAction.Step:
                // TODO: Implement

                break;

            case FsmAction.UnInit:
                Flag2_0 = false;
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
    private void FUN_080254d8(FsmAction action) { }
    private void FUN_0802ea74(FsmAction action) { }
    private void FUN_0802d44c(FsmAction action) { }
    private void FUN_0803283c(FsmAction action) { }
    private void FUN_08031d24(FsmAction action) { }
    private void FUN_0802cb38(FsmAction action) { }
    private void FUN_08032650(FsmAction action) { }
    private void FUN_08033228(FsmAction action) { }
    private void FUN_0802cfec(FsmAction action) { }
    private void FUN_0802f5d8(FsmAction action) { }
}