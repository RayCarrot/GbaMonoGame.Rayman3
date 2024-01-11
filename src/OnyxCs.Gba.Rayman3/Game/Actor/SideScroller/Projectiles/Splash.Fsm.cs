using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public partial class Splash
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
                    if (!SoundEventsManager.IsPlaying(Rayman3SoundEvent.Play__SplshGen_Mix04))
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__SplshGen_Mix04);

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