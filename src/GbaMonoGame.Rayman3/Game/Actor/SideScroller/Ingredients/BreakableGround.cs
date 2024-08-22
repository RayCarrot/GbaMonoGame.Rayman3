﻿using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class BreakableGround : MovableActor
{
    public BreakableGround(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        QuickFinishBodyAttack = (Action)actorResource.FirstActionId == Action.Idle_QuickFinishBodyShotAttack;

        AnimatedObject.YPriority = 60;

        // Destroy actor if the one in the hub world and we've defeated the boss
        if ((Action)actorResource.FirstActionId == Action.Idle_World &&
            (GameInfo.PersistentInfo.LastCompletedLevel > (int)MapId.BossRockAndLava ||
             GameInfo.PersistentInfo.LastPlayedLevel > (int)MapId.BossRockAndLava))
        {
            ProcessMessage(this, Message.Destroy);
        }

        State.SetTo(Fsm_Idle);
    }

    public bool QuickFinishBodyAttack { get; }
    public bool IsDestroyed { get; set; }

    protected override bool ProcessMessageImpl(object sender, Message message, object param)
    {
        if (base.ProcessMessageImpl(sender, message, param))
            return false;

        // Handle messages
        switch (message)
        {
            case Message.Hit:
                if (State == Fsm_Idle && ((RaymanBody)param).BodyPartType == RaymanBody.RaymanBodyPartType.Torso)
                    ActionId = Action.Destroyed;
                return false;

            default:
                return false;
        }
    }

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        if (Engine.Settings.Platform == Platform.NGage && IsDestroyed)
            return;

        DrawLarge(animationPlayer, forceDraw);
    }
}