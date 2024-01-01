using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public partial class FlowerFire
{
    private void Fsm_Default(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                AnimatedObject.CurrentAnimation = 2;
                break;
            
            case FsmAction.Step:
                if (AnimatedObject.EndOfAnimation)
                    AnimatedObject.CurrentAnimation = 0;

                Timer--;

                if (Timer == 0)
                {
                    AnimatedObject.CurrentAnimation = 2;
                    ProcessMessage(Message.Destroy);
                    Platform.Destroy();
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_End(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                AnimatedObject.CurrentAnimation = 1;
                break;

            case FsmAction.Step:
                if (AnimatedObject.EndOfAnimation)
                    Fsm.ChangeAction(Fsm_Default);
                break;

            case FsmAction.UnInit:
                ProcessMessage(Message.Destroy);
                break;
        }
    }
}