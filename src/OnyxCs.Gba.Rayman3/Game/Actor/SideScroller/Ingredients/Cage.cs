using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public class Cage : InteractableActor
{
    public Cage(int id, ActorResource actorResource) : base(id, actorResource)
    {
        // TODO: Properly implement cage actor

        SetActionId(actorResource.FirstActionId == 0 ? 0 : 6);
    }
}