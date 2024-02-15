using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class Arrive : ActionActor
{
    public Arrive(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        LinkedActor = null;

        if ((GameInfo.MapId == MapId.ChallengeLy1 && !GameInfo.PersistentInfo.FinishedLyChallenge1) ||
            (GameInfo.MapId == MapId.ChallengeLy2 && !GameInfo.PersistentInfo.FinishedLyChallenge2))
        {
            LinkedActor = actorResource.Links[0];
            Fsm.ChangeAction(Fsm_IdleWithLink);
        }
        else if (GameInfo.MapId == MapId.ChallengeLyGCN)
        {
            Fsm.ChangeAction(Fsm_IdleWithLink);
        }
        else
        {
            Fsm.ChangeAction(Fsm_Idle);
        }
    }

    private int? LinkedActor { get; }
}