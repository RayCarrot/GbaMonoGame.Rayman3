using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class KegDebris : ActionActor
{
    public KegDebris(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        Timer = 0;
        State.SetTo(Fsm_Default);
    }

    public byte Timer { get; set; }
}