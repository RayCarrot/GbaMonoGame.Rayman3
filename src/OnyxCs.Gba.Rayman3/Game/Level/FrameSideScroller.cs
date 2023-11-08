﻿using System;
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
        Scene.Init();
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
        Scene.Step();
        Scene.AnimationPlayer.Execute();
    }

    #endregion
}