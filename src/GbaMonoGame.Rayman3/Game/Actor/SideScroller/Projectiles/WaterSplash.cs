using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class WaterSplash : BaseActor
{
    public WaterSplash(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        AnimatedObject.YPriority = 15;
        Fsm.ChangeAction(Fsm_Default);
    }
}