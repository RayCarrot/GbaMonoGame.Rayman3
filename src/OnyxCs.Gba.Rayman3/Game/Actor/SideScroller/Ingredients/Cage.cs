using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public sealed class Cage : InteractableActor
{
    public Cage(int id, ActorResource actorResource) : base(id, actorResource)
    {
        // TODO: Properly implement cage actor

        ActionId = actorResource.FirstActionId == 0 ? 0 : 6;
    }
}