using System;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Rayman3;

public class FrameWorldSideScroller : Frame, IHasScene, IHasPlayfield
{
    #region Constructor

    public FrameWorldSideScroller(MapId mapId)
    {
        GameInfo.SetNextMapId(mapId);
    }

    #endregion

    #region Protected Properties

    protected Scene2D Scene { get; set; }
    protected Action CurrentStepAction { get; set; }

    protected CachedTileKit CachedTileKit { get; set; } // Cache so we can quickly reload when dying

    #endregion

    #region Private Properties

    private byte field6_0x16 { get; set; }

    #endregion

    #region Public Properties

    public TransitionsFX TransitionsFX { get; set; }
    public PauseDialog PauseDialog { get; set; }

    #endregion

    #region Interface Properties

    Scene2D IHasScene.Scene => Scene;
    TgxPlayfield IHasPlayfield.Playfield => Scene.Playfield;

    #endregion

    #region Pubic Methods

    public override void Init()
    {
        GameInfo.InitLevel(LevelType.Normal);
        LevelMusicManager.Init();

        TransitionsFX = new TransitionsFX(true);
        TransitionsFX.FadeInInit(1 / 16f);

        BaseActor.ActorDrawPriority = 1;
        Scene = new Scene2D((int)GameInfo.MapId, x => new CameraSideScroller(x), 3, CachedTileKit);
        CachedTileKit = CachedTileKit.FromPlayfield(Scene.Playfield);

        // Set start position
        if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4)
        {
            // Get the actor to spawn the main actor at (either default position or at a curtain)
            int startActorId = GameInfo.PersistentInfo.LastPlayedLevel == 0xFF
                ? 0
                : GameInfo.Levels[GameInfo.PersistentInfo.LastPlayedLevel].LevelCurtainActorId;

            Vector2 startPos = Scene.KnotManager.GetGameObject(startActorId).Position;
            startPos -= new Vector2(32, 0);

            while (Scene.GetPhysicalType(startPos) == PhysicalTypeValue.None)
                startPos += new Vector2(0, Constants.TileSize);

            Scene.MainActor.Position = startPos;
            Scene.Camera.SetFirstPosition();
        }

        // Create pause dialog, but don't add yet
        PauseDialog = new PauseDialog();

        Scene.Init();
        Scene.Playfield.Step();
        Scene.AnimationPlayer.Execute();

        if (!SoundEventsManager.IsSongPlaying(GameInfo.GetLevelMusicSoundEvent())) // TODO: N-Gage doesn't have this condition - why?
            GameInfo.PlayLevelMusic();

        field6_0x16 = 0;
        CurrentStepAction = Step_Normal;
    }

    public override void UnInit()
    {
        Gfx.Fade = 1;

        Scene.UnInit();
        Scene = null;

        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__LumTimer_Mix02);
    }

    public override void Step()
    {
        CurrentStepAction();

        if (EndOfFrame)
            GameInfo.LoadLevel(GameInfo.GetNextLevelId());
    }

    #endregion

    #region Steps

    protected void Step_Normal()
    {
        Scene.Step();
        Scene.Playfield.Step();
        TransitionsFX.StepAll();
        Scene.AnimationPlayer.Execute();
        LevelMusicManager.Step();

        if (JoyPad.CheckSingle(GbaInput.Start) && field6_0x16 == 0)
        {
            GameTime.IsPaused = true;
            // TODO: Go to pause state
        }
    }

    #endregion
}