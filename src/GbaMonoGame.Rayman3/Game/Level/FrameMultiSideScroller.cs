using System;
using GbaMonoGame.Engine2d;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Rayman3;

public class FrameMultiSideScroller : Frame, IHasScene, IHasPlayfield
{
    #region Constructor

    public FrameMultiSideScroller(MapId mapId)
    {
        GameInfo.SetNextMapId(mapId);
        throw new NotImplementedException();
    }

    #endregion

    #region Public Properties

    public Scene2D Scene { get; set; }

    public UserInfoMulti2D UserInfo { get; set; }

    #endregion

    #region Interface Properties

    Scene2D IHasScene.Scene => Scene;
    TgxPlayfield IHasPlayfield.Playfield => Scene.Playfield;

    #endregion

    #region Pubic Methods

    public override void Init()
    {
        throw new NotImplementedException();
    }

    public override void UnInit()
    {
        throw new NotImplementedException();
    }

    public override void Step()
    {
        throw new NotImplementedException();
    }

    #endregion
}