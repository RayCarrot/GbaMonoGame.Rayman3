namespace GbaMonoGame;

public class FiniteStateMachine
{
    private Fsm CurrentState { get; set; }

    /// <summary>
    /// Sets the current state without uninitializing the previous one
    /// </summary>
    /// <param name="state">The new state</param>
    public void SetTo(Fsm state)
    {
        CurrentState = state;
        CurrentState?.Invoke(FsmAction.Init);
    }

    /// <summary>
    /// Moves to a new state
    /// </summary>
    /// <param name="state">The new state</param>
    public void MoveTo(Fsm state)
    {
        CurrentState?.Invoke(FsmAction.UnInit);
        CurrentState = state;
        CurrentState?.Invoke(FsmAction.Init);
    }

    public void Step()
    {
        CurrentState?.Invoke(FsmAction.Step);
    }

    public bool EqualsState(Fsm state)
    {
        return CurrentState == state;
    }

    public delegate void Fsm(FsmAction action);

    public override string ToString()
    {
        return CurrentState?.Method.Name ?? "None";
    }
}