using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class BouncyPlatform : InteractableActor
{
    public BouncyPlatform(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        HasTrap = actorResource.FirstActionId != 0;
        Fsm.ChangeAction(Fsm_Idle);
    }

    private bool HasTrap { get; }
    private byte Timer { get; set; }
    private bool HasTriggeredBounce { get; set; }
    private Vector2 InitialMainActorSpeed { get; set; }

    // These are added on N-Gage to support bouncy platforms used in multiplayer levels
    private byte MultiplayerCooldown { get; set; }
    private MovableActor TriggeredActor { get; set; }
    private MovableActor[] DetectedActors { get; } = new MovableActor[RSMultiplayer.PlayersCount];
}