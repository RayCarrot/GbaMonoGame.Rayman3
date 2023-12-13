namespace OnyxCs.Gba.Engine2d;

public abstract class CameraActor : Object
{
    protected CameraActor(Scene2D scene)
    {
        Scene = scene;
    }

    protected FiniteStateMachine Fsm { get; } = new();

    public Scene2D Scene { get; }
    public MovableActor LinkedObject { get; set; }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        return false;
    }

    public virtual void Step()
    {
        Fsm.Step();
    }

    public abstract void SetFirstPosition();
    public abstract bool IsActorFramed(BaseActor actor);
}