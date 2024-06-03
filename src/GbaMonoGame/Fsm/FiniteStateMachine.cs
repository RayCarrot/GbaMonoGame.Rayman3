namespace GbaMonoGame;

public class FiniteStateMachine
{
    private Fsm CurrentAction { get; set; }

    public void ChangeAction(Fsm newAction) => ChangeAction(newAction, true);
    public void ChangeAction(Fsm newAction, bool unInit)
    {
        if (unInit)
            CurrentAction?.Invoke(FsmAction.UnInit);
        
        CurrentAction = newAction;
        CurrentAction?.Invoke(FsmAction.Init);
    }

    public void Step()
    {
        CurrentAction?.Invoke(FsmAction.Step);
    }

    public bool EqualsAction(Fsm action)
    {
        return CurrentAction == action;
    }

    public delegate void Fsm(FsmAction action);

    public override string ToString()
    {
        return CurrentAction?.Method.Name ?? "None";
    }
}