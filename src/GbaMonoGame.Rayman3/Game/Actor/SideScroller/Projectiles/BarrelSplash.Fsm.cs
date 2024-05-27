using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class BarrelSplash
{
    private void Fsm_Default(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                AnimatedObject.Rewind();
                break;

            case FsmAction.Step:
                // Do nothing
                break;

            case FsmAction.UnInit:
                ProcessMessage(this, Message.Destroy);
                break;
        }
    }
}