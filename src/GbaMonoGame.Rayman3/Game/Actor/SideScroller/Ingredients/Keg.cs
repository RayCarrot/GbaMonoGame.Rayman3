﻿using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class Keg : MovableActor
{
    public Keg(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        Links = actorResource.Links;
        AnimatedObject.YPriority = 60;
        ShouldDraw = true;

        if ((Action)actorResource.FirstActionId == Action.Fall)
        {
            Fsm.ChangeAction(Fsm_WaitingToFall);
            InitialPos = Position;
            Timer = 0;
        }
        else if (GameInfo.MapId == MapId.BossMachine)
        {
            Fsm.ChangeAction(Fsm_InitBossMachine);
            InitialPos = Position;
            Timer = 30;
        }
        else
        {
            Fsm.ChangeAction(Fsm_Idle);
            InitialPos = Position;
        }
    }

    public byte?[] Links { get; }
    public bool ShouldDraw { get; set; }
    public ushort Timer { get; set; }
    public int SpawnedDebrisCount { get; set; }
    public Vector2 InitialPos { get; set; }

    private void SpawnDebris()
    {
        KegDebris debris = Scene.CreateProjectile<KegDebris>(ActorType.KegDebris);

        if (debris != null)
        {
            debris.Position = Position + new Vector2(Random.GetNumber(33) - 16, 0);
            debris.ActionId = Random.GetNumber(7) / 2; // 0-3
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__BarlLeaf_SkiWeed_Mix02);
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__BarlLeaf_SkiWeed_Mix02);
        }
    }

    private void SpawnExplosion(bool forcePlaySound)
    {
        Explosion explosion = Scene.CreateProjectile<Explosion>(ActorType.Explosion);

        if (forcePlaySound || AnimatedObject.IsFramed)
        {
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__BangGen1_Mix07);
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__BangGen1_Mix07);
        }

        if (explosion != null)
            explosion.Position = Position - new Vector2(0, 8);
    }

    protected override bool ProcessMessageImpl(object sender, Message message, object param)
    {
        if (base.ProcessMessageImpl(sender, message, param))
            return false;

        switch (message)
        {
            case Message.ThrowObjectUp:
                Fsm.ChangeAction(Fsm_ThrownUp);
                return false;

            case Message.ThrowObjectForward:
                Fsm.ChangeAction(Fsm_ThrownForward);
                return false;

            case Message.DropObject:
                if (!Fsm.EqualsAction(FUN_08063fe4))
                    Fsm.ChangeAction(Fsm_Drop);
                return false;

            case Message.Damaged:
                Explosion explosion = Scene.CreateProjectile<Explosion>(ActorType.Explosion);
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__BangGen1_Mix07);
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__BangGen1_Mix07);
                
                if (explosion != null)
                    explosion.Position = Position - new Vector2(0, 8);
                
                Fsm.ChangeAction(Fsm_Respawn);
                return false;

            // TODO: Implement
            //case 1034:
            //case 1035:
            //    return false;

            default:
                return false;
        }
    }

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        if (Fsm.EqualsAction(Fsm_Respawn))
        {
            AnimatedObject.IsFramed = Timer > 180 && 
                                      Scene.Camera.IsActorFramed(this) &&
                                      (GameTime.ElapsedFrames & 1) != 0;
        }
        else
        {
            AnimatedObject.IsFramed = ShouldDraw && 
                                      Scene.Camera.IsActorFramed(this);
        }

        if (AnimatedObject.IsFramed)
            animationPlayer.Play(AnimatedObject);
        else
            AnimatedObject.ComputeNextFrame();
    }
}