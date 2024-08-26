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
            State.SetTo(Fsm_WaitingToFall);
            InitialPos = Position;
            Timer = 0;
        }
        else if (GameInfo.MapId == MapId.BossMachine)
        {
            State.SetTo(Fsm_InitBossMachine);
            InitialPos = Position;
            Timer = 30;
        }
        else
        {
            State.SetTo(Fsm_Idle);
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
                State.MoveTo(Fsm_ThrownUp);
                return false;

            case Message.ThrowObjectForward:
                State.MoveTo(Fsm_ThrownForward);
                return false;

            case Message.DropObject:
                if (State != Fsm_Fly)
                    State.MoveTo(Fsm_Drop);
                return false;

            case Message.Damaged:
                Explosion explosion = Scene.CreateProjectile<Explosion>(ActorType.Explosion);
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__BangGen1_Mix07);
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__BangGen1_Mix07);
                
                if (explosion != null)
                    explosion.Position = Position - new Vector2(0, 8);
                
                State.MoveTo(Fsm_Respawn);
                return false;

            case Message.LightOnFire_Right:
            case Message.LightOnFire_Left:
                if (State == Fsm_PickedUp)
                {
                    ActionId = message == Message.LightOnFire_Right ? Action.Ignite_Right : Action.Ignite_Left;
                    Scene.MainActor.ProcessMessage(this, message);
                    State.MoveTo(Fsm_Fly);
                }
                return false;

            default:
                return false;
        }
    }

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        if (State == Fsm_Respawn)
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