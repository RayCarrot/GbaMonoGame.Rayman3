using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

// TODO: Fully implement
public class LifeBar : Bar
{
    public AnimatedObject HitPoints { get; set; }
    public AnimatedObject LifeDigit1 { get; set; }
    public AnimatedObject LifeDigit2 { get; set; }

    public override void Init()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.HudAnimations);
        
        HitPoints = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 15,
            ScreenPos = new Vector2(-4, 0),
            SpritePriority = 0,
            YPriority = 0,
        };
        
        LifeDigit1 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(49, 20),
            SpritePriority = 0,
            YPriority = 0,
        };
        
        LifeDigit2 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(61, 20),
            SpritePriority = 0,
            YPriority = 0,
        };
    }

    public override void Load()
    {
        HitPoints.CurrentAnimation = 20;
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        animationPlayer.PlayFront(HitPoints);
        animationPlayer.PlayFront(LifeDigit1);
        animationPlayer.PlayFront(LifeDigit2);
    }
}