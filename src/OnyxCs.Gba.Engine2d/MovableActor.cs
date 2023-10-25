namespace OnyxCs.Gba.Engine2d;

public class MovableActor : InteractableActor
{
    public MovableActor(int id, ActorResource actorResource) : base(id, actorResource)
    {
        ActorMovementState = new ActorMovementState();
        Speed = new Vector2();
    }

    public ActorMovementState ActorMovementState { get; }
    
    public Vector2 Speed { get; set; }

    public override void Step()
    {
        // TODO: Change some flags
        base.Step();
    }
}