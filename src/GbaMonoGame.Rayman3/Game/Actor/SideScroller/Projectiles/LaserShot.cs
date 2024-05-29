using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class LaserShot : MovableActor
{
    public LaserShot(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        Fsm.ChangeAction(Fsm_Default);
    }
}