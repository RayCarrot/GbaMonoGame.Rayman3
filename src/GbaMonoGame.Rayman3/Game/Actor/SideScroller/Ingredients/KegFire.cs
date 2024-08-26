using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class KegFire : InteractableActor
{
    public KegFire(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        AnimatedObject.YPriority = 2;

        State.SetTo(Fsm_Default);
    }
}