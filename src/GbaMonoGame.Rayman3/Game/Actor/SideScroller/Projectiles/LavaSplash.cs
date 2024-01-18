using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class LavaSplash : MovableActor
{
    public LavaSplash(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        AnimatedObject.YPriority = 5;
        Fsm.ChangeAction(Fsm_Default);
    }

    public void InitMainActorDrown()
    {
        ActionId = Action.MainActorDrownSplash;
        ChangeAction();
    }
}