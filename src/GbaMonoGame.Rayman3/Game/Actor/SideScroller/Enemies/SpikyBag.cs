using System;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

// SacPieux
public sealed partial class SpikyBag : InteractableActor
{
    public SpikyBag(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        CurrentSwingAnimation = 0;

        if ((Action)actorResource.FirstActionId == Action.Stationary)
            State.SetTo(Fsm_Stationary);
        else if ((Action)actorResource.FirstActionId == Action.Swing)
            State.SetTo(Fsm_Swing);
        else
            throw new Exception("Invalid initial action id");
    }

    public int CurrentSwingAnimation { get; set; }

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        DrawLarge(animationPlayer, forceDraw);
    }
}