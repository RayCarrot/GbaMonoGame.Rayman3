using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public sealed class Butterfly : BaseActor
{
    public Butterfly(int id, ActorResource actorResource) : base(id, actorResource)
    {
        AnimatedObject.SetCurrentAnimation(actorResource.FirstActionId);
    }
}