using BinarySerializer.Onyx.Gba.Rayman3;
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
                    if (!SoundManager.IsPlaying(Rayman3SoundEvent.Play__484_SplshGen_Mix04))
                        SoundManager.Play(Rayman3SoundEvent.Play__484_SplshGen_Mix04);

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