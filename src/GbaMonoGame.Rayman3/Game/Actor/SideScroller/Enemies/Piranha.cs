﻿using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class Piranha : MovableActor
{
    public Piranha(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        InitPos = Position;
        Fsm.ChangeAction(Fsm_Wait);
    }

    private Vector2 InitPos { get; }
    private int Timer { get; set; }
    private bool ShouldDraw { get; set; }

    private void SpawnSplash()
    {
        WaterSplash waterSplash = Scene.KnotManager.CreateProjectile<WaterSplash>(ActorType.WaterSplash);
        if (waterSplash != null)
            waterSplash.Position = Position;
    }

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        if (ShouldDraw)
            base.Draw(animationPlayer, forceDraw);
    }
}