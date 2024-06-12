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
            ScreenPos = new Vector2(Scene.HudCamera.Resolution.X - 60, 24),
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
        
        BossHealthBar.ScreenPos = new Vector2(Scene.HudCamera.Resolution.X - 60, 24);
        animationPlayer.PlayFront(BossHealthBar);
    }
}