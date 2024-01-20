﻿using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public class LifeBar : Bar
{
    public LifeBar(Scene2D scene)
    {
        Scene = scene;
    }

    private Scene2D Scene { get; }

    private int WaitTimer { get; set; }
    private int YOffset { get; set; }
    private int PreviousLivesCount { get; set; }
    private int PreviousHitPoints { get; set; }
    private bool HitPointsChanged { get; set; }

    private AnimatedObject HitPoints { get; set; }
    private AnimatedObject LifeDigit1 { get; set; }
    private AnimatedObject LifeDigit2 { get; set; }

    public override void Load()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.HudAnimations);
        
        HitPoints = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 15,
            ScreenPos = new Vector2(-4, 0),
            SpritePriority = 0,
            YPriority = 0,
        };
        
        LifeDigit1 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(49, 20),
            SpritePriority = 0,
            YPriority = 0,
        };
        
        LifeDigit2 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(61, 20),
            SpritePriority = 0,
            YPriority = 0,
        };
    }

    public override void Set()
    {
        LifeDigit1.CurrentAnimation = GameInfo.PersistentInfo.Lives / 10;
        LifeDigit2.CurrentAnimation = GameInfo.PersistentInfo.Lives % 10;
        HitPoints.CurrentAnimation = 15 + Scene.MainActor.HitPoints;
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        if (Mode is 1 or 3)
            return;

        int hp = Scene.MainActor.HitPoints;

        // Check if lives count has changed
        if (PreviousLivesCount != GameInfo.PersistentInfo.Lives)
        {
            PreviousLivesCount = GameInfo.PersistentInfo.Lives;

            LifeDigit1.CurrentAnimation = GameInfo.PersistentInfo.Lives / 10;
            LifeDigit2.CurrentAnimation = GameInfo.PersistentInfo.Lives % 10;

            State = BarState.MoveIn;
            WaitTimer = 0;
        }

        // Check if dead
        if (hp == 0 && State == BarState.Wait)
        {
            HitPoints.CurrentAnimation = 10;
         
            State = BarState.Wait;
            WaitTimer = 0;
        }
        // TODO: Have option not to play this sound because it's annoying
        // Check if close to dead
        else if (hp == 1 && (GameTime.ElapsedFrames & 0x3f) == 0x3f)
        {
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__MinHP);
        }

        // Check if hp has changed
        if (PreviousHitPoints != hp)
        {
            PreviousHitPoints = hp;

            HitPoints.CurrentAnimation = 10 + Scene.MainActor.HitPoints;

            HitPointsChanged = true;
            State = BarState.MoveIn;
            WaitTimer = 0;
        }
        else if (HitPointsChanged && HitPoints.EndOfAnimation)
        {
            if (HitPoints.CurrentAnimation < 16)
                HitPoints.CurrentAnimation += 5;

            HitPointsChanged = false;
        }

        switch (State)
        {
            case BarState.Hide:
                YOffset = 36;
                break;

            case BarState.MoveIn:
                if (YOffset != 0)
                {
                    YOffset -= 2;
                }
                else
                {
                    State = BarState.Wait;
                    WaitTimer = 0;
                }
                break;

            case BarState.MoveOut:
                if (YOffset < 36)
                {
                    YOffset += 2;
                }
                else
                {
                    YOffset = 36;
                    State = BarState.Hide;
                }
                break;

            case BarState.Wait:
                if (Mode != 2)
                {
                    if (WaitTimer >= 360)
                    {
                        YOffset = 0;
                        State = BarState.MoveOut;
                    }
                    else
                    {
                        WaitTimer++;
                    }
                }
                break;
        }

        if (State != BarState.Hide)
        {
            HitPoints.ScreenPos = new Vector2(HitPoints.ScreenPos.X, 0 - YOffset);
            LifeDigit1.ScreenPos = new Vector2(LifeDigit1.ScreenPos.X, 20 - YOffset);
            LifeDigit2.ScreenPos = new Vector2(LifeDigit2.ScreenPos.X, 20 - YOffset);

            animationPlayer.PlayFront(HitPoints);
            animationPlayer.PlayFront(LifeDigit1);
            animationPlayer.PlayFront(LifeDigit2);
        }
    }
}