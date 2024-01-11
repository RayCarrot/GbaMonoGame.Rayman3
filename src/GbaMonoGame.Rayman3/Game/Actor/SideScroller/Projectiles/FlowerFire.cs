using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class FlowerFire : BaseActor
{
    public FlowerFire(int id, Scene2D scene, ActorResource actorResource) : base(id, scene, actorResource)
    {
        AnimatedObject.CurrentAnimation = 0;
        AnimatedObject.YPriority = 15;

        Fsm.ChangeAction(Fsm_Default);
    }

    private byte Timer { get; set; }
    private MovingPlatform Platform { get; set; }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        if (message == Message.ResurrectWakeUp)
            Timer = 180;

        if (base.ProcessMessageImpl(message, param))
            return false;

        if (message == Message.FlowerFire_End)
            Fsm.ChangeAction(Fsm_End);

        return false;
    }

    public void AttachPlatform(MovingPlatform platform) => Platform = platform;
}