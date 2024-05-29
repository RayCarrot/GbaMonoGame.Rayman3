using BinarySerializer.Nintendo.GBA;
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

        LaserShot shot = Scene.CreateProjectile<LaserShot>(ActorType.LaserShot);

        if (shot == null) 
            return;
        
        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Laser3_Mix03);

        if (IsFacingRight)
        {
            shot.Position = Position + new Vector2(28, -32);
            shot.ActionId = LaserShot.Action.Shot1_Right;
        }
        else
        {
            shot.Position = Position + new Vector2(-28, -32);
            shot.ActionId = LaserShot.Action.Shot1_Right;
        }

        shot.ChangeAction();
    }

    protected override void ReInit()
    {
        Ammo = 0;
        AttackTimer = 0;
        DoubleHitTimer = 0;
        Fsm.ChangeAction(Fsm_Fall);
    }
}