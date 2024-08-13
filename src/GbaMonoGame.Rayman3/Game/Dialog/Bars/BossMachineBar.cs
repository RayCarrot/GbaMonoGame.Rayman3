using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public class BossMachineBar : Bar
{
    public BossMachineBar(Scene2D scene) : base(scene) { }

    private AnimatedObject BossHealthBar { get; set; }
    private int BossDamage { get; set; }

    public override void Load()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.BossMachineBarAnimations);

        BossHealthBar = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = BossDamage,
            ScreenPos = new Vector2(-60, 24),
            HorizontalAnchor = HorizontalAnchorMode.Right,
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
        };
    }

    public override void Set()
    {
        BossDamage++;
        BossHealthBar.CurrentAnimation = BossDamage;
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        if (Mode == 1) 
            return;
        
        animationPlayer.PlayFront(BossHealthBar);
    }
}