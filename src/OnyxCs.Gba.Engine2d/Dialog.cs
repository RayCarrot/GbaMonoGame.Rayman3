using OnyxCs.Gba.AnimEngine;

namespace OnyxCs.Gba.Engine2d;

public abstract class Dialog
{
    public Fsm Fsm { get; set; }

    public abstract void ProcessMessage();
    public abstract void Load();
    public abstract void Init();
    public abstract void Draw(AnimationPlayer animationPlayer);
}