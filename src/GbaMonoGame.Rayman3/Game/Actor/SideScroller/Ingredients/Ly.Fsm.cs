﻿using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class Ly
{
    private void Fsm_Init(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                TextBox = Scene.GetDialog<TextBoxDialog>();
                State.MoveTo(Fsm_Idle);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Idle(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.IdleWaiting;
                break;

            case FsmAction.Step:
                if (Scene.IsDetectedMainActor(this))
                    State.MoveTo(Fsm_Talk);
                break;

            case FsmAction.UnInit:
                Scene.Camera.ProcessMessage(this, Message.Cam_FollowPositionY, 160);
                break;
        }
    }

    private void Fsm_Talk(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                Scene.MainActor.ProcessMessage(this, Message.Main_EnterCutscene);
                ActionId = Action.BeginTalk;
                Timer = 0;
                TextBox.SetCutsceneCharacter(TextBoxCutsceneCharacter.Ly);
                SetText();
                break;

            case FsmAction.Step:
                Timer++;

                if (ActionId == Action.BeginTalk && IsActionFinished)
                {
                    ActionId = Action.Talk1;
                    TextBox.MoveInOurOut(true);
                }
                else if (Timer > 120 && IsActionFinished)
                {
                    ActionId = Random.GetNumber(9) switch
                    {
                        0 => Action.Talk1,
                        1 => Action.Talk2,
                        2 => Action.Talk3,
                        3 => Action.Talk4,
                        _ => Action.Talk1
                    };

                    Timer = 0;
                }
                else if (IsActionFinished && ActionId != Action.IdleActive)
                {
                    ActionId = Action.IdleActive;
                }

                if (JoyPad.IsButtonJustPressed(GbaInput.A) && !TextBox.IsFinished && ActionId != Action.BeginTalk)
                    TextBox.MoveToNextText();

                if (TextBox.IsFinished)
                    State.MoveTo(Fsm_GivePower);
                break;

            case FsmAction.UnInit:
                Scene.Camera.ProcessMessage(this, Message.Cam_FollowPositionY, 120);
                TextBox.MoveInOurOut(false);
                break;
        }
    }

    private void Fsm_GivePower(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.GivePower1;
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__LyMagic1_Mix01);
                break;

            case FsmAction.Step:
                if (IsActionFinished)
                {
                    if (ActionId == Action.GivePower1)
                    {
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__LyMagic2_Mix07);
                        ActionId = Action.GivePower2;
                    }
                    else if (ActionId == Action.GivePower2)
                    {
                        ActionId = Action.GivePower3;
                    }
                    else if (ActionId == Action.GivePower3)
                    {
                        ActionId = Action.GivePower4;
                    }
                    else if (ActionId == Action.GivePower4)
                    {
                        ActionId = Action.GivePower5;
                    }
                    else if (ActionId == Action.GivePower5)
                    {
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__NewPower_Mix06);
                        ((Rayman)Scene.MainActor).ActionId = Rayman.Action.NewPower_Right;

                        ChainedSparkles sparkle = Scene.CreateProjectile<ChainedSparkles>(ActorType.ChainedSparkles);
                        if (sparkle != null)
                        {
                            sparkle.InitNewPower();
                            sparkle.AreSparklesFacingLeft = false;
                        }

                        sparkle = Scene.CreateProjectile<ChainedSparkles>(ActorType.ChainedSparkles);
                        if (sparkle != null)
                        {
                            sparkle.InitNewPower();
                            sparkle.AreSparklesFacingLeft = true;
                        }
                    }

                    ChangeAction();
                }

                if (IsActionFinished && ActionId == Action.GivePower5)
                    State.MoveTo(Fsm_RaymanReceivePower);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_RaymanReceivePower(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.IdleActive;
                Timer = 0;
                break;

            case FsmAction.Step:
                Timer++;

                if (Scene.MainActor.IsActionFinished && ((Rayman)Scene.MainActor).ActionId == Rayman.Action.NewPower_Right)
                {
                    Scene.MainActor.ProcessMessage(this, Message.Main_ExitStopOrCutscene);
                    SetPowerAndReplayData();
                }

                if (Timer > 150)
                    State.MoveTo(Fsm_Leave);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Leave(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.Leave;
                break;

            case FsmAction.Step:
                if (IsActionFinished)
                    State.MoveTo(Fsm_Idle);
                break;

            case FsmAction.UnInit:
                ProcessMessage(this, Message.Destroy);
                break;
        }
    }
}