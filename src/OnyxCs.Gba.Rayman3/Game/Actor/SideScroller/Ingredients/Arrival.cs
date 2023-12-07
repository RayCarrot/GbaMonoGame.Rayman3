using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public sealed partial class Arrival : ActionActor
{
    public Arrival(int id, Scene2D scene, ActorResource actorResource) : base(id, scene, actorResource)
    {
        LinkedActor = -1;

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

    private int LinkedActor { get; }
}