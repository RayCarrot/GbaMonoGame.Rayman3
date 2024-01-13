using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed class Cage : InteractableActor
{
    public Cage(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        // TODO: Properly implement cage actor

        ActionId = actorResource.FirstActionId == 0 ? 0 : 6;
    }
}