﻿using OnyxCs.Gba.AnimEngine;

namespace OnyxCs.Gba.Rayman3;

public class CagesBar : Bar
{
    public AnimatedObject CageIcon { get; set; }
    public AnimatedObject CollectedCagesDigit { get; set; }
    public AnimatedObject TotalCagesDigit { get; set; }

    public override void Init()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(0x46);
        CageIcon = new AnimatedObject(resource, false);
        CollectedCagesDigit = new AnimatedObject(resource, false);
        TotalCagesDigit = new AnimatedObject(resource, false);

        CageIcon.SetCurrentAnimation(22);
        CollectedCagesDigit.SetCurrentAnimation(0);
        TotalCagesDigit.SetCurrentAnimation(0);

        CageIcon.ScreenPos = new Vector2(196, 41);
        CollectedCagesDigit.ScreenPos = new Vector2(212, 45);
        TotalCagesDigit.ScreenPos = new Vector2(230, 45);
    }

    public override void Load()
    {
        TotalCagesDigit.SetCurrentAnimation(GameInfo.Level.CagesCount);
        CollectedCagesDigit.SetCurrentAnimation(0);
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        animationPlayer.AddObject1(CageIcon);
        animationPlayer.AddObject1(CollectedCagesDigit);
        animationPlayer.AddObject1(TotalCagesDigit);
    }
}