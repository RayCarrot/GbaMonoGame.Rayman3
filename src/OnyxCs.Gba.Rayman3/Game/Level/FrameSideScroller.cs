using System;
using OnyxCs.Gba.Engine2d;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public abstract class FrameSideScroller : Frame, IHasScene, IHasPlayfield
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

    #region Interface Properties

    Scene2D IHasScene.Scene => Scene;
    TgxPlayfield IHasPlayfield.Playfield => Scene.Playfield;

    #endregion

    #region Pubic Override Methods

    public override void Init()
    {
        GameInfo.LoadedYellowLums = 0;
        GameInfo.GreenLums = 0;
        GameInfo.MapId = GameInfo.NextMapId;
        // TODO: More setup...
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
        {
            // TODO: Load next level
        }
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