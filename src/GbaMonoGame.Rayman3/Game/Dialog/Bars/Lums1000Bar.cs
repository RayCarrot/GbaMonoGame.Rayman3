using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public class Lums1000Bar : Bar
{
    private int DeadLums { get; set; }

    private AnimatedObject LumsIcon { get; set; }
    private AnimatedObject CollectedLumsDigit1 { get; set; }
    private AnimatedObject CollectedLumsDigit2 { get; set; }
    private AnimatedObject CollectedLumsDigit3 { get; set; }

    public void AddLastLums()
    {
        DeadLums = 1000;
        LumsIcon.CurrentAnimation = 35;
        LumsIcon.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 70, LumsIcon.ScreenPos.Y);
    }

    public override void Load()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.HudAnimations);

        LumsIcon = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 88, 8),
            SpritePriority = 0,
            YPriority = 0,
        };

        CollectedLumsDigit1 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 72, 24),
            SpritePriority = 0,
            YPriority = 0,
        };

        CollectedLumsDigit2 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 61, 24),
            SpritePriority = 0,
            YPriority = 0,
        };

        CollectedLumsDigit3 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 50, 24),
            SpritePriority = 0,
            YPriority = 0,
        };
    }

    public override void Set()
    {
        DeadLums = GameInfo.GetTotalCollectedYellowLums();

        if (DeadLums == 1000)
        {
            LumsIcon.CurrentAnimation = 35;
            LumsIcon.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 100, LumsIcon.ScreenPos.Y);
        }
        else
        {
            LumsIcon.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 88, LumsIcon.ScreenPos.Y);
            CollectedLumsDigit1.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 72, CollectedLumsDigit1.ScreenPos.Y);
            CollectedLumsDigit2.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 61, CollectedLumsDigit2.ScreenPos.Y);
            CollectedLumsDigit3.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 50, CollectedLumsDigit3.ScreenPos.Y);

            int digit1 = DeadLums / 100;
            int digit2 = DeadLums % 100 / 10;
            int digit3 = DeadLums % 10;

            CollectedLumsDigit1.CurrentAnimation = digit1;
            CollectedLumsDigit2.CurrentAnimation = digit2;
            CollectedLumsDigit3.CurrentAnimation = digit3;

            if (DeadLums == 999)
            {
                LumsIcon.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 100, LumsIcon.ScreenPos.Y);
                CollectedLumsDigit1.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 86, CollectedLumsDigit1.ScreenPos.Y);
                CollectedLumsDigit2.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 75, CollectedLumsDigit2.ScreenPos.Y);
                CollectedLumsDigit3.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 64, CollectedLumsDigit3.ScreenPos.Y);

                LumsIcon.CurrentAnimation = 36;
            }
            else
            {
                if (digit1 != 0)
                    LumsIcon.CurrentAnimation = 34;
                else if (digit2 != 0)
                    LumsIcon.CurrentAnimation = 32;
                else
                    LumsIcon.CurrentAnimation = 33;

                LumsIcon.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 88, LumsIcon.ScreenPos.Y);
            }
        }
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        // Keep position updated for different screen resolutions support
        if (DeadLums == 1000)
        {
            LumsIcon.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 100, LumsIcon.ScreenPos.Y);
        }
        else if (DeadLums == 999)
        {
            LumsIcon.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 100, LumsIcon.ScreenPos.Y);
            CollectedLumsDigit1.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 86, CollectedLumsDigit1.ScreenPos.Y);
            CollectedLumsDigit2.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 75, CollectedLumsDigit2.ScreenPos.Y);
            CollectedLumsDigit3.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 64, CollectedLumsDigit3.ScreenPos.Y);
        }
        else
        {
            LumsIcon.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 88, LumsIcon.ScreenPos.Y);
            CollectedLumsDigit1.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 72, CollectedLumsDigit1.ScreenPos.Y);
            CollectedLumsDigit2.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 61, CollectedLumsDigit2.ScreenPos.Y);
            CollectedLumsDigit3.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 50, CollectedLumsDigit3.ScreenPos.Y);
        }

        animationPlayer.PlayFront(LumsIcon);

        if (DeadLums < 1000)
        {
            if (DeadLums > 99)
                animationPlayer.PlayFront(CollectedLumsDigit1);

            if (DeadLums > 9)
                animationPlayer.PlayFront(CollectedLumsDigit2);

            animationPlayer.PlayFront(CollectedLumsDigit3);
        }
    }
}