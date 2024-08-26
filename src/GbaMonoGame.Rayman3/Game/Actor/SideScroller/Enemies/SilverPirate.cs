﻿using BinarySerializer.Ubisoft.GbaEngine.Rayman3;

namespace GbaMonoGame.Rayman3;

public sealed partial class SilverPirate : PirateBaseActor
{
    public SilverPirate(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        SpawnsRedLum = ActionId is Action.Init_HasRedLum_Left or Action.Init_HasRedLum_Right;
        ReInit();
    }

    public uint AttackTimer { get; set; }
    public uint DoubleHitTimer { get; set; }
    public float KnockBackYPosition { get; set; }
    public bool HighShot { get; set; }

    private void Shoot()
    {
        Missile missile = Scene.CreateProjectile<Missile>(ActorType.Missile);

        if (missile == null)
            return;

        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Laser1_Mix02);

        float yOffset = HighShot ? -27 : -9;

        if (IsFacingRight)
        {
            missile.Position = Position + new Vector2(36, yOffset);
            missile.ActionId = Missile.Action.Shot2_Right;
        }
        else
        {
            missile.Position = Position + new Vector2(-36, yOffset);
            missile.ActionId = Missile.Action.Shot2_Left;
        }
    }

    protected override void ReInit()
    {
        DoubleHitTimer = 0;
        HighShot = false;
        AttackTimer = 0;
        State.SetTo(Fsm_Fall);
    }
}