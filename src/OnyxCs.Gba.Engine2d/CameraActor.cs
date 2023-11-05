namespace OnyxCs.Gba.Engine2d;

public abstract class CameraActor
{
    public BaseActor LinkedObject { get; set; }
    public FiniteStateMachine Fsm { get; } = new();

    protected virtual bool ProcessMessage(Message message, object param)
    {
        return false;
    }

    public abstract void SetFirstPosition();
    public abstract bool IsActorFramed(BaseActor actor);
}