using System;
using OnyxCs.Gba.Engine2d;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public class FrameSideScroller : Frame, IHasScene, IHasPlayfield
{
    #region Constructor

    public FrameSideScroller(MapId mapId)
    {
        GameInfo.SetNextMapId(mapId);
    }

    #endregion

    #region Protected Properties

    protected Scene2D Scene { get; set; }
    protected Action CurrentStepAction { get; set; }

    #endregion

    #region Public Properties

    public TransitionsFX TransitionsFX { get; set; }

    public bool CanPause { get; set; }
    public bool IsTimed { get; set; }

    #endregion

    #region Interface Properties

    Scene2D IHasScene.Scene => Scene;
    TgxPlayfield IHasPlayfield.Playfield => Scene.Playfield;

    #endregion

    #region Pubic Override Methods

    public override void Init()
    {
        GameInfo.LoadedYellowLums = 0;
        GameInfo.GreenLums = 0;
        GameInfo.MapId = GameInfo.NextMapId ?? throw new Exception("No map id set");
        // TODO: More setup...
        TransitionsFX = new TransitionsFX();
        BaseActor.ActorDrawPriority = 1;
        Scene = new Scene2D((int)GameInfo.MapId, x => new CameraSideScroller(x), 4);
        // TODO: More setup...
        Scene.AddDialog(new UserInfoSideScroller(GameInfo.Level.HasBlueLum));
        // TODO: More setup...
        Scene.Init();
        // TODO: More setup...
        GameInfo.PlayLevelMusic();
        CurrentStepAction = Step_Normal;
    }
    
    public override void UnInit()
    {
        GameInfo.StopLevelMusic();
    }

    public override void Step()
    {
        CurrentStepAction();

        if (EndOfFrame)
            GameInfo.LoadLevel(GameInfo.GetNextLevelId());
    }

    #endregion

    #region Steps

    private void Step_Normal()
    {
        Scene.Step();
        Scene.AnimationPlayer.Execute();
    }

    #endregion
}