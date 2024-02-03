using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Engine2d;

public abstract class Dialog : Object
{
    private FiniteStateMachine Fsm { get; } = new();

    public void Step() => Fsm.Step();
    public abstract void Load();
    public abstract void Init();
    public abstract void Draw(AnimationPlayer animationPlayer);
}