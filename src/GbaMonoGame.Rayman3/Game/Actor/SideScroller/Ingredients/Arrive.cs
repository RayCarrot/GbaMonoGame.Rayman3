using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

// Original name: Arrivee
public sealed partial class Arrive : ActionActor
{
    public Arrive(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        LinkedActor = null;

        if ((GameInfo.MapId == MapId.ChallengeLy1 && !GameInfo.PersistentInfo.FinishedLyChallenge1) ||
            (GameInfo.MapId == MapId.ChallengeLy2 && !GameInfo.PersistentInfo.FinishedLyChallenge2))
        {
            LinkedActor = actorResource.Links[0];
            State.SetTo(Fsm_IdleWithLink);
        }
        else if (GameInfo.MapId == MapId.ChallengeLyGCN)
        {
            State.SetTo(Fsm_IdleWithLink);
        }
        else
        {
            State.SetTo(Fsm_Idle);
        }
    }

    public int? LinkedActor { get; }
}