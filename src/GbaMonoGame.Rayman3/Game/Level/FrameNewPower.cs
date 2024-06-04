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

    #endregion

    #region Public Properties

    public TransitionsFX TransitionsFX { get; set; }

    #endregion

    #region Interface Properties

    Scene2D IHasScene.Scene => Scene;
    TgxPlayfield IHasPlayfield.Playfield => Scene.Playfield;

    #endregion

    #region Private Methods

    private void CheckForEndOfLevel()
    {
        // TODO: On N-Gage it has additional code here. The very first replay frame it forces a re-scan of the JoyPad,
        //       essentially skipping the first replay frame. Why is this needed? Because of port differences, but if
        //       so do we need to do something similar?

        if (JoyPad.IsReplayFinished)
        {
            Timer = 1;
            Scene.MainActor.ProcessMessage(this, Message.Main_Stop);
        }
    }

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
        
        Scene.AddDialog(new TextBoxDialog(Scene), false, false);

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
            Scene.AddDialog(new FogDialog(Scene), false, false);
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

        if (Timer == 0)
        {
            CheckForEndOfLevel();
        }
        else
        {
            Timer++;

            if (TransitionsFX.IsFadeOutFinished)
            {
                // Begin fade out after 1 second
                if (Timer == 61)
                {
                    TransitionsFX.FadeOutInit(1 / 16f);
                }
                else if (Timer > 61)
                {
                    // Stop music
                    if (!HasStoppedMusic)
                    {
                        SoundEventsManager.StopAllSongs();
                        HasStoppedMusic = true;
                    }
                    // End level
                    else
                    {
                        GameInfo.UpdateLastCompletedLevel();
                        GameInfo.Save(GameInfo.CurrentSlot);
                        GameInfo.LoadLevel(GameInfo.GetNextLevelId());
                    }
                }
            }
        }
    }

    #endregion
}