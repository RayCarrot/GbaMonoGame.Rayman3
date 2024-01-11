using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed class Butterfly : BaseActor
{
    public Butterfly(int id, Scene2D scene, ActorResource actorResource) : base(id, scene, actorResource)
    {
        AnimatedObject.CurrentAnimation = actorResource.FirstActionId;
        AnimatedObject.YPriority = 60;
    }
}