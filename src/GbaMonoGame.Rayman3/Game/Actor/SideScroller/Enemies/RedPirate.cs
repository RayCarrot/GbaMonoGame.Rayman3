using BinarySerializer.Nintendo.GBA;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class RedPirate : PirateBaseActor
{
    public RedPirate(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        SpawnsRedLum = true;
        ReInit();
    }

    private uint AttackTimer { get; set; }
    private uint DoubleHitTimer { get; set; }
    private Vector2 KnockBackPosition { get; set; }
    private int Ammo { get; set; } // Ammo functionality appears unused
    private bool HasFiredShot { get; set; }

    private void Walk()
    {
        if (IsFacingRight)
        {
            if (Scene.GetPhysicalType(Position - new Vector2(0, Constants.TileSize)) == PhysicalTypeValue.Enemy_Left)
                ActionId = Action.Walk_Left;
        }
        else
        {
            if (Scene.GetPhysicalType(Position - new Vector2(0, Constants.TileSize)) == PhysicalTypeValue.Enemy_Right)
                ActionId = Action.Walk_Right;
        }
    }

    private void Shoot()
    {
        Ammo--;
        // TODO: Create projectile type 6
    }

    protected override void ReInit()
    {
        Ammo = 0;
        AttackTimer = 0;
        DoubleHitTimer = 0;
        Fsm.ChangeAction(Fsm_Fall);
    }
}