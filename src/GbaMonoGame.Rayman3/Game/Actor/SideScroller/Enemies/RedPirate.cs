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

    public uint AttackTimer { get; set; }
    public uint DoubleHitTimer { get; set; }
    public Vector2 KnockBackPosition { get; set; }
    public int Ammo { get; set; } // Ammo functionality appears unused
    public bool HasFiredShot { get; set; }

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