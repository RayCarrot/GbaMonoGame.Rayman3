using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public class Cages50Bar : Bar
{
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
            ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 68, 41),
            SpritePriority = 0,
            YPriority = 0,
        };

        CollectedCagesDigit1 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 56, 45),
            SpritePriority = 0,
            YPriority = 0,
        };

        CollectedCagesDigit2 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 44, 45),
            SpritePriority = 0,
            YPriority = 0,
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
        // Keep position updated for different screen resolutions support
        CagesIcon.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 68, CagesIcon.ScreenPos.Y);
        CollectedCagesDigit1.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 56, CollectedCagesDigit1.ScreenPos.Y);
        CollectedCagesDigit2.ScreenPos = new Vector2(Engine.GameViewPort.GameResolution.X - 44, CollectedCagesDigit2.ScreenPos.Y);

        animationPlayer.PlayFront(CagesIcon);

        if (DeadCages < 50)
        {
            if (DeadCages > 9)
                animationPlayer.PlayFront(CollectedCagesDigit1);

            animationPlayer.PlayFront(CollectedCagesDigit2);
        }
    }
}