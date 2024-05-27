﻿using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class RaymanBody : MovableActor
{
    public RaymanBody(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        Rayman = Scene.MainActor;
        AnimatedObject.YPriority = 18;
        Fsm.ChangeAction(Fsm_Wait);
    }

    public MovableActor Rayman { get; set; }
    public RaymanBodyPartType BodyPartType { get; set; }
    public uint ChargePower { get; set; }
    public bool HasCharged { get; set; }
    public byte BaseActionId { get; set; }
    public InteractableActor HitActor { get; set; }

    private void SpawnHitEffect()
    {
        RaymanBody hitEffectActor = Scene.KnotManager.CreateProjectile<RaymanBody>(ActorType.RaymanBody);
        if (hitEffectActor != null)
        {
            hitEffectActor.BodyPartType = RaymanBodyPartType.HitEffect;
            hitEffectActor.Position = Position;
            hitEffectActor.CheckAgainstMapCollision = false;
            hitEffectActor.ActionId = 25;
            hitEffectActor.AnimatedObject.YPriority = 1;
            hitEffectActor.ChangeAction();
        }
    }

    protected override bool ProcessMessageImpl(object sender, Message message, object param)
    {
        if (base.ProcessMessageImpl(sender, message, param))
            return false;

        // Handle messages
        switch (message)
        {
            case Message.RaymanBody_FinishedAttack:
                if (!Fsm.EqualsAction(Fsm_MoveBackwards))
                {
                    if (BodyPartType == RaymanBodyPartType.Torso)
                        ActionId = IsFacingRight ? 15 : 16;
                    else
                        ActionId = BaseActionId + (IsFacingRight ? 4 : 3);

                    ChangeAction();
                    Fsm.ChangeAction(Fsm_MoveBackwards);
                }
                SpawnHitEffect();
                return false;

            default:
                return false;
        }
    }

    // Game overrides this and calls PlayChannelBox even when not on screen. Makes sense since it should
    // retain its collision. But it seems unnecessary since ComputeNextFrame calls that as well...
    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        if (Scene.Camera.IsActorFramed(this) || forceDraw)
        {
            AnimatedObject.IsFramed = true;
            animationPlayer.Play(AnimatedObject);
        }
        else
        {
            AnimatedObject.IsFramed = false;
            AnimatedObject.PlayChannelBox();
            AnimatedObject.ComputeNextFrame();
        }
    }

    public enum RaymanBodyPartType
    {
        Fist = 0,
        SecondFist = 1,
        Foot = 2,
        Torso = 3,
        HitEffect = 4,
        SuperFist = 5,
        SecondSuperFist = 6,
    }
}