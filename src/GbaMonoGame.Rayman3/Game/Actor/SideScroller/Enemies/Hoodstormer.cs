using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

// Original name: CagoulardVolant
public sealed partial class Hoodstormer : MovableActor
{
    public Hoodstormer(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene,
        actorResource)
    {
        State.SetTo(Fsm_Wait);
    }

    private void ShootMissile()
    {
        Missile missile = Scene.CreateProjectile<Missile>(ActorType.Missile);

        if (missile != null)
        {
            if (IsFacingRight)
                missile.Position = Position + new Vector2(58, -8);
            else
                missile.Position = Position + new Vector2(-58, -8);

            missile.ActionId = IsFacingRight ? Missile.Action.DownShot_Right : Missile.Action.DownShot_Left;
            missile.ChangeAction();

            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Laser3_Mix03);
        }
    }
}