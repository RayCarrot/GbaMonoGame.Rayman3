﻿using System.Reflection;

namespace GbaMonoGame;

#pragma warning disable CS0660, CS0661
public class FiniteStateMachine
#pragma warning restore CS0660, CS0661
{
    private Fsm CurrentState { get; set; }

    public MethodInfo GetCurrentStateMethodInfo() => CurrentState?.Method;

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

    public static bool operator ==(FiniteStateMachine stateMachine, Fsm state)
    {
        if (stateMachine is null)
            return false;

        return stateMachine.CurrentState == state;
    }

    public static bool operator !=(FiniteStateMachine stateMachine, Fsm state)
    {
        return !(stateMachine == state);
    }

    public delegate bool Fsm(FsmAction action);

    public override string ToString()
    {
        return CurrentState?.Method.Name ?? "None";
    }
}