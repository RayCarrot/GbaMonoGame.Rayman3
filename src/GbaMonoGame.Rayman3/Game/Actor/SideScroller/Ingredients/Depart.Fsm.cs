namespace GbaMonoGame.Rayman3;

public partial class Depart
{
    private void Fsm_Idle(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                if (Scene.IsDetectedMainActor(this))
                {
                    Scene.MainActor.ProcessMessage(MessageToSend);
                    Fsm.ChangeAction(null);
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }
}