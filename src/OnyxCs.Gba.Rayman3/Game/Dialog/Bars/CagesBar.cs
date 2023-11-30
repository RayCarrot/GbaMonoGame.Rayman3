using BinarySerializer.Onyx.Gba;
using OnyxCs.Gba.AnimEngine;

namespace OnyxCs.Gba.Rayman3;

public class CagesBar : Bar
{
    public AnimatedObject CageIcon { get; set; }
    public AnimatedObject CollectedCagesDigit { get; set; }
    public AnimatedObject TotalCagesDigit { get; set; }

    public override void Init()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.HudAnimations);
        CageIcon = new AnimatedObject(resource, false);
        CollectedCagesDigit = new AnimatedObject(resource, false);
        TotalCagesDigit = new AnimatedObject(resource, false);

        CageIcon.SetCurrentAnimation(22);
        CollectedCagesDigit.SetCurrentAnimation(0);
        TotalCagesDigit.SetCurrentAnimation(0);

        CageIcon.ScreenPos = new Vector2(44, 41);
        CageIcon.Anchor |= ScreenAnchor.Right;
        
        CollectedCagesDigit.ScreenPos = new Vector2(28, 45);
        CollectedCagesDigit.Anchor |= ScreenAnchor.Right;

        TotalCagesDigit.ScreenPos = new Vector2(10, 45);
        TotalCagesDigit.Anchor |= ScreenAnchor.Right;
    }

    public override void Load()
    {
        TotalCagesDigit.SetCurrentAnimation(GameInfo.Level.CagesCount);
        CollectedCagesDigit.SetCurrentAnimation(0);
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        animationPlayer.AddPrimaryObject(CageIcon);
        animationPlayer.AddPrimaryObject(CollectedCagesDigit);
        animationPlayer.AddPrimaryObject(TotalCagesDigit);
    }
}