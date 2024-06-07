using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class PurpleLum : BaseActor
{
    public PurpleLum(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        State.MoveTo(Fsm_Default);
    }
}