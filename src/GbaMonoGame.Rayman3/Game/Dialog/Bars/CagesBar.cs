using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

// TODO: Fully implement
public class CagesBar : Bar
{
    public AnimatedObject CageIcon { get; set; }
    public AnimatedObject CollectedCagesDigit { get; set; }
    public AnimatedObject TotalCagesDigit { get; set; }

    public void AddCages(int count)
    {
        // TODO: Implement
    }

    public override void Init()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.HudAnimations);

        CageIcon = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 22,
            ScreenPos = new Vector2(Engine.GameWindow.GameResolution.X - 44, 41),
            SpritePriority = 0,
            YPriority = 0,
        };

        CollectedCagesDigit = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(Engine.GameWindow.GameResolution.X - 28, 45),
            SpritePriority = 0,
            YPriority = 0,
        };

        TotalCagesDigit = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(Engine.GameWindow.GameResolution.X - 10, 45),
            SpritePriority = 0,
            YPriority = 0,
        };
    }

    public override void Load()
    {
        TotalCagesDigit.CurrentAnimation = GameInfo.Level.CagesCount;
        CollectedCagesDigit.CurrentAnimation = 0;
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        animationPlayer.PlayFront(CageIcon);
        animationPlayer.PlayFront(CollectedCagesDigit);
        animationPlayer.PlayFront(TotalCagesDigit);
    }
}