using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Engine2d;

public abstract class Dialog
{
    public FiniteStateMachine Fsm { get; } = new();

    public abstract void ProcessMessage();
    public abstract void Load();
    public abstract void Init();
    public abstract void Draw(AnimationPlayer animationPlayer);
}