using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public partial class Splash : BaseActor
{
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
                    if (!SoundManager.IsPlaying(124))
                        SoundManager.Play(124, -1);

                    ActionId = 0;
                }

                if (IsActionFinished)
                    Fsm.ChangeAction(Fsm_Default);
                break;

            case FsmAction.UnInit:
                ProcessMessage(Message.Destroy);
                break;
        }
    }
}