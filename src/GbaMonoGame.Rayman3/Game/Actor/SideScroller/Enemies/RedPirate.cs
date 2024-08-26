using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class RedPirate : PirateBaseActor
{
    public RedPirate(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        SpawnsRedLum = true;
        ReInit();
    }

    public uint AttackTimer { get; set; }
    public uint DoubleHitTimer { get; set; }
    public Vector2 KnockBackPosition { get; set; }
    public int Ammo { get; set; } // Ammo functionality appears unused
    public bool HasFiredShot { get; set; }

    private void Walk()
    {
        if (IsFacingRight)
        {
            if (Scene.GetPhysicalType(Position - new Vector2(0, Tile.Size)) == PhysicalTypeValue.Enemy_Left)
                ActionId = Action.Walk_Left;
        }
        else
        {
            if (Scene.GetPhysicalType(Position - new Vector2(0, Tile.Size)) == PhysicalTypeValue.Enemy_Right)
                ActionId = Action.Walk_Right;
        }
    }

    private void Shoot()
    {
        Ammo--;

        Missile missile = Scene.CreateProjectile<Missile>(ActorType.Missile);

        if (missile == null) 
            return;
        
        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Laser3_Mix03);

        if (IsFacingRight)
        {
            missile.Position = Position + new Vector2(28, -32);
            missile.ActionId = Missile.Action.Shot1_Right;
        }
        else
        {
            missile.Position = Position + new Vector2(-28, -32);
            missile.ActionId = Missile.Action.Shot1_Left;
        }

        missile.ChangeAction();
    }

    protected override void ReInit()
    {
        Ammo = 0;
        AttackTimer = 0;
        DoubleHitTimer = 0;
        State.SetTo(Fsm_Fall);
    }
}