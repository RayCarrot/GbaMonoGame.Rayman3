using System;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;
using GbaMonoGame.TgxEngine;
using Action = System.Action;

namespace GbaMonoGame.Rayman3;

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

    private CachedTileKit CachedTileKit { get; set; } // Cache so we can quickly reload when dying

    #endregion

    #region Protected Properties

    protected Scene2D Scene { get; set; }
    protected Action CurrentStepAction { get; set; }

    #endregion

    #region Public Properties

    public TransitionsFX TransitionsFX { get; set; }
    public UserInfoSideScroller UserInfo { get; set; }
    public FogDialog Fog { get; set; }
    public LyTimerDialog LyTimer { get; set; }
    public PauseDialog PauseDialog { get; set; }

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
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__SlideIn_Mix02);
        }
        else
        {
            CircleFXTimer = 252;
            CircleFXMode = CircleFXTransitionMode.Out;
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__SlideOut_Mix01);
        }

        CircleFXScreen.IsEnabled = true;
        CircleFXRenderer.Radius = CircleFXTimer;
        CircleFXRenderer.CirclePosition = Scene.MainActor.ScreenPosition - new Vector2(0, 32);
        Gfx.Fade = 0; // We might have a leftover fade making the screen black, but now we show the circle FX screen instead
    }

    public override void Init()
    {
        GameInfo.LoadedYellowLums = 0;
        GameInfo.LoadedCages = 0;
        GameInfo.GreenLums = 0;
        GameInfo.MapId = GameInfo.NextMapId ?? throw new Exception("No map id set");
        GameInfo.YellowLumsCount = GameInfo.Level.LumsCount;
        GameInfo.CagesCount = GameInfo.Level.CagesCount;
        GameInfo.GameCubeCollectedYellowLumsCount = 0;
        GameInfo.GameCubeCollectedCagesCount = 0;
        GameInfo.LevelType = LevelType.Normal;

        CanPause = true;
        TransitionsFX = new TransitionsFX();
        BaseActor.ActorDrawPriority = 1;
        Scene = new Scene2D((int)GameInfo.MapId, x => new CameraSideScroller(x), 4, CachedTileKit);
        CachedTileKit = CachedTileKit.FromPlayfield(Scene.Playfield);

        // Add fog
        if (GameInfo.MapId is 
            MapId.SanctuaryOfBigTree_M1 or 
            MapId.SanctuaryOfBigTree_M2 or 
            MapId.MenhirHills_M1 or 
            MapId.MenhirHills_M2 or 
            MapId.ThePrecipice_M1 or 
            MapId.TheCanopy_M2 or 
            MapId.Bonus1)
        {
            Fog = new FogDialog(Scene.Playfield);
            Scene.AddDialog(Fog, false, false);
        }
        else
        {
            Fog = null;
        }

        // Add timer
        if (GameInfo.MapId is 
            MapId.ChallengeLy1 or 
            MapId.ChallengeLy2 or 
            MapId.ChallengeLyGCN)
        {
            LyTimer = new LyTimerDialog();
            Scene.AddDialog(LyTimer, false, false);
        }
        else
        {
            LyTimer = null;
        }

        // Add user info (default hud)
        UserInfo = new UserInfoSideScroller(Scene, GameInfo.Level.HasBlueLum);
        Scene.AddDialog(UserInfo, false, false);

        // Create pause dialog, but don't add yet
        PauseDialog = new PauseDialog();

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
        Gfx.Fade = 1;

        Scene.UnInit();
        Scene = null;

        CircleFXTimer = 0;
        CircleFXMode = CircleFXTransitionMode.None;
        CircleFXScreen = null;
        CircleFXRenderer = null;

        GameInfo.StopLevelMusic();
        SoundEventsManager.StopAll();
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
        TransitionsFX.StepAll();
        StepCircleFX();
        Scene.AnimationPlayer.Execute();
        // FUN_08001850(); // TODO: Implement
        
        if (IsTimed)
        {
            if (GameInfo.RemainingTime != 0)
                GameInfo.RemainingTime--;
        }

        if (JoyPad.CheckSingle(GbaInput.Start) && CircleFXMode == CircleFXTransitionMode.None && CanPause)
        {
            GameTime.IsPaused = true;
            // TODO: Go to pause state
        }
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