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
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(0x46);
        LumsIcon = new AnimatedObject(resource, false);
        CollectedLumsDigit1 = new AnimatedObject(resource, false);
        CollectedLumsDigit2 = new AnimatedObject(resource, false);
        TotalLumsDigit1 = new AnimatedObject(resource, false);
        TotalLumsDigit2 = new AnimatedObject(resource, false);

        LumsIcon.SetCurrentAnimation(24);
        CollectedLumsDigit1.SetCurrentAnimation(0);
        CollectedLumsDigit2.SetCurrentAnimation(0);
        TotalLumsDigit1.SetCurrentAnimation(0);
        TotalLumsDigit2.SetCurrentAnimation(0);

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
        TotalLumsDigit1.SetCurrentAnimation(GameInfo.Level.LumsCount / 10);
        TotalLumsDigit2.SetCurrentAnimation(GameInfo.Level.LumsCount % 10);

        CollectedLumsDigit1.SetCurrentAnimation(0);
        CollectedLumsDigit2.SetCurrentAnimation(0);

        LumsIcon.SetCurrentAnimation(24);
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        animationPlayer.AddPrimaryObject(LumsIcon);
        animationPlayer.AddPrimaryObject(CollectedLumsDigit2);
        animationPlayer.AddPrimaryObject(TotalLumsDigit1);
        animationPlayer.AddPrimaryObject(TotalLumsDigit2);
    }
}