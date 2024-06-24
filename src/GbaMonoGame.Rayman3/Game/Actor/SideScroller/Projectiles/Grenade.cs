using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class Grenade : MovableActor
{
    public Grenade(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        State.SetTo(Fsm_Default);
    }

    public byte TouchingMapTimer { get; set; }
}