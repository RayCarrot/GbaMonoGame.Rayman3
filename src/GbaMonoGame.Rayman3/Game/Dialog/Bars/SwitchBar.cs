using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public class SwitchBar : Bar
{
    public SwitchBar(Scene2D scene) : base(scene) { }

    public int OffsetY { get; set; } = 35;
    public int ActivatedSwitches { get; set; }
    public int PaletteShiftValue { get; set; }
    public int PaletteShiftDirection { get; set; }

    public AnimatedObject Switches { get; set; }

    public void ActivateSwitch()
    {
        ActivatedSwitches++;
        Switches.CurrentAnimation = ActivatedSwitches;
    }

    public override void Load()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.SwitchBarAnimations);

        Switches = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            CurrentAnimation = ActivatedSwitches,
            ScreenPos = new Vector2(-40, -4),
            HorizontalAnchor = HorizontalAnchorMode.Right,
            VerticalAnchor = VerticalAnchorMode.Bottom,
            SpritePriority = 0,
            YPriority = 0,
            Camera = Scene.HudCamera,
        };

        PaletteShiftValue = 0;
        PaletteShiftDirection = 0;
    }

    public override void Set() { }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        if (((FrameSideScroller)Frame.Current).CurrentStepAction == ((FrameSideScroller)Frame.Current).Step_Normal)
        {
            // TODO: Update palette
        }

        // TODO: Update palette value

        if (Mode is BarMode.StayHidden or BarMode.Disabled)
            return;

        switch (DrawStep)
        {
            case BarDrawStep.Hide:
                OffsetY = 35;
                break;

            case BarDrawStep.MoveIn:
                if (OffsetY > 0)
                    OffsetY -= 2;
                else
                    DrawStep = BarDrawStep.Wait;
                break;

            case BarDrawStep.MoveOut:
                if (OffsetY < 35)
                {
                    OffsetY++;
                }
                else
                {
                    OffsetY = 35;
                    DrawStep = BarDrawStep.Hide;
                }
                break;
        }

        if (DrawStep != BarDrawStep.Hide)
        {
            Switches.ScreenPos = Switches.ScreenPos with { Y = -4 + OffsetY };
            
            animationPlayer.PlayFront(Switches);
        }
    }
}