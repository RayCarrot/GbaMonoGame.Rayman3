using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Rayman3;

public class FrameSideScrollerGCN : FrameSideScroller
{
    public FrameSideScrollerGCN(GameCubeMapInfo mapInfo, GameCubeMap map, int gcnMapId) : base(default)
    {
        MapInfo = mapInfo;
        Map = map;
        GcnMapId = gcnMapId;
    }

    private GameCubeMapInfo MapInfo { get; }
    private GameCubeMap Map { get; }

    private MapId PreviousMapId { get; set; }
    private Power PreviousPowers { get; set; }

    public int GcnMapId { get; }

    public void RestoreMapAndPowers()
    {
        GameInfo.MapId = PreviousMapId;
        GameInfo.Powers = PreviousPowers;
    }

    public void FUN_0808a9f4()
    {
        if (GcnMapId == 3)
        {
            // TODO: Implement
        }
    }

    public override void Init()
    {
        GameInfo.InitLevel(LevelType.GameCube);

        PreviousMapId = GameInfo.MapId;
        GameInfo.MapId = MapId.GameCube_Bonus1 + GcnMapId;

        PreviousPowers = GameInfo.Powers;
        GameInfo.Powers = Power.All;

        GameInfo.YellowLumsCount = MapInfo.LumsCount;
        GameInfo.CagesCount = MapInfo.CagesCount;

        SoundEventsManager.ProcessEvent(MapInfo.StartMusicSoundEvent);

        TransitionsFX = new TransitionsFX(true);
        BaseActor.ActorDrawPriority = 1;
        Scene = new Scene2D(Map, x => new CameraSideScroller(x), 4, CachedTileKit);
        CachedTileKit = CachedTileKit.FromPlayfield(Scene.Playfield);

        // Add user info (default hud)
        UserInfo = new UserInfoSideScroller(Scene, MapInfo.HasBlueLum);
        UserInfo.ProcessMessage((Message)1081); // TODO: Implement and name message

        // Create pause dialog, but don't add yet
        PauseDialog = new PauseDialog();

        Scene.AddDialog(UserInfo, false, false);
        Scene.Init();

        CreateCircleFXTransition();
        InitNewCircleFXTransition(true);

        Scene.AnimationPlayer.Execute();

        CanPause = true;
        Fog = null;
        LyTimer = null;
        CurrentStepAction = Step_Normal;

        switch (GcnMapId)
        {
            case 0:
                // Add fog
                Fog = new FogDialog(Scene.Playfield);
                Scene.AddDialog(Fog, false, false);
                break;
            
            case 1:
                // TODO: Add config option for scrolling on N-Gage
                if (Engine.Settings.Platform == Platform.GBA)
                {
                    TgxTileLayer cloudsLayer = Scene.Playfield.TileLayers[0];
                    cloudsLayer.Screen.Renderer = new LevelCloudsRenderer(((TextureScreenRenderer)cloudsLayer.Screen.Renderer).Texture, new[]
                    {
                        32, 120, 227
                    });
                }
                break;
            
            case 2:
                // TODO: Implement
                break;
            
            case 4:
                // TODO: Implement
                break;
            
            case 3 or 6:
                // TODO: Implement
                break;
        }
    }

    public override void Step()
    {
        base.Step();

        // TODO: Level-specific code
    }
}