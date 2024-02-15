using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class MurfyStone : BaseActor
{
    public MurfyStone(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        MurfyId = actorResource.Links[0];
        AnimatedObject.YPriority = 63;
        Timer = 181;
        Fsm.ChangeAction(Fsm_Default);
    }

    private int? MurfyId { get; }
    private uint Timer { get; set; }
    private byte RaymanIdleTimer { get; set; }
    private bool HasTriggered { get; set; }
}