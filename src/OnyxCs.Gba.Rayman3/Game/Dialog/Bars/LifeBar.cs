using OnyxCs.Gba.AnimEngine;

namespace OnyxCs.Gba.Rayman3;

public class LifeBar : Bar
{
    public AnimatedObject HitPoints { get; set; }
    public AnimatedObject LifeDigit1 { get; set; }
    public AnimatedObject LifeDigit2 { get; set; }

    public override void Init()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(0x46);
        HitPoints = new AnimatedObject(resource, false);
        LifeDigit1 = new AnimatedObject(resource, false);
        LifeDigit2 = new AnimatedObject(resource, false);

        HitPoints.SetCurrentAnimation(15);
        LifeDigit1.SetCurrentAnimation(0);
        LifeDigit2.SetCurrentAnimation(0);

        HitPoints.ScreenPos = new Vector2(-4, 0);
        LifeDigit1.ScreenPos = new Vector2(49, 20);
        LifeDigit2.ScreenPos = new Vector2(61, 20);
    }

    public override void Load()
    {
        HitPoints.SetCurrentAnimation(20);
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        animationPlayer.AddObject1(HitPoints);
        animationPlayer.AddObject1(LifeDigit1);
        animationPlayer.AddObject1(LifeDigit2);
    }
}