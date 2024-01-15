using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class Explosion : BaseActor
{
    public Explosion(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        AnimatedObject.YPriority = 25;
        Fsm.ChangeAction(Fsm_Default);
    }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        base.ProcessMessageImpl(message, param);
        return false;
    }
}