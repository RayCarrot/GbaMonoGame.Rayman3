using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class FallingPlatform : MovableActor
{
    public FallingPlatform(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        InitialPosition = Position;
        Timer = 0;
        AnimatedObject.ObjPriority = 60;

        State.SetTo(Fsm_Idle);
    }

    public Vector2 InitialPosition { get; }
    public uint GameTime { get; set; } // TODO: This is how the game respawns the obj if off-screen - we need a different solution
    public byte Timer { get; set; }

    public override void Init(ActorResource actorResource)
    {
        InitWithLink(actorResource);
    }
}