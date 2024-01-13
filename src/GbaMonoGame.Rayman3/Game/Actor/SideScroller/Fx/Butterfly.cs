using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed class Butterfly : BaseActor
{
    public Butterfly(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        AnimatedObject.CurrentAnimation = actorResource.FirstActionId;
        AnimatedObject.YPriority = 60;
    }
}