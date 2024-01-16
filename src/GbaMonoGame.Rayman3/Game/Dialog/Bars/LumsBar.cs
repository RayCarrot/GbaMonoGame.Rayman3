using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public class LumsBar : Bar
{
    private LumsBarState State { get; set; } = LumsBarState.Wait;
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
        State = LumsBarState.MoveIn;
        WaitTimer = 0;

        CollectedLumsDigitValue2 += count;
        
        if (CollectedLumsDigitValue2 >= 10)
        {
            CollectedLumsDigitValue2 -= 10;
            CollectedLumsDigitValue1++;
        }

        LumsIcon.CurrentAnimation = CollectedLumsDigitValue1 == 0 ? 24 : 21;
    }

    public override void Init()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.HudAnimations);

        LumsIcon = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 24,
            ScreenPos = new Vector2(Engine.GameWindow.GameResolution.X - 77, 8),
            SpritePriority = 0,
            YPriority = 0,
        };

        CollectedLumsDigit1 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(Engine.GameWindow.GameResolution.X - 52, 24),
            SpritePriority = 0,
            YPriority = 0,
        };

        CollectedLumsDigit2 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(Engine.GameWindow.GameResolution.X - 40, 24),
            SpritePriority = 0,
            YPriority = 0,
        };

        TotalLumsDigit1 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(Engine.GameWindow.GameResolution.X - 22, 24),
            SpritePriority = 0,
            YPriority = 0,
        };

        TotalLumsDigit2 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(Engine.GameWindow.GameResolution.X - 10, 24),
            SpritePriority = 0,
            YPriority = 0,
        };
    }

    public override void Load()
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
        if (Mode is 1 or 3)
            return;

        switch (State)
        {
            case LumsBarState.Hide:
                YOffset = 35;
                break;
                
            case LumsBarState.MoveIn:
                if (YOffset > 0)
                {
                    YOffset -= 2;
                }
                else
                {
                    State = Mode == 2 ? LumsBarState.Wait : LumsBarState.Bounce;
                    WaitTimer = 0;
                }
                break;

            case LumsBarState.MoveOut:
                if (YOffset < 35)
                {
                    YOffset++;
                }
                else
                {
                    YOffset = 35;
                    State = LumsBarState.Hide;
                }
                break;

            case LumsBarState.Bounce:
                if (WaitTimer < 35)
                {
                    YOffset = -BounceData[WaitTimer];
                    WaitTimer++;
                }
                else
                {
                    YOffset = 0;
                    State = LumsBarState.Wait;
                    WaitTimer = 0;
                }
                break;

            case LumsBarState.Wait:
                if (Mode != 2)
                {
                    if (WaitTimer >= 180)
                    {
                        YOffset = 0;
                        State = LumsBarState.MoveOut;
                    }
                    else
                    {
                        WaitTimer++;
                    }
                }
                break;
        }

        if (State != LumsBarState.Hide)
        {
            LumsIcon.ScreenPos = new Vector2(Engine.GameWindow.GameResolution.X - 77, 8 - YOffset);
            CollectedLumsDigit1.ScreenPos = new Vector2(Engine.GameWindow.GameResolution.X - 52, 24 - YOffset);
            CollectedLumsDigit2.ScreenPos = new Vector2(Engine.GameWindow.GameResolution.X - 40, 24 - YOffset);
            TotalLumsDigit1.ScreenPos = new Vector2(Engine.GameWindow.GameResolution.X - 22, 24 - YOffset);
            TotalLumsDigit2.ScreenPos = new Vector2(Engine.GameWindow.GameResolution.X - 10, 24 - YOffset);

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

    private enum LumsBarState
    {
        Hide,
        MoveIn,
        MoveOut,
        Bounce,
        Wait,
    }
}