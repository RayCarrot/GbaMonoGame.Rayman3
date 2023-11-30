using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public sealed class Butterfly : BaseActor
{
    public Butterfly(int id, Scene2D scene, ActorResource actorResource) : base(id, scene, actorResource)
    {
        AnimatedObject.CurrentAnimation = actorResource.FirstActionId;
    }
}