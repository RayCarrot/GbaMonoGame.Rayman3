using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public sealed class Cage : InteractableActor
{
    public Cage(int id, Scene2D scene, ActorResource actorResource) : base(id, scene, actorResource)
    {
        // TODO: Properly implement cage actor

        ActionId = actorResource.FirstActionId == 0 ? 0 : 6;
    }
}