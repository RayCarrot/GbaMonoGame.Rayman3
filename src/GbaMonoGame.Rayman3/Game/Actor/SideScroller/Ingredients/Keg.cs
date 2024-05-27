using System;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
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
            throw new NotImplementedException();
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public byte?[] Links { get; }
    public bool ShouldDraw { get; set; }
    public ushort Timer { get; set; }
    public int SpawnedDebrisCount { get; set; }
    public Vector2 InitialPos { get; set; }

    private void SpawnDebris()
    {
        KegDebris debris = Scene.KnotManager.CreateProjectile<KegDebris>(ActorType.KegDebris);

        if (debris != null)
        {
            debris.Position = Position + new Vector2(Random.GetNumber(33) - 16, 0);
            debris.ActionId = Random.GetNumber(7) / 2; // 0-3
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__BarlLeaf_SkiWeed_Mix02);
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__BarlLeaf_SkiWeed_Mix02);
        }
    }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        // TODO: Implement
        return base.ProcessMessageImpl(message, param);
    }

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        if (Fsm.EqualsAction(FUN_08063bd4))
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