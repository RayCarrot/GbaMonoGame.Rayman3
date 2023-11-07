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
                ActionId = IsFacingLeft ? Action.Spawn_Left : Action.Spawn_Right;
                ChangeAction();

                Timer = 0;

                CameraSideScroller cam = (CameraSideScroller)Frame.GetComponent<Scene2D>().Camera;
                if (GameInfo.MapId == MapId.TheCanopy_M2)
                {
                    cam.HorizontalOffset = 120;
                }
                else
                {
                    if (!MultiplayerManager.IsInMultiplayer)
                        cam.HorizontalOffset = 40;
                    else if (IsLocalPlayer)
                        cam.HorizontalOffset = 95;
                }

                if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4)
                    cam.HorizontalOffset = 120;
                break;

            case FsmAction.Step:
                // Check if we're spawning at a curtain
                if (Flag1_6)
                {
                    // Hide while fading and then show spawn animation
                    if (TransitionsFX.FadeCoefficient == 0)
                    {
                        if (ActionId is not (Action.Spawn_Curtain_Right or Action.Spawn_Curtain_Left))
                            ActionId = IsFacingLeft ? Action.Spawn_Curtain_Left : Action.Spawn_Curtain_Right;
                    }
                    else
                    {
                        ActionId = IsFacingLeft ? Action.Hidden_Left : Action.Hidden_Right;
                    }
                }

                Timer++;

                if (IsActionFinished && IsBossFight())
                    NextActionId = IsFacingLeft ? Action.Idle_Determined_Left : Action.Idle_Determined_Right;

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
        Scene2D scene = Frame.GetComponent<Scene2D>();
        CameraSideScroller cam = (CameraSideScroller)scene.Camera;

        switch (action)
        {
            case FsmAction.Init:
                UpdatePhysicalType();

                if (PhysicalType == 32 || Math.Abs(MechSpeedX) <= 1.5f)
                {
                    if (scene.Camera.LinkedObject == this)
                        SoundManager.Play(208, -1);

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
                else
                {
                    SlidingOnSlippery();
                }

                Timer = 0;
                break;

            case FsmAction.Step:
                if (Flag1_D)
                {
                    field16_0x91++;

                    if (field16_0x91 > 60)
                    {
                        cam.HorizontalOffset = 40;
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
                    if (!Fsm.EqualsAction(FUN_0802ddac) &&
                        CheckInput(GbaInput.Down) &&
                        (Speed.Y > 0 || Fsm.EqualsAction(FUN_080254d8)) &&
                        !Fsm.EqualsAction(FUN_08026cd4))
                    {
                        field18_0x93 = 70;
                        cam.ProcessMessage((Message)1040, field18_0x93);
                    }
                    else if (CheckInput(GbaInput.Up) && (Fsm.EqualsAction(Fsm_Default) || Fsm.EqualsAction(FUN_0802ea74)))
                    {
                        field18_0x93 = 160;
                        cam.ProcessMessage((Message)1040, field18_0x93);
                    }
                    else if (Fsm.EqualsAction(FUN_0802d44c) && field27_0x9c == 0)
                    {
                        cam.ProcessMessage((Message)1039, field18_0x93);
                    }
                    else if (Fsm.EqualsAction(FUN_0803283c))
                    {
                        field18_0x93 = 65;
                        cam.ProcessMessage((Message)1040, field18_0x93);
                    }
                    else if (Fsm.EqualsAction(FUN_08026cd4) || Fsm.EqualsAction(FUN_0802ddac))
                    {
                        field18_0x93 = 112;
                        cam.ProcessMessage((Message)1040, field18_0x93);
                    }
                    else
                    {
                        field18_0x93 = 120;
                        cam.ProcessMessage((Message)1040, field18_0x93);
                    }
                }

                if (field23_0x98 != 0)
                    field23_0x98--;

                if (!IsOnInstaKillType())
                {
                    UpdatePhysicalType();
                    HorizontalMovement();

                    if (CheckForDamage())
                    {
                        Fsm.ChangeAction(FUN_08031d24);

                        if (ActionId is not (Action.LookUp_Right or Action.LookUp_Left) && PhysicalType == 32)
                            ActionId = IsFacingLeft ? Action.LookUp_Left : Action.LookUp_Right;
                    }
                    else
                    {
                        if (CheckSingleInput(GbaInput.A) && PhysicalType != 32)
                        {
                            Fsm.ChangeAction(FUN_0802cb38);

                            if (ActionId is not (Action.LookUp_Right or Action.LookUp_Left) && PhysicalType == 32)
                                ActionId = IsFacingLeft ? Action.LookUp_Left : Action.LookUp_Right;
                        }
                        else
                        {
                            if (FUN_0802a0f8())
                            {
                                if (cam.LinkedObject == this)
                                    SoundManager.Play(208, -1);

                                Fsm.ChangeAction(FUN_0802cb38);

                                if (ActionId is not (Action.LookUp_Right or Action.LookUp_Left) && PhysicalType == 32)
                                    ActionId = IsFacingLeft ? Action.LookUp_Left : Action.LookUp_Right;
                            }
                            else
                            {
                                Timer++;

                                if (!CheckInput(GbaInput.Up))
                                {
                                    if (ActionId is Action.LookUp_Right or Action.LookUp_Left)
                                    {
                                        ActionId = IsFacingLeft ? Action.Idle_Left : Action.Idle_Right;
                                    }
                                }
                                else
                                {
                                    if (ActionId is not (Action.LookUp_Right or Action.LookUp_Left) && PhysicalType == 32)
                                        ActionId = IsFacingLeft ? Action.LookUp_Left : Action.LookUp_Right;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!MultiplayerManager.IsInMultiplayer)
                    {
                        Fsm.ChangeAction(FUN_08032650);
                    }
                    else
                    {
                        Fsm.ChangeAction(FUN_08033228);
                    }

                    if (ActionId is not (Action.LookUp_Right or Action.LookUp_Left) && PhysicalType == 32)
                        ActionId = IsFacingLeft ? Action.LookUp_Left : Action.LookUp_Right;
                }

                if (IsActionFinished)
                {
                    if ((ActionId == NextActionId && ActionId is not (
                            Action.Idle_Bored_Right or Action.Idle_Bored_Left or 
                            Action.EnterCurtain_Right or Action.EnterCurtain_Left)) &&
                        (ActionId is not (
                            Action.Idle_BasketBall_Right or Action.Idle_BasketBall_Left or 
                            Action.Idle_Grimace_Right or Action.Idle_Grimace_Left) || 
                         Timer > 180))
                    {
                        if (!IsBossFight())
                        {
                            ActionId = IsFacingLeft ? Action.Idle_Left : Action.Idle_Right;
                        }
                        else
                        {
                            ActionId = IsFacingLeft ? Action.Idle_ReadyToFight_Left : Action.Idle_ReadyToFight_Right;
                        }

                        NextActionId = null;

                        if (cam.LinkedObject == this)
                            SoundManager.Play(340, -1);
                    }
                }

                if (PhysicalType == 32 || Math.Abs(MechSpeedX) <= 1.5f)
                {
                    if (cam.LinkedObject == this)
                        SoundManager.Play(208, -1);

                    if (NextActionId != null && NextActionId != ActionId)
                    {
                        ActionId = NextActionId.Value;
                    }
                    else
                    {
                        if (ActionId is not (
                                Action.LookUp_Right or Action.LookUp_Left or 
                                Action.Idle_Left or Action.Idle_Right or 
                                Action.Idle_ReadyToFight_Right or Action.Idle_ReadyToFight_Left) && 
                            ActionId != NextActionId)
                        {
                            if (!IsBossFight())
                            {
                                ActionId = IsFacingLeft ? Action.Idle_Left : Action.Idle_Right;
                            }
                            else
                            {
                                ActionId = IsFacingLeft ? Action.Idle_ReadyToFight_Left : Action.Idle_ReadyToFight_Right;
                            }
                        }
                    }
                }
                else
                {
                    SlidingOnSlippery();
                }

                if (CheckInput(GbaInput.Left) && !IsFacingLeft)
                {
                    ActionId = Action.Walk_Left;
                    ChangeAction();
                }
                else if (CheckInput(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = Action.Walk_Right;
                    ChangeAction();
                }

                if (CheckSingleInput(GbaInput.A) && Flag2_2)
                {
                    if (cam.LinkedObject == this)
                        SoundManager.Play(340, -1);

                    Fsm.ChangeAction(Fsm_Jump);
                }
                else if (CheckInput(GbaInput.Down))
                {
                    NextActionId = IsFacingLeft ? Action.CrouchDown_Left : Action.CrouchDown_Right;

                    if (cam.LinkedObject == this)
                        SoundManager.Play(340, -1);

                    Fsm.ChangeAction(FUN_080254d8);
                }
                else if (Speed.Y > 1)
                {
                    if (cam.LinkedObject == this)
                        SoundManager.Play(340, -1);

                    Fsm.ChangeAction(FUN_0802cfec);
                }
                else if (CheckInput(GbaInput.Left) || CheckInput(GbaInput.Right))
                {
                    if (cam.LinkedObject == this)
                        SoundManager.Play(340, -1);

                    Fsm.ChangeAction(FUN_08024470);
                }
                else if (field23_0x98 == 0 && CheckSingleInput(GbaInput.B) && FUN_0802c414(2))
                {
                    if (cam.LinkedObject == this)
                        SoundManager.Play(340, -1);

                    Fsm.ChangeAction(FUN_0802f5d8);
                }
                else if (MechSpeedX == 0 || FUN_0802986c() == 0 || Flag1_1)
                {
                    if (MechSpeedX != 0 || FUN_0802986c() == 0 || Flag1_1)
                    {
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
                        }
                        else
                        {
                            Flag1_1 = false;
                        }
                    }
                    else
                    {
                        if (cam.LinkedObject == this)
                            SoundManager.Play(340, -1);

                        Fsm.ChangeAction(Fsm_StandingNearEdge);
                    }
                }
                else
                {
                    if (cam.LinkedObject == this)
                        SoundManager.Play(340, -1);

                    Position += new Vector2(MechSpeedX < 0 ? -16 : 16);

                    Fsm.ChangeAction(FUN_0802cfec);
                }
                break;

            case FsmAction.UnInit:
                if (ActionId is Action.Idle_SpinBody_Right or Action.Idle_SpinBody_Left && cam.LinkedObject == this)
                    SoundManager.Play(240, -1);

                if (ActionId == NextActionId || ActionId is Action.Walk_Right or Action.Walk_Left or Action.Walk2_Right or Action.Walk2_Left)
                    NextActionId = null;

                if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4 && cam.HorizontalOffset == 120)
                    cam.HorizontalOffset = 40;
                break;
        }
    }

    // TODO: Implement
    private void Fsm_StandingNearEdge(FsmAction action) { }

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
    private void FUN_08024470(FsmAction action)
    {

    }
    private void FUN_0802f5d8(FsmAction action) { }
}