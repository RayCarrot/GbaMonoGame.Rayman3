﻿using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class Teensies
{
    private bool Fsm_WaitMaster(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsFacingRight ? Action.Init_Master_Right : Action.Init_Master_Left;
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);

                if (!HasSetTextBox)
                {
                    TextBox = Scene.GetRequiredDialog<TextBoxDialog>();
                    HasSetTextBox = true;
                }

                // Why is this called a second time...? Probably a mistake.
                LevelMusicManager.PlaySpecialMusicIfDetected(this);

                SetMasterAction();

                bool requirementMet = IsWorldFinished() && IsEnoughCagesTaken();

                if (Scene.IsDetectedMainActor(this) && InitialActionId is Action.Init_World1_Right or Action.Init_World1_Left)
                {
                    Scene.MainActor.ProcessMessage(this, Message.Main_EnterCutscene);
                    State.MoveTo(Fsm_World1IntroText);
                    return false;
                }

                if (Scene.IsDetectedMainActor(this) && requirementMet)
                {
                    Scene.MainActor.ProcessMessage(this, Message.Main_EnterCutscene);
                    State.MoveTo(Fsm_ShowRequirementMetText);
                    return false;
                }

                if (Scene.IsDetectedMainActor(this) && !requirementMet)
                {
                    Scene.MainActor.ProcessMessage(this, Message.Main_EnterCutscene);
                    State.MoveTo(Fsm_ShowRequirementNotMetText);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                TextBox.SetCutsceneCharacter(TextBoxCutsceneCharacter.Teensies);
                TextBox.MoveInOurOut(true);
                ((World)Frame.Current).UserInfo.Hide = true;
                break;
        }

        return true;
    }

    private bool Fsm_World1IntroText(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsFacingRight ? Action.Init_World1_Right : Action.Init_World1_Left;
                TextBox.SetText(0);
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);

                if (IsActionFinished)
                    ActionId = Random.GetNumber(3) * 2 + (IsFacingRight ? Action.Init_World1_Right : Action.Init_World1_Left);

                if (JoyPad.IsButtonJustPressed(GbaInput.A))
                    TextBox.MoveToNextText();

                if (TextBox.IsFinished && IsWorldFinished() && IsEnoughCagesTaken())
                {
                    State.MoveTo(Fsm_ShowRequirementMetText);
                    return false;
                }
                
                if (TextBox.IsFinished)
                {
                    State.MoveTo(Fsm_ShowRequirementNotMetText);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_ShowRequirementMetText(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsFacingRight ? Action.Init_ShowRequirementMet_Right : Action.Init_ShowRequirementMet_Left;
                IsSolid = false;
                SetRequirementMetText();
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);

                SetMasterAction();

                if (!IsMovingOutTextBox)
                {
                    if (TextBox.IsFinished)
                    {
                        TextBox.MoveInOurOut(false);
                        Scene.MainActor.ProcessMessage(this, Message.Main_ExitStopOrCutscene);
                        IsMovingOutTextBox = true;
                    }
                    else if (JoyPad.IsButtonJustPressed(GbaInput.A))
                    {
                        TextBox.MoveToNextText();
                    }
                }

                if (!TextBox.IsOnScreen())
                {
                    State.MoveTo(Fsm_ExitedRequirementMetText);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                ((World)Frame.Current).UserInfo.Hide = false;
                break;
        }

        return true;
    }

    private bool Fsm_ExitedRequirementMetText(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);
                SetMasterAction();
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_ShowRequirementNotMetText(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsFacingRight ? Action.Init_ShowRequirementNotMet_Left : Action.Init_ShowRequirementNotMet_Right;
                SetRequirementNotMetText();
                break;

            case FsmAction.Step:
                bool finished = false;

                LevelMusicManager.PlaySpecialMusicIfDetected(this);

                SetMasterAction();

                if (TextBox.IsFinished)
                {
                    TextBox.MoveInOurOut(false);
                    Scene.MainActor.ProcessMessage(this, Message.Main_ExitStopOrCutscene);
                    finished = true;
                }
                else if (JoyPad.IsButtonJustPressed(GbaInput.A))
                {
                    TextBox.MoveToNextText();
                }
                
                if (finished)
                {
                    State.MoveTo(Fsm_WaitExitRequirementNotMetText);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_WaitExitRequirementNotMetText(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);

                if (!TextBox.IsOnScreen())
                {
                    State.MoveTo(Fsm_ExitedRequirementNotMetText);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                ((World)Frame.Current).UserInfo.Hide = false;
                break;
        }

        return true;
    }

    private bool Fsm_ExitedRequirementNotMetText(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);

                SetMasterAction();

                if (HasLeftMainActorView())
                {
                    State.MoveTo(Fsm_WaitMaster);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    // Appears unused
    private bool Fsm_VictoryDance(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                IsSolid = false;
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);
                if (IsActionFinished)
                    ActionId = Random.GetNumber(2) * 2 + (IsFacingRight ? Action.Victory1_Right : Action.Victory1_Left);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_Idle(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                IsSolid = false;
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }
}