using System;

namespace GbaMonoGame.Rayman3;

public partial class Teensies
{
    private void Fsm_WaitMaster(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsFacingRight ? 19 : 18;
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);
                if (!HasSetTextBox)
                {
                    TextBox = Scene.GetRequiredDialog<TextBoxDialog>();
                    HasSetTextBox = true;
                }
                LevelMusicManager.PlaySpecialMusicIfDetected(this); // Why is this called a second time...? Probably a mistake.
                // TODO: Implement
                break;

            case FsmAction.UnInit:
                TextBox.SetCutsceneCharacter(TextBoxCutsceneCharacter.Teensies);
                TextBox.FUN_10077108(true);
                // TODO: Set some value in the current frame's user info
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
                    ActionId = Random.Shared.Next(2) * 2 + (IsFacingRight ? 3 : 2);
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