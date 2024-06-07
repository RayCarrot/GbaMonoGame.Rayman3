using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class Explosion : BaseActor
{
    public Explosion(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        AnimatedObject.YPriority = 25;
        State.MoveTo(Fsm_Default);
    }

    protected override bool ProcessMessageImpl(object sender, Message message, object param)
    {
        base.ProcessMessageImpl(sender, message, param);
        return false;
    }
}