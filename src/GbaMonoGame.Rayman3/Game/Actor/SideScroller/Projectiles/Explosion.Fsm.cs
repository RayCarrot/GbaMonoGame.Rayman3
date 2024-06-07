using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

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
                    State.MoveTo(Fsm_Default);
                break;

            case FsmAction.UnInit:
                AnimatedObject.CurrentAnimation = 0;
                ProcessMessage(this, Message.Destroy);
                break;
        }
    }
}