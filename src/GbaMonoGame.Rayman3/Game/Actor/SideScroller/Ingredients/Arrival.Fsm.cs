using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class Arrive
{
    private void Fsm_Idle(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.Idle;
                break;

            case FsmAction.Step:
                if (Scene.IsDetectedMainActor(this))
                {
                    Scene.MainActor.ProcessMessage(Message.Main_LevelEnd);
                    Fsm.ChangeAction(Fsm_EndLevel);
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_IdleWithLink(FsmAction action)
    {
        // TODO: Implement
        switch (action)
        {
            case FsmAction.Init:

                break;

            case FsmAction.Step:

                break;

            case FsmAction.UnInit:

                break;
        }
    }

    private void Fsm_EndLevel(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.EndingLevel;
                break;

            case FsmAction.Step:
                if (IsActionFinished && ActionId == Action.EndingLevel)
                {
                    if (GameInfo.MapId == MapId.ChallengeLyGCN)
                        Scene.MainActor.ProcessMessage(Message.Main_LevelEnd);

                    ActionId = Action.EndedLevel;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }
}