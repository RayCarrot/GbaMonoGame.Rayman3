using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public class LifeBar : Bar
{
    public LifeBar(Scene2D scene) : base(scene) { }

    private int WaitTimer { get; set; }
    private int YOffset { get; set; }
    private int PreviousLivesCount { get; set; }
    private int PreviousHitPoints { get; set; }
    private bool HitPointsChanged { get; set; }

    private AnimatedObject HitPoints { get; set; }
    private AnimatedObject LifeDigit1 { get; set; }
    private AnimatedObject LifeDigit2 { get; set; }

    public void UpdateLife()
    {
        if (DrawStep != BarDrawStep.Bounce && Mode != BarMode.StayHidden)
            DrawStep = BarDrawStep.MoveIn;
    }

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
            Camera = Scene.HudCamera,
        };
        
        LifeDigit1 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(49, 20),
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
        };
        
        LifeDigit2 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(61, 20),
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
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
        if (Mode is BarMode.StayHidden or BarMode.Disabled)
            return;

        int hp = Scene.MainActor.HitPoints;

        // Check if lives count has changed
        if (PreviousLivesCount != GameInfo.PersistentInfo.Lives)
        {
            PreviousLivesCount = GameInfo.PersistentInfo.Lives;

            LifeDigit1.CurrentAnimation = GameInfo.PersistentInfo.Lives / 10;
            LifeDigit2.CurrentAnimation = GameInfo.PersistentInfo.Lives % 10;

            DrawStep = BarDrawStep.MoveIn;
            WaitTimer = 0;
        }

        // Check if dead
        if (hp == 0 && DrawStep == BarDrawStep.Wait)
        {
            HitPoints.CurrentAnimation = 10;
         
            DrawStep = BarDrawStep.Wait;
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
            DrawStep = BarDrawStep.MoveIn;
            WaitTimer = 0;
        }
        else if (HitPointsChanged && HitPoints.EndOfAnimation)
        {
            if (HitPoints.CurrentAnimation < 16)
                HitPoints.CurrentAnimation += 5;

            HitPointsChanged = false;
        }

        switch (DrawStep)
        {
            case BarDrawStep.Hide:
                YOffset = 36;
                break;

            case BarDrawStep.MoveIn:
                if (YOffset != 0)
                {
                    YOffset -= 2;
                }
                else
                {
                    DrawStep = BarDrawStep.Wait;
                    WaitTimer = 0;
                }
                break;

            case BarDrawStep.MoveOut:
                if (YOffset < 36)
                {
                    YOffset += 2;
                }
                else
                {
                    YOffset = 36;
                    DrawStep = BarDrawStep.Hide;
                }
                break;

            case BarDrawStep.Wait:
                if (Mode != BarMode.StayVisible)
                {
                    if (WaitTimer >= 360)
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
            HitPoints.ScreenPos = HitPoints.ScreenPos with { Y = 0 - YOffset };
            LifeDigit1.ScreenPos = LifeDigit1.ScreenPos with { Y = 20 - YOffset };
            LifeDigit2.ScreenPos = LifeDigit2.ScreenPos with { Y = 20 - YOffset };

            animationPlayer.PlayFront(HitPoints);
            animationPlayer.PlayFront(LifeDigit1);
            animationPlayer.PlayFront(LifeDigit2);
        }
    }
}