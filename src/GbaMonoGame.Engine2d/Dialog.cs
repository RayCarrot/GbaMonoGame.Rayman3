using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Engine2d;

public abstract class Dialog : Object
{
    protected FiniteStateMachine Fsm { get; } = new();

    public void Step() => Fsm.Step();
    public abstract void Load();
    public virtual void Init() { }
    public abstract void Draw(AnimationPlayer animationPlayer);
}