namespace OnyxCs.Gba.Engine2d;

public abstract class CameraActor : Object
{
    public BaseActor LinkedObject { get; set; }
    public FiniteStateMachine Fsm { get; } = new();

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        return false;
    }

    public abstract void SetFirstPosition();
    public abstract bool IsActorFramed(BaseActor actor);
}