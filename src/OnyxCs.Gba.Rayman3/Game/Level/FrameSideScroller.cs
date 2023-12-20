using System;
using BinarySerializer.Onyx.Gba;
using BinarySerializer.Onyx.Gba.Rayman3;
using OnyxCs.Gba.Engine2d;
using OnyxCs.Gba.TgxEngine;
using Action = System.Action;

namespace OnyxCs.Gba.Rayman3;

public class FrameSideScroller : Frame, IHasScene, IHasPlayfield
{
    #region Constructor

    public FrameSideScroller(MapId mapId)
    {
        GameInfo.SetNextMapId(mapId);
    }

    #endregion

    #region Private Properties

    private GfxScreen CircleFXScreen { get; set; }
    private CircleFXScreenRenderer CircleFXRenderer { get; set; }
    private int CircleFXTimer { get; set; }
    private CircleFXTransitionMode CircleFXMode { get; set; }

    #endregion

    #region Protected Properties

    protected Scene2D Scene { get; set; }
    protected Action CurrentStepAction { get; set; }

    #endregion

    #region Public Properties

    public TransitionsFX TransitionsFX { get; set; }
    public UserInfoSideScroller UserInfo { get; set; }

    public bool CanPause { get; set; }
    public bool IsTimed { get; set; }

    #endregion

    #region Interface Properties

    Scene2D IHasScene.Scene => Scene;
    TgxPlayfield IHasPlayfield.Playfield => Scene.Playfield;

    #endregion

    #region Private Methods

    private void StepCircleFX()
    {
        switch (CircleFXMode)
        {
            case CircleFXTransitionMode.FinishedIn:
                CircleFXMode = CircleFXTransitionMode.None;
                break;

            case CircleFXTransitionMode.In:
                CircleFXTimer += 6;
                if (CircleFXTimer > 252)
                {
                    CircleFXTimer = 252;
                    CircleFXMode = CircleFXTransitionMode.FinishedIn;
                }
                CircleFXRenderer.Radius = CircleFXTimer;
                break;

            case CircleFXTransitionMode.Out:
                CircleFXTimer -= 6;
                if (CircleFXTimer < 0)
                {
                    CircleFXTimer = 0;
                    CircleFXMode = CircleFXTransitionMode.FinishedOut;
                }
                CircleFXRenderer.Radius = CircleFXTimer;
                break;
        }

        CircleFXScreen.IsEnabled = CircleFXMode != CircleFXTransitionMode.None;
    }

    #endregion

    #region Pubic Methods

    public void InitNewCircleFXTransition(bool transitionIn)
    {
        if (transitionIn)
        {
            CircleFXTimer = 0;
            CircleFXMode = CircleFXTransitionMode.In;
            SoundManager.Play(Rayman3SoundEvent.Play__SlideIn_Mix02);
        }
        else
        {
            CircleFXTimer = 252;
            CircleFXMode = CircleFXTransitionMode.Out;
            SoundManager.Play(Rayman3SoundEvent.Play__SlideOut_Mix01);
        }

        CircleFXScreen.IsEnabled = true;
        CircleFXRenderer.Radius = CircleFXTimer;
        CircleFXRenderer.CirclePosition = Scene.MainActor.ScreenPosition - new Vector2(0, 32);
        Gfx.Fade = 0; // We might have a leftover fade making the screen black, but now we show the circle FX screen instead
    }

    public override void Init()
    {
        GameInfo.LoadedYellowLums = 0;
        GameInfo.GreenLums = 0;
        GameInfo.MapId = GameInfo.NextMapId ?? throw new Exception("No map id set");
        GameInfo.YellowLumsCount = GameInfo.Level.LumsCount;
        GameInfo.CagesCount = GameInfo.Level.CagesCount;
        // TODO: More setup...
        GameInfo.LevelType = LevelType.Normal;
        // TODO: More setup...
        TransitionsFX = new TransitionsFX();
        BaseActor.ActorDrawPriority = 1;
        Scene = new Scene2D((int)GameInfo.MapId, x => new CameraSideScroller(x), 4);
        // TODO: More setup...
        UserInfo = new UserInfoSideScroller(GameInfo.Level.HasBlueLum);
        Scene.AddDialog(UserInfo, false, false);
        // TODO: More setup...
        Scene.Init();

        // Add the circle FX as a screen. On the GBA this is done using a window.
        CircleFXRenderer = new CircleFXScreenRenderer();
        CircleFXScreen = new GfxScreen(4)
        {
            Priority = -1,
            IsEnabled = false,
            Camera = Engine.ScreenCamera, // If we use the tgx camera then it won't fill the entire screen, so use the screen camera for now
            Renderer = CircleFXRenderer,
        };

        // TODO: N-Gage seems to only have a transition when enter the hub, and it's a slight fade
        if (Engine.Settings.Platform == Platform.GBA)
            Gfx.AddScreen(CircleFXScreen);

        InitNewCircleFXTransition(true);

        Scene.AnimationPlayer.Execute();

        GameInfo.PlayLevelMusic();
        CurrentStepAction = Step_Normal;
    }
    
    public override void UnInit()
    {
        Scene.UnInit();
        GameInfo.StopLevelMusic();
        SoundManager.StopAll();
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
        StepCircleFX();
        Scene.AnimationPlayer.Execute();
    }

    #endregion

    #region Enums

    private enum CircleFXTransitionMode
    {
        None = 0,
        FinishedIn = 1,
        In = 2,
        Out = 3,
        FinishedOut = 4,
    }

    #endregion
}