using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class Cage : InteractableActor
{
    public Cage(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        InitialActionId = actorResource.FirstActionId == 0 ? 0 : 6;
        PrevHitPoints = HitPoints;
        
        CageId = GameInfo.LoadedCages;
        GameInfo.LoadedCages++;

        Fsm.ChangeAction(Fsm_Idle);

        if (GameInfo.HasCollectedCage(CageId, GameInfo.MapId))
            ProcessMessage(Message.Destroy);
    }

    public int CageId { get; }
    public int InitialActionId { get; }

    public int PrevHitPoints { get; set; }
    public int Timer { get; set; }
    public int HitAction { get; set; }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        if (base.ProcessMessageImpl(message, param))
            return false;

        // TODO: Handle messages 1043 and 1025
        return false;
    }
}