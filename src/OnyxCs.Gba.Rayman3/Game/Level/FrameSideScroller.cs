using System;
using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public abstract class FrameSideScroller : Frame
{
    #region Constructor

    protected FrameSideScroller(MapId mapId)
    {
        GameInfo.SetNextMapId(mapId);
    }

    #endregion

    #region Protected Properties

    protected Scene2D Scene { get; set; }
    protected Action CurrentStepAction { get; set; }

    #endregion

    #region Pubic Override Methods

    public override void Init()
    {
        GameInfo.LoadedYellowLums = 0;
        GameInfo.GreenLums = 0;
        GameInfo.MapId = GameInfo.NextMapId;
        // TODO: More setup...
        BaseActor.ActorDrawPriority = 1;
        Scene = new Scene2D((int)GameInfo.MapId, new CameraSideScroller(), 4);
        // TODO: More setup...
        Scene.AddDialog(new UserInfoSideScroller(GameInfo.Level.HasBlueLum));
        // TODO: More setup...
        CurrentStepAction = Step_Normal;
    }

    public override void Step()
    {
        CurrentStepAction();

        if (EndOfFrame)
        {
            // TODO: Load next level
        }
    }

    #endregion

    #region Steps

    private void Step_Normal()
    {
        Scene.RunActors();
        // TODO: Enable/disable actors
        Scene.StepActors();
        Scene.MoveActors();
        // TODO: Check captors
        Scene.RunCamera();
        Scene.ProcessDialogs();
        Scene.DrawActors();

        Scene.AnimationPlayer.Execute();

        //foreach (BaseActor actor in Scene.Objects.Actors)
        //{
        //    Gfx.AddDebugBox(new DebugBox(nameof(actor.ViewBox), actor.ViewBox, actor.AnimatedObject.ScreenPos, Color.DarkOrange));

        //    if (actor is ActionActor actionActor)
        //    {
        //        Gfx.AddDebugBox(new DebugBox(nameof(actionActor.ActionBox), actionActor.ActionBox, actor.AnimatedObject.ScreenPos, Color.BlueViolet));
        //        Gfx.AddDebugBox(new DebugBox(nameof(actionActor.DetectionBox), actionActor.DetectionBox, actor.AnimatedObject.ScreenPos, Color.Pink));
        //    }
        //    if (actor is InteractableActor interactableActor)
        //    {
        //        Gfx.AddDebugBox(new DebugBox(nameof(interactableActor.AnimationBoxTable.AttackBox), interactableActor.AnimationBoxTable.AttackBox, actor.AnimatedObject.ScreenPos, Color.Red));
        //        Gfx.AddDebugBox(new DebugBox(nameof(interactableActor.AnimationBoxTable.VulnerabilityBox), interactableActor.AnimationBoxTable.VulnerabilityBox, actor.AnimatedObject.ScreenPos, Color.Green));
        //    }
        //}
    }

    #endregion
}