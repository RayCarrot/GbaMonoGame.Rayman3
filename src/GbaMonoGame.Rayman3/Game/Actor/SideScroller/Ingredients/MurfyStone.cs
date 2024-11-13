using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class MurfyStone : BaseActor
{
    public MurfyStone(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        MurfyId = actorResource.Links[0];
        AnimatedObject.ObjPriority = 63;
        Timer = 181;
        State.SetTo(Fsm_Default);
    }

    public int? MurfyId { get; }
    public uint Timer { get; set; }
    public byte RaymanIdleTimer { get; set; }
    public bool HasTriggered { get; set; }
}