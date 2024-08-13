﻿using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public class LumsBar : Bar
{
    public LumsBar(Scene2D scene) : base(scene) { }

    private int WaitTimer { get; set; }
    private int YOffset { get; set; }
    private int CollectedLumsDigitValue1 { get; set; }
    private int CollectedLumsDigitValue2 { get; set; }

    private AnimatedObject LumsIcon { get; set; }
    private AnimatedObject CollectedLumsDigit1 { get; set; }
    private AnimatedObject CollectedLumsDigit2 { get; set; }
    private AnimatedObject TotalLumsDigit1 { get; set; }
    private AnimatedObject TotalLumsDigit2 { get; set; }

    public void AddLums(int count)
    {
        DrawStep = BarDrawStep.MoveIn;
        WaitTimer = 0;

        CollectedLumsDigitValue2 += count;
        
        if (CollectedLumsDigitValue2 >= 10)
        {
            CollectedLumsDigitValue2 -= 10;
            CollectedLumsDigitValue1++;
        }

        LumsIcon.CurrentAnimation = CollectedLumsDigitValue1 == 0 ? 24 : 21;
    }

    public override void Load()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.HudAnimations);

        LumsIcon = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 24,
            ScreenPos = new Vector2(-77, 8),
            HorizontalAnchor = HorizontalAnchorMode.Right,
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
        };

        CollectedLumsDigit1 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(-52, 24),
            HorizontalAnchor = HorizontalAnchorMode.Right,
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
        };

        CollectedLumsDigit2 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(-40, 24),
            HorizontalAnchor = HorizontalAnchorMode.Right,
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
        };

        TotalLumsDigit1 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(-22, 24),
            HorizontalAnchor = HorizontalAnchorMode.Right,
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
        };

        TotalLumsDigit2 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(-10, 24),
            HorizontalAnchor = HorizontalAnchorMode.Right,
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
        };
    }

    public override void Set()
    {
        int lumsCount = GameInfo.LevelType == LevelType.GameCube ? GameInfo.YellowLumsCount : GameInfo.Level.LumsCount;

        TotalLumsDigit1.CurrentAnimation = lumsCount / 10;
        TotalLumsDigit2.CurrentAnimation = lumsCount % 10;

        int collectedLums = GameInfo.GetCollectedYellowLumsInLevel(GameInfo.MapId);

        CollectedLumsDigitValue1 = collectedLums / 10;
        CollectedLumsDigitValue2 = collectedLums % 10;

        LumsIcon.CurrentAnimation = CollectedLumsDigitValue1 == 0 ? 24 : 21;
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        if (Mode is BarMode.StayHidden or BarMode.Disabled)
            return;

        switch (DrawStep)
        {
            case BarDrawStep.Hide:
                YOffset = 35;
                break;
                
            case BarDrawStep.MoveIn:
                if (YOffset > 0)
                {
                    YOffset -= 2;
                }
                else
                {
                    DrawStep = Mode == BarMode.StayVisible ? BarDrawStep.Wait : BarDrawStep.Bounce;
                    WaitTimer = 0;
                }
                break;

            case BarDrawStep.MoveOut:
                if (YOffset < 35)
                {
                    YOffset++;
                }
                else
                {
                    YOffset = 35;
                    DrawStep = BarDrawStep.Hide;
                }
                break;

            case BarDrawStep.Bounce:
                if (WaitTimer < 35)
                {
                    YOffset = -BounceData[WaitTimer];
                    WaitTimer++;
                }
                else
                {
                    YOffset = 0;
                    DrawStep = BarDrawStep.Wait;
                    WaitTimer = 0;
                }
                break;

            case BarDrawStep.Wait:
                if (Mode != BarMode.StayVisible)
                {
                    if (WaitTimer >= 180)
                    {
                        YOffset = 0;
                        DrawStep = BarDrawStep.MoveOut;
                    }
                    else
                    {
                        WaitTimer++;
                    }
                }
                break;
        }

        if (DrawStep != BarDrawStep.Hide)
        {
            LumsIcon.ScreenPos = LumsIcon.ScreenPos with { Y = 8 - YOffset };
            CollectedLumsDigit1.ScreenPos = CollectedLumsDigit1.ScreenPos with { Y = 24 - YOffset };
            CollectedLumsDigit2.ScreenPos = CollectedLumsDigit2.ScreenPos with { Y = 24 - YOffset };
            TotalLumsDigit1.ScreenPos = TotalLumsDigit1.ScreenPos with { Y = 24 - YOffset };
            TotalLumsDigit2.ScreenPos = TotalLumsDigit2.ScreenPos with { Y = 24 - YOffset };

            CollectedLumsDigit1.CurrentAnimation = CollectedLumsDigitValue1;
            CollectedLumsDigit2.CurrentAnimation = CollectedLumsDigitValue2;

            animationPlayer.PlayFront(LumsIcon);

            if (CollectedLumsDigitValue1 != 0)
                animationPlayer.PlayFront(CollectedLumsDigit1);

            animationPlayer.PlayFront(CollectedLumsDigit2);
            animationPlayer.PlayFront(TotalLumsDigit1);
            animationPlayer.PlayFront(TotalLumsDigit2);
        }
    }
}