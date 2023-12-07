using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public sealed partial class Depart : ActionActor
{
    public Depart(int id, Scene2D scene, ActorResource actorResource) : base(id, scene, actorResource)
    {
        // The behavior to end a level with the sign facing to the right seems unused in the game
        MessageToSend = actorResource.FirstActionId == 1 ? Message.LevelEnd : Message.LevelExit;
        Fsm.ChangeAction(Fsm_Idle);
    }

    private Message MessageToSend { get; }
}