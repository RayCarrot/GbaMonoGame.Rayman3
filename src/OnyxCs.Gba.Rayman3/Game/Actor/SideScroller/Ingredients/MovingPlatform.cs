using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public sealed class MovingPlatform : MovableActor
{
    public MovingPlatform(int id, Scene2D scene, ActorResource actorResource) : base(id, scene, actorResource)
    {
        Resource = actorResource;
        
        AnimatedObject.YPriority = 50;
        XPosMinus48 = Position.X - 48;
        
        Setup();
    }

    private ActorResource Resource { get; }
    private float XPosMinus48 { get; set; }

    private void Setup()
    {
        // TODO: Implement
    }
}