using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public class Splash : BaseActor
{
    public Splash(int id, ActorResource actorResource) : base(id, actorResource)
    {
        Fsm.ChangeAction(Fsm_Default);
    }

    private void Fsm_Default(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                RewindAction();
                ActionId = 1;
                break;

            case FsmAction.Step:
                if (ActionId == 1)
                {
                    // TODO: Play sound
                    ActionId = 0;
                }

                if (IsActionFinished)
                    Fsm.ChangeAction(Fsm_Default);
                break;

            case FsmAction.UnInit:
                SendMessage(Message.Disable);
                break;
        }
    }
}