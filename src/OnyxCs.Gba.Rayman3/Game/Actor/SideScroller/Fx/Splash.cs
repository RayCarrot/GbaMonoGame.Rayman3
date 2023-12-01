using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public sealed partial class Splash : BaseActor
{
    public Splash(int id, Scene2D scene, ActorResource actorResource) : base(id, scene, actorResource)
    {
        AnimatedObject.YPriority = 15;
        Fsm.ChangeAction(Fsm_Default);
    }
}