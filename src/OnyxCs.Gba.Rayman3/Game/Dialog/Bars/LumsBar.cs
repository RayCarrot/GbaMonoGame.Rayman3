using BinarySerializer.Onyx.Gba;
using OnyxCs.Gba.AnimEngine;

namespace OnyxCs.Gba.Rayman3;

public class LumsBar : Bar
{
    public AnimatedObject LumsIcon { get; set; }
    public AnimatedObject CollectedLumsDigit1 { get; set; }
    public AnimatedObject CollectedLumsDigit2 { get; set; }
    public AnimatedObject TotalLumsDigit1 { get; set; }
    public AnimatedObject TotalLumsDigit2 { get; set; }

    public override void Init()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.HudAnimations);
        LumsIcon = new AnimatedObject(resource, false);
        CollectedLumsDigit1 = new AnimatedObject(resource, false);
        CollectedLumsDigit2 = new AnimatedObject(resource, false);
        TotalLumsDigit1 = new AnimatedObject(resource, false);
        TotalLumsDigit2 = new AnimatedObject(resource, false);

        LumsIcon.CurrentAnimation = 24;
            CollectedLumsDigit1.CurrentAnimation = 0;
        CollectedLumsDigit2.CurrentAnimation = 0;
        TotalLumsDigit1.CurrentAnimation = 0;
        TotalLumsDigit2.CurrentAnimation = 0;

        LumsIcon.ScreenPos = new Vector2(77, 8);
        LumsIcon.Anchor |= ScreenAnchor.Right;

        CollectedLumsDigit1.ScreenPos = new Vector2(52, 24);
        CollectedLumsDigit1.Anchor |= ScreenAnchor.Right;

        CollectedLumsDigit2.ScreenPos = new Vector2(40, 24);
        CollectedLumsDigit2.Anchor |= ScreenAnchor.Right;

        TotalLumsDigit1.ScreenPos = new Vector2(22, 24);
        TotalLumsDigit1.Anchor |= ScreenAnchor.Right;

        TotalLumsDigit2.ScreenPos = new Vector2(10, 24);
        TotalLumsDigit2.Anchor |= ScreenAnchor.Right;
    }

    public override void Load()
    {
        TotalLumsDigit1.CurrentAnimation = GameInfo.Level.LumsCount / 10;
        TotalLumsDigit2.CurrentAnimation = GameInfo.Level.LumsCount % 10;

        CollectedLumsDigit1.CurrentAnimation = 0;
        CollectedLumsDigit2.CurrentAnimation = 0;

        LumsIcon.CurrentAnimation = 24;
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        animationPlayer.AddObject(LumsIcon);
        animationPlayer.AddObject(CollectedLumsDigit2);
        animationPlayer.AddObject(TotalLumsDigit1);
        animationPlayer.AddObject(TotalLumsDigit2);
    }
}