using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class FlowerFire : BaseActor
{
    public FlowerFire(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        AnimatedObject.CurrentAnimation = 0;
        AnimatedObject.YPriority = 15;

        Fsm.ChangeAction(Fsm_Default);
    }

    private byte Timer { get; set; }
    private MovingPlatform Platform { get; set; }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        // Intercept messages
        switch (message)
        {
            case Message.ResurrectWakeUp:
                Timer = 180;
                break;
        }

        if (base.ProcessMessageImpl(message, param))
            return false;

        // Handle messages
        switch (message)
        {
            case Message.FlowerFire_End:
                Fsm.ChangeAction(Fsm_End);
                return false;

            default:
                return false;
        }
    }

    public void AttachPlatform(MovingPlatform platform) => Platform = platform;
}