using System;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class Teensies
{
    private void Fsm_WaitMaster(FsmAction action)
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

                if (IsActionFinished)
                    ActionId = Random.Shared.Next(5) * 2 + (IsFacingRight ? Action.Master1_Right : Action.Master1_Left);

                bool requirementMet = IsWorldFinished() && IsEnoughCagesTaken();

                if (Scene.IsDetectedMainActor(this) && InitialActionId is Action.Init_World1_Right or Action.Init_World1_Left)
                {
                    Scene.MainActor.ProcessMessage((Message)1088); // TODO: Implement and name
                    Fsm.ChangeAction(Fsm_World1IntroText);
                    return;
                }

                if (Scene.IsDetectedMainActor(this) && requirementMet)
                {
                    Scene.MainActor.ProcessMessage((Message)1088); // TODO: Implement and name
                    //Fsm.ChangeAction(FUN_08078230); // TODO: Implement
                    return;
                }

                if (Scene.IsDetectedMainActor(this) && !requirementMet)
                {
                    Scene.MainActor.ProcessMessage((Message)1088); // TODO: Implement and name
                    //Fsm.ChangeAction(FUN_08078498); // TODO: Implement
                    return;
                }
                break;

            case FsmAction.UnInit:
                TextBox.SetCutsceneCharacter(TextBoxCutsceneCharacter.Teensies);
                TextBox.MoveInOurOut(true);
                ((World)Frame.Current).UserInfo.Hide = true;
                break;
        }
    }

    private void Fsm_World1IntroText(FsmAction action)
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
                    ActionId = Random.Shared.Next(3) * 2 + (IsFacingRight ? Action.Init_World1_Right : Action.Init_World1_Left);

                if (JoyPad.CheckSingle(GbaInput.A))
                    TextBox.FUN_100770e4();

                // TODO: Implement
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    // Appears unused
    private void Fsm_VictoryDance(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                IsSolid = false;
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);
                if (IsActionFinished)
                    ActionId = Random.Shared.Next(2) * 2 + (IsFacingRight ? Action.Victory1_Right : Action.Victory1_Left);
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
                IsSolid = false;
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }
}