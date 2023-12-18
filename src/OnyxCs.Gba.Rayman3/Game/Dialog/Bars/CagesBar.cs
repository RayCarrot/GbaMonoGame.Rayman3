using BinarySerializer.Onyx.Gba;
using OnyxCs.Gba.AnimEngine;

namespace OnyxCs.Gba.Rayman3;

// TODO: Fully implement
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

        CageIcon.CurrentAnimation = 22;
        CollectedCagesDigit.CurrentAnimation = 0;
        TotalCagesDigit.CurrentAnimation = 0;

        CageIcon.ScreenPos = new Vector2(Engine.ScreenCamera.ScaledGameResolution.X - 44, 41);
        CollectedCagesDigit.ScreenPos = new Vector2(Engine.ScreenCamera.ScaledGameResolution.X - 28, 45);
        TotalCagesDigit.ScreenPos = new Vector2(Engine.ScreenCamera.ScaledGameResolution.X - 10, 45);
    }

    public override void Load()
    {
        TotalCagesDigit.CurrentAnimation = GameInfo.Level.CagesCount;
        CollectedCagesDigit.CurrentAnimation = 0;
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        animationPlayer.AddObject(CageIcon);
        animationPlayer.AddObject(CollectedCagesDigit);
        animationPlayer.AddObject(TotalCagesDigit);
    }
}