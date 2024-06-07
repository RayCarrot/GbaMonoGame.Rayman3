using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class BarrelSplash : BaseActor
{
    public BarrelSplash(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        AnimatedObject.CurrentAnimation = 0;
        AnimatedObject.YPriority = 31;

        State.SetTo(Fsm_Default);
    }
}