namespace OnyxCs.Gba;

public class FiniteStateMachine
{
    private Fsm CurrentAction { get; set; }

    public void ChangeAction(Fsm newAction)
    {
        CurrentAction?.Invoke(FsmAction.UnInit);
        CurrentAction = newAction;
        CurrentAction(FsmAction.Init);
    }

    public void Step()
    {
        CurrentAction?.Invoke(FsmAction.Step);
    }

    public delegate void Fsm(FsmAction action);
}