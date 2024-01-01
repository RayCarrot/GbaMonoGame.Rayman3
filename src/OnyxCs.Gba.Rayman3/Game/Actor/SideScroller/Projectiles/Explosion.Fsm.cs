using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public partial class Explosion
{
    private void Fsm_Default(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                AnimatedObject.Rewind();
                break;

            case FsmAction.Step:
                if (IsActionFinished)
                    Fsm.ChangeAction(Fsm_Default);
                break;

            case FsmAction.UnInit:
                AnimatedObject.CurrentAnimation = 0;
                ProcessMessage(Message.Destroy);
                break;
        }
    }
}