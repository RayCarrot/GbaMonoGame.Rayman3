using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public class CagesBar : Bar
{
    public CagesBar(Scene2D scene) : base(scene) { }

    private int WaitTimer { get; set; }
    private int XOffset { get; set; }
    private int CollectedCagesDigitValue { get; set; }

    private AnimatedObject CageIcon { get; set; }
    private AnimatedObject CollectedCagesDigit { get; set; }
    private AnimatedObject TotalCagesDigit { get; set; }

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
            ScreenPos = new Vector2(Scene.HudCamera.Resolution.X - 44, 41),
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
        };

        CollectedCagesDigit = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(Scene.HudCamera.Resolution.X - 28, 45),
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
        };

        TotalCagesDigit = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(Scene.HudCamera.Resolution.X - 10, 45),
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
        if (Mode is 1 or 3)
            return;

        switch (DrawStep)
        {
            case BarDrawStep.Hide:
                XOffset = 65;
                break;

            case BarDrawStep.MoveIn:
                if (XOffset > 0)
                {
                    XOffset -= 3;
                }
                else
                {
                    DrawStep = Mode == 2 ? BarDrawStep.Wait : BarDrawStep.Bounce;
                    WaitTimer = 0;
                }
                break;

            case BarDrawStep.MoveOut:
                if (XOffset < 65)
                {
                    XOffset += 2;
                }
                else
                {
                    XOffset = 65;
                    DrawStep = BarDrawStep.Hide;
                }
                break;

            case BarDrawStep.Bounce:
                if (WaitTimer < 35)
                {
                    XOffset = BounceData[WaitTimer];
                    WaitTimer++;
                }
                else
                {
                    XOffset = 0;
                    DrawStep = BarDrawStep.Wait;
                    WaitTimer = 0;
                }
                break;

            case BarDrawStep.Wait:
                if (Mode != 2)
                {
                    if (WaitTimer >= 180)
                    {
                        XOffset = 0;
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
            CageIcon.ScreenPos = CageIcon.ScreenPos with { X = Scene.HudCamera.Resolution.X - 44 + XOffset };
            CollectedCagesDigit.ScreenPos = CollectedCagesDigit.ScreenPos with { X = Scene.HudCamera.Resolution.X - 28 + XOffset };
            TotalCagesDigit.ScreenPos = TotalCagesDigit.ScreenPos with { X = Scene.HudCamera.Resolution.X - 10 + XOffset };

            CollectedCagesDigit.CurrentAnimation = CollectedCagesDigitValue;

            animationPlayer.PlayFront(CageIcon);
            animationPlayer.PlayFront(CollectedCagesDigit);
            animationPlayer.PlayFront(TotalCagesDigit);
        }
    }
}