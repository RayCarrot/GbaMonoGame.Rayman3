using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;
using GbaMonoGame.TgxEngine;
using Action = System.Action;

namespace GbaMonoGame.Rayman3;

public abstract class FrameWorldSideScroller : Frame, IHasScene, IHasPlayfield
{
    #region Constructor

    protected FrameWorldSideScroller(MapId mapId)
    {
        GameInfo.SetNextMapId(mapId);
    }

    #endregion

    #region Protected Properties

    protected Scene2D Scene { get; set; }
    protected Action CurrentStepAction { get; set; }

    #endregion

    #region Public Properties

    public bool BlockPause { get; set; }
    public TransitionsFX TransitionsFX { get; set; }
    public PauseDialog PauseDialog { get; set; }

    #endregion

    #region Interface Properties

    Scene2D IHasScene.Scene => Scene;
    TgxPlayfield IHasPlayfield.Playfield => Scene.Playfield;

    #endregion

    #region Pubic Methods

    public bool IsBusy() => CurrentStepAction != Step_Normal;

    public override void Init()
    {
        GameInfo.InitLevel(LevelType.Normal);
        LevelMusicManager.Init();

        TransitionsFX = new TransitionsFX(true);
        TransitionsFX.FadeInInit(1 / 16f);

        BaseActor.ActorDrawPriority = 1;
        Scene = new Scene2D((int)GameInfo.MapId, x => new CameraSideScroller(x), 3);

        // Set start position
        if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4)
        {
            // Get the actor to spawn the main actor at (either default position or at a curtain)
            int startActorId = GameInfo.PersistentInfo.LastPlayedLevel == 0xFF
                ? 0
                : GameInfo.Levels[GameInfo.PersistentInfo.LastPlayedLevel].LevelCurtainActorId;

            Vector2 startPos = Scene.GetGameObject(startActorId).Position;
            startPos -= new Vector2(32, 0);

            while (Scene.GetPhysicalType(startPos) == PhysicalTypeValue.None)
                startPos += new Vector2(0, Tile.Size);

            Scene.MainActor.Position = startPos;
            Scene.Camera.SetFirstPosition();
        }

        // Create pause dialog, but don't add yet
        PauseDialog = new PauseDialog(Scene);

        Scene.Init();
        // NOTE: The game calls vsync, steps the playfield and executes the animations here, but we do
        //       that in the derived classed instead since this is all to be run in one game frame.

        if (!SoundEventsManager.IsSongPlaying(GameInfo.GetLevelMusicSoundEvent())) // TODO: N-Gage doesn't have this condition - why?
            GameInfo.PlayLevelMusic();

        BlockPause = false;
        CurrentStepAction = Step_Normal;
    }

    public override void UnInit()
    {
        Gfx.SetFullFade();

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

        if (JoyPad.IsButtonJustPressed(GbaInput.Start) && !BlockPause)
        {
            GameTime.Pause();
            // TODO: Go to pause state
        }
    }

    #endregion
}