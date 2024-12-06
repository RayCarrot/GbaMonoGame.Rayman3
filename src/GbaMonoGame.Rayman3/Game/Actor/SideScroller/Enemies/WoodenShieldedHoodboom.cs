﻿using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

// Original name: CagoulardBouclier
public sealed partial class WoodenShieldedHoodboom : InteractableActor
{
    public WoodenShieldedHoodboom(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene,
        actorResource)
    {
        PrevHitPoints = HitPoints;
        Flags = 0;
        HasShield = true;

        IsObjectCollisionXOnly = true;

        State.SetTo(Fsm_Idle);
    }

    public int PrevHitPoints { get; set; }
    public uint Timer { get; set; }
    public uint InvulnerabilityTimer { get; set; }
    public byte Flags { get; set; } // TODO: Rename and split up into multiple values if possible
    public bool HasShield { get; set; }

    private void StartInvulnerability()
    {
        InvulnerabilityTimer = GameTime.ElapsedFrames;
        IsInvulnerable = true;
    }

    protected override bool ProcessMessageImpl(object sender, Message message, object param)
    {
        if (base.ProcessMessageImpl(sender, message, param))
            return false;

        // Handle messages
        switch (message)
        {
            case Message.Hit:
                if (!IsInvulnerable)
                {
                    // Can't take damage while shield is still there
                    if (HasShield)
                        HitPoints = PrevHitPoints;

                    if (ActionId is Action.ShieldedBreakShield_Right or Action.ShieldedBreakShield_Left)
                        HitPoints = PrevHitPoints;
                    else if (State != Fsm_Hit || (Flags & 0x40) != 0)
                        State.MoveTo(Fsm_Hit);
                }
                return false;

            default:
                return false;
        }
    }

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        DrawWithInvulnerability(animationPlayer, forceDraw);
    }
}