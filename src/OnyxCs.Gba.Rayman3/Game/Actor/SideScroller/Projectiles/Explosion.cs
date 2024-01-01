using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public sealed partial class Explosion : BaseActor
{
    public Explosion(int id, Scene2D scene, ActorResource actorResource) : base(id, scene, actorResource)
    {
        AnimatedObject.YPriority = 25;
        Fsm.ChangeAction(Fsm_Default);
    }
}