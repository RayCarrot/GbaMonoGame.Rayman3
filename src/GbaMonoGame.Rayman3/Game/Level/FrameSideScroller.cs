using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;
using GbaMonoGame.TgxEngine;
using Microsoft.Xna.Framework;
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

    #region Protected Properties

    protected CircleWindowEffectObject CircleEffect { get; set; }
    protected int CircleFXTimer { get; set; }
    protected CircleFXTransitionMode CircleFXMode { get; set; }

    protected FadeControl SavedFadeControl { get; set; }

    #endregion

    #region Public Properties

    public Scene2D Scene { get; set; }
    public Action CurrentStepAction { get; set; }

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
                CircleEffect.Radius = CircleFXTimer;
                break;

            case CircleFXTransitionMode.Out:
                CircleFXTimer -= 6;
                if (CircleFXTimer < 0)
                {
                    CircleFXTimer = 0;
                    CircleFXMode = CircleFXTransitionMode.FinishedOut;
                }
                CircleEffect.Radius = CircleFXTimer;
                break;
        }

        if (CircleFXMode != CircleFXTransitionMode.None)
            Scene.AnimationPlayer.PlayFront(CircleEffect);
    }

    #endregion

    #region Protected Methods

    protected void CreateCircleFXTransition()
    {
        Gfx.ClearColor = Color.Black;

        // Add the circle FX as an effect object. On the GBA this is done using a window.
        CircleEffect = new CircleWindowEffectObject
        {
            SpritePriority = 0,
        };
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

        CircleEffect.Camera = Scene.Playfield.Camera;
        CircleEffect.Init(CircleFXTimer, Scene.MainActor.ScreenPosition - new Vector2(0, 32));
    }

    public override void Init()
    {
        GameInfo.InitLevel(LevelType.Normal);

        CanPause = true;
        LevelMusicManager.Init();
        CreateCircleFXTransition();
        TransitionsFX = new TransitionsFX(true);
        BaseActor.ActorDrawPriority = 1;
        Scene = new Scene2D((int)GameInfo.MapId, x => new CameraSideScroller(x), 4);

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
            Fog = new FogDialog(Scene);
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
            LyTimer = new LyTimerDialog(Scene);
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
        PauseDialog = new PauseDialog(Scene);

        Scene.Init();
        Scene.Playfield.Step();

        InitNewCircleFXTransition(true);

        // We have to show the circle effect already now or we have one game frame with the level visible
        Scene.AnimationPlayer.PlayFront(CircleEffect);

        Scene.AnimationPlayer.Execute();

        GameInfo.PlayLevelMusic();
        CurrentStepAction = Step_Normal;
    }
    
    public override void UnInit()
    {
        Gfx.FadeControl = new FadeControl(FadeMode.BrightnessDecrease);
        Gfx.Fade = 1;

        Scene.UnInit();
        Scene = null;

        CircleFXTimer = 0;
        CircleFXMode = CircleFXTransitionMode.None;
        CircleEffect = null;

        GameInfo.StopLevelMusic();
        SoundEventsManager.StopAllSongs();
    }

    public override void Step()
    {
        CurrentStepAction();

        if (EndOfFrame)
            GameInfo.LoadLevel(GameInfo.GetNextLevelId());
    }

    #endregion

    #region Steps

    public void Step_Normal()
    {
        Scene.Step();
        Scene.Playfield.Step();
        TransitionsFX.StepAll();
        StepCircleFX();
        Scene.AnimationPlayer.Execute();
        LevelMusicManager.Step();
        
        if (IsTimed)
        {
            if (GameInfo.RemainingTime != 0)
                GameInfo.RemainingTime--;
        }

        // Pause
        if (JoyPad.IsButtonJustPressed(GbaInput.Start) && CircleFXMode == CircleFXTransitionMode.None && CanPause)
        {
            GameTime.Pause();
            CurrentStepAction = Fog != null ? Step_Pause_DisableFog :Step_Pause_Init;
        }
    }

    public void Step_Pause_DisableFog()
    {
        Fog.ShouldDraw = false;
        Scene.Step();
        Scene.Playfield.Step();
        Scene.AnimationPlayer.Execute();
        CurrentStepAction = Step_Pause_Init;
    }

    public void Step_Pause_Init()
    {
        SavedFadeControl = Gfx.FadeControl;

        // Fade after drawing screen 0, thus only leaving the sprites 0 as not faded
        Gfx.FadeControl = new FadeControl(FadeMode.BrightnessDecrease, FadeFlags.Screen0);
        Gfx.Fade = 6 / 16f;

        UserInfo.ProcessMessage(this, Message.UserInfo_Pause);

        SoundEventsManager.FinishReplacingAllSongs();
        SoundEventsManager.PauseAllSongs();

        Scene.ProcessDialogs();
        Scene.Playfield.Step();
        Scene.AnimationPlayer.Execute();
        CurrentStepAction = Step_Pause_AddDialog;
    }

    public void Step_Pause_AddDialog()
    {
        Scene.AddDialog(PauseDialog, true, false);

        if (Engine.Settings.Platform == Platform.NGage)
            NGage_0x4 = true;

        Scene.Step();
        UserInfo.Draw(Scene.AnimationPlayer);
        Scene.Playfield.Step();
        Scene.AnimationPlayer.Execute();
        CurrentStepAction = Step_Pause_Paused;
    }

    public void Step_Pause_Paused()
    {
        if (PauseDialog.DrawStep == PauseDialog.PauseDialogDrawStep.Hide)
            CurrentStepAction = Step_Pause_UnInit;

        Scene.Step();

        // The original game doesn't have this check, but since we're still running the game loop
        // while in the simulated sleep mode we have to make sure to not draw the HUD then
        if (!PauseDialog.IsInSleepMode)
            UserInfo.Draw(Scene.AnimationPlayer);
        
        Scene.Playfield.Step();
        Scene.AnimationPlayer.Execute();
    }

    public void Step_Pause_UnInit()
    {
        Scene.RemoveLastDialog();

        if (Engine.Settings.Platform == Platform.NGage)
            NGage_0x4 = false;

        Scene.RefreshDialogs();
        Scene.ProcessDialogs();

        // We probably don't need to do this, but in the original game it needs to reload things like
        // palette indexes since it might be allocated differently in VRAM after unpausing.
        foreach (GameObject gameObj in Scene.KnotManager.GameObjects)
            gameObj.ProcessMessage(this, Message.ReloadAnimation);

        Scene.Playfield.Step();
        Scene.AnimationPlayer.Execute();
        CurrentStepAction = Step_Pause_Resume;
    }

    public void Step_Pause_Resume()
    {
        Gfx.FadeControl = SavedFadeControl;
        Gfx.Fade = 0;

        if (Fog != null)
            Fog.ShouldDraw = true;

        UserInfo.ProcessMessage(this, Message.UserInfo_Unpause);

        SoundEventsManager.ResumeAllSongs();
        Scene.Step();
        Scene.Playfield.Step();
        Scene.AnimationPlayer.Execute();
        GameTime.Resume();
        CurrentStepAction = Step_Normal;
    }

    #endregion

    #region Enums

    protected enum CircleFXTransitionMode
    {
        None = 0,
        FinishedIn = 1,
        In = 2,
        Out = 3,
        FinishedOut = 4,
    }

    #endregion
}