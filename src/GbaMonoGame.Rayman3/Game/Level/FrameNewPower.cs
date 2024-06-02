using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.Engine2d;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Rayman3;

public class FrameNewPower : Frame, IHasScene, IHasPlayfield
{
    #region Constructor

    public FrameNewPower(MapId mapId)
    {
        OriginalMapId = GameInfo.MapId;
        GameInfo.SetNextMapId(mapId);
    }

    #endregion

    #region Private Properties

    private MapId OriginalMapId { get; }
    private Scene2D Scene { get; set; }
    private ushort Timer { get; set; }
    private bool HasStoppedMusic { get; set; }
    private byte field7_0x15 { get; set; }

    #endregion

    #region Public Properties

    public TransitionsFX TransitionsFX { get; set; }

    #endregion

    #region Interface Properties

    Scene2D IHasScene.Scene => Scene;
    TgxPlayfield IHasPlayfield.Playfield => Scene.Playfield;

    #endregion

    #region Public Methods

    public override void Init()
    {
        GameInfo.InitLevel(LevelType.Normal);

        LevelMusicManager.Init();
        TransitionsFX = new TransitionsFX(true);
        TransitionsFX.FadeInInit(1 / 16f);
        BaseActor.ActorDrawPriority = 1;
        Scene = new Scene2D((int)GameInfo.MapId, x => new CameraSideScroller(x), 3);

        Scene.Init();
        Scene.Playfield.Step();
        Scene.AnimationPlayer.Execute();
        GameInfo.PlayLevelMusic();

        Scene.MainActor.ProcessMessage(this, Message.Main_Stop);
        ((Rayman)Scene.MainActor).ActionId = Rayman.Action.Walk_Right;

        Timer = 0;
        HasStoppedMusic = false;
        
        if (Engine.Settings.Platform == Platform.NGage)
            field7_0x15 = 0;

        Scene.AddDialog(new TextBoxDialog(), false, false);

        SoundEngineInterface.SetNbVoices(10);

        if (GameInfo.MapId == MapId.Power1)
        {
            // TODO: Add config option for scrolling on N-Gage
            if (Engine.Settings.Platform == Platform.GBA)
            {
                TgxTileLayer cloudsLayer = ((TgxPlayfield2D)Scene.Playfield).TileLayers[0];
                cloudsLayer.Screen.Renderer = new LevelCloudsRenderer(((TextureScreenRenderer)cloudsLayer.Screen.Renderer).Texture, new[]
                {
                    56, 120, 227
                });
            }
        }
        else if (GameInfo.MapId == MapId.Power2)
        {
            Scene.AddDialog(new FogDialog(Scene.Playfield), false, false);
        }

        // Re-init with the original map id
        GameInfo.SetNextMapId(OriginalMapId);
        GameInfo.InitLevel(LevelType.Normal);
    }

    public override void UnInit()
    {
        Gfx.Fade = 1;

        Scene.UnInit();
        Scene = null;

        GameInfo.StopLevelMusic();
        SoundEventsManager.StopAllSongs();
        SoundEngineInterface.SetNbVoices(7);
    }

    public override void Step()
    {
        Scene.Step();
        Scene.Playfield.Step();
        TransitionsFX.StepAll();
        Scene.AnimationPlayer.Execute();
        LevelMusicManager.Step();

        // TODO: Implement ending level
    }

    #endregion
}