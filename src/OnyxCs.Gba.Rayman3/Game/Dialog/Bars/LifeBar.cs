﻿using BinarySerializer.Onyx.Gba;
using OnyxCs.Gba.AnimEngine;

namespace OnyxCs.Gba.Rayman3;

public class LifeBar : Bar
{
    public AnimatedObject HitPoints { get; set; }
    public AnimatedObject LifeDigit1 { get; set; }
    public AnimatedObject LifeDigit2 { get; set; }

    public override void Init()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.HudAnimations);
        HitPoints = new AnimatedObject(resource, false);
        LifeDigit1 = new AnimatedObject(resource, false);
        LifeDigit2 = new AnimatedObject(resource, false);

        HitPoints.CurrentAnimation = 15;
        LifeDigit1.CurrentAnimation = 0;
        LifeDigit2.CurrentAnimation = 0;

        HitPoints.ScreenPos = new Vector2(-4, 0);
        LifeDigit1.ScreenPos = new Vector2(49, 20);
        LifeDigit2.ScreenPos = new Vector2(61, 20);
    }

    public override void Load()
    {
        HitPoints.CurrentAnimation = 20;
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        animationPlayer.AddObject(HitPoints);
        animationPlayer.AddObject(LifeDigit1);
        animationPlayer.AddObject(LifeDigit2);
    }
}