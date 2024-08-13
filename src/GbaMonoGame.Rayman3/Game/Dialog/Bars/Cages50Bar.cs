﻿using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public class Cages50Bar : Bar
{
    public Cages50Bar(Scene2D scene) : base(scene) { }

    private int DeadCages { get; set; }

    private AnimatedObject CagesIcon { get; set; }
    private AnimatedObject CollectedCagesDigit1 { get; set; }
    private AnimatedObject CollectedCagesDigit2 { get; set; }

    public override void Load()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.HudAnimations);

        CagesIcon = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            ScreenPos = new Vector2(-68, 41),
            HorizontalAnchor = HorizontalAnchorMode.Right,
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
        };

        CollectedCagesDigit1 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            ScreenPos = new Vector2(-56, 45),
            HorizontalAnchor = HorizontalAnchorMode.Right,
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
        };

        CollectedCagesDigit2 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            ScreenPos = new Vector2(-44, 45),
            HorizontalAnchor = HorizontalAnchorMode.Right,
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
        };
    }

    public override void Set()
    {
        DeadCages = GameInfo.GetTotalCollectedCages();

        if (DeadCages == 50)
        {
            CagesIcon.CurrentAnimation = 38;
        }
        else
        {
            CollectedCagesDigit1.CurrentAnimation = DeadCages / 10;
            CollectedCagesDigit2.CurrentAnimation = DeadCages % 10;
            CagesIcon.CurrentAnimation = DeadCages / 10 == 0 ? 37 : 39;
        }
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        animationPlayer.PlayFront(CagesIcon);

        if (DeadCages < 50)
        {
            if (DeadCages > 9)
                animationPlayer.PlayFront(CollectedCagesDigit1);

            animationPlayer.PlayFront(CollectedCagesDigit2);
        }
    }
}