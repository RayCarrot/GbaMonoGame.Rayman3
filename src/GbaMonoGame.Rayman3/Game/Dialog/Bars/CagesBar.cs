﻿using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public class CagesBar : Bar
{
    public CagesBar(Scene2D scene) : base(scene) { }

    public int WaitTimer { get; set; }
    public int OffsetX { get; set; }
    public int CollectedCagesDigitValue { get; set; }

    public AnimatedObject CageIcon { get; set; }
    public AnimatedObject CollectedCagesDigit { get; set; }
    public AnimatedObject TotalCagesDigit { get; set; }

    public void AddCages(int count)
    {
        DrawStep = BarDrawStep.MoveIn;
        WaitTimer = 0;
        CollectedCagesDigitValue += count;
    }

    public override void Load()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.HudAnimations);

        CageIcon = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 22,
            ScreenPos = new Vector2(-44, 41),
            HorizontalAnchor = HorizontalAnchorMode.Right,
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
        };

        CollectedCagesDigit = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(-28, 45),
            HorizontalAnchor = HorizontalAnchorMode.Right,
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
        };

        TotalCagesDigit = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(-10, 45),
            HorizontalAnchor = HorizontalAnchorMode.Right,
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
        };
    }

    public override void Set()
    {
        int cagesCount = GameInfo.LevelType == LevelType.GameCube ? GameInfo.CagesCount : GameInfo.Level.CagesCount;
        TotalCagesDigit.CurrentAnimation = cagesCount;

        CollectedCagesDigitValue = GameInfo.GetCollectedCagesInLevel(GameInfo.MapId);
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        if (Mode is BarMode.StayHidden or BarMode.Disabled)
            return;

        switch (DrawStep)
        {
            case BarDrawStep.Hide:
                OffsetX = 65;
                break;

            case BarDrawStep.MoveIn:
                if (OffsetX > 0)
                {
                    OffsetX -= 3;
                }
                else
                {
                    DrawStep = Mode == BarMode.StayVisible ? BarDrawStep.Wait : BarDrawStep.Bounce;
                    WaitTimer = 0;
                }
                break;

            case BarDrawStep.MoveOut:
                if (OffsetX < 65)
                {
                    OffsetX += 2;
                }
                else
                {
                    OffsetX = 65;
                    DrawStep = BarDrawStep.Hide;
                }
                break;

            case BarDrawStep.Bounce:
                if (WaitTimer < 35)
                {
                    OffsetX = BounceData[WaitTimer];
                    WaitTimer++;
                }
                else
                {
                    OffsetX = 0;
                    DrawStep = BarDrawStep.Wait;
                    WaitTimer = 0;
                }
                break;

            case BarDrawStep.Wait:
                if (Mode != BarMode.StayVisible)
                {
                    if (WaitTimer >= 180)
                    {
                        OffsetX = 0;
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
            CageIcon.ScreenPos = CageIcon.ScreenPos with { X = -44 + OffsetX };
            CollectedCagesDigit.ScreenPos = CollectedCagesDigit.ScreenPos with { X = -28 + OffsetX };
            TotalCagesDigit.ScreenPos = TotalCagesDigit.ScreenPos with { X = -10 + OffsetX };

            CollectedCagesDigit.CurrentAnimation = CollectedCagesDigitValue;

            animationPlayer.PlayFront(CageIcon);
            animationPlayer.PlayFront(CollectedCagesDigit);
            animationPlayer.PlayFront(TotalCagesDigit);
        }
    }
}