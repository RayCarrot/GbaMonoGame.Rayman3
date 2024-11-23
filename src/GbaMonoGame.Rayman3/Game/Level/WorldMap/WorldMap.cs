using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;
using GbaMonoGame.TgxEngine;
using Microsoft.Xna.Framework;
using Action = System.Action;

namespace GbaMonoGame.Rayman3;

public class WorldMap : Frame, IHasScene, IHasPlayfield
{
    #region Constructor

    public WorldMap(MapId mapId)
    {
        GameInfo.SetNextMapId(mapId);

        unk3 = 0;
        WorldId = GameInfo.World;
        unk2 = 0;
        ScrollX = Engine.Settings.Platform switch
        {
            Platform.GBA => 56,
            Platform.NGage => 0,
            _ => throw new UnsupportedPlatformException()
        };

        GameInfo.IsInWorldMap = true;
    }

    #endregion

    #region Public Properties

    public Scene2D Scene { get; set; }
    public TransitionsFX TransitionsFX { get; set; }
    public UserInfoWorld UserInfo { get; set; }
    public PauseDialog PauseDialog { get; set; }

    public AnimatedObject Rayman { get; set; }
    public AnimatedObject[] WorldPaths { get; set; }
    public AnimatedObject GameCubeSparkles { get; set; }
    public SpriteTextObject FullWorldName { get; set; }

    public Action CurrentStepAction { get; set; }
    public Action CurrentExStepAction { get; set; }

    public float ScrollX { get; set; }
    public byte CheatValue { get; set; }
    public byte CircleWipeFXMode { get; set; } // TODO: Enum
    public int WorldId { get; set; }

    // TODO: Name
    public byte NGageUnk1 { get; set; }
    public byte unk2 { get; set; }
    public byte unk3 { get; set; }
    public ushort unk4 { get; set; }
    public short unk5 { get; set; }

    #endregion

    #region Interface Properties

    Scene2D IHasScene.Scene => Scene;
    TgxPlayfield IHasPlayfield.Playfield => Scene.Playfield;

    #endregion

    #region Private Methods

    // TODO: Name
    private void FUN_0808ee08()
    {
        // TODO: Implement
    }

    // TODO: Name
    private void FUN_0808ef90()
    {
        // TODO: Implement
    }

    // TODO: Name
    private void FUN_0808f2b4()
    {
        // TODO: Implement
    }

    #endregion

    #region Public Methods

    public override void Init()
    {
        switch (WorldId)
        {
            case 0:
                ScrollX = 0;
                break;

            case 1:
                ScrollX = Engine.Settings.Platform switch
                {
                    Platform.GBA => 128,
                    Platform.NGage => 72,
                    _ => throw new UnsupportedPlatformException()
                };
                break;
            
            case 2:
            case 3:
                ScrollX = Engine.Settings.Platform switch
                {
                    Platform.GBA => MathHelpers.FromFixedPoint(0xdf3544), // ???
                    Platform.NGage => 176,
                    _ => throw new UnsupportedPlatformException()
                };
                break;
        }

        unk5 = 0xFF;
        CircleWipeFXMode = 2;
        // TODO: Create circle wipe fx

        TransitionsFX = new TransitionsFX(true);
        GameInfo.InitLevel(LevelType.Normal);
        LevelMusicManager.Init();
        
        Scene = new Scene2D((int)GameInfo.MapId, x => new CameraWorldMap(x), 3, 1);

        // TODO: Find way to get rid of black bar at the bottom
        // Engine.GameViewPort.SetResolutionBounds(null, ((TgxPlayfield2D)Scene.Playfield).Size - new Vector2(0, 64));

        // Create pause dialog, but don't add yet
        PauseDialog = new PauseDialog(Scene);

        Scene.Init();
        Scene.Playfield.Step();

        Scene.AnimationPlayer.Execute();

        if (!SoundEventsManager.IsSongPlaying(GameInfo.GetLevelMusicSoundEvent()))
            GameInfo.PlayLevelMusic();

        UserInfo = new UserInfoWorld(Scene, GameInfo.Level.HasBlueLum);
        Scene.AddDialog(UserInfo, false, false);

        AnimatedObjectResource raymanResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.RaymanWorldMapAnimations);
        Rayman = new AnimatedObject(raymanResource, raymanResource.IsDynamic)
        {
            IsFramed = true,
            CurrentAnimation = 15,
            BgPriority = 1,
            ObjPriority = 0,
            ScreenPos = Engine.Settings.Platform switch
            {
                Platform.GBA => new Vector2(246 - ScrollX, 84),
                Platform.NGage => new Vector2(175 - ScrollX, 114),
                _ => throw new UnsupportedPlatformException()
            },
            Camera = Scene.Playfield.Camera,
        };

        AnimatedObjectResource worldPathsResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.WorldMapPathAnimations);
        WorldPaths = new AnimatedObject[3];
        for (int i = 0; i < WorldPaths.Length; i++)
        {
            WorldPaths[i] = new AnimatedObject(worldPathsResource, worldPathsResource.IsDynamic)
            {
                IsFramed = true,
                BgPriority = 1,
                ObjPriority = 32,
                Camera = Scene.Playfield.Camera,
            };
        }

        WorldPaths[0].CurrentAnimation = GameInfo.PersistentInfo.UnlockedWorld2 ? 3 : 6;
        WorldPaths[0].ScreenPos = Engine.Settings.Platform switch
        {
            Platform.GBA => new Vector2(246 - ScrollX, 84),
            Platform.NGage => new Vector2(175 - ScrollX, 114),
            _ => throw new UnsupportedPlatformException()
        };

        WorldPaths[1].CurrentAnimation = 4;
        WorldPaths[1].ScreenPos = Engine.Settings.Platform switch
        {
            Platform.GBA => new Vector2(246 - ScrollX, 84),
            Platform.NGage => new Vector2(175 - ScrollX, 114),
            _ => throw new UnsupportedPlatformException()
        };

        WorldPaths[2].CurrentAnimation = 5;
        WorldPaths[2].ScreenPos = Engine.Settings.Platform switch
        {
            Platform.GBA => new Vector2(246 - ScrollX, 84),
            Platform.NGage => new Vector2(175 - ScrollX, 114),
            _ => throw new UnsupportedPlatformException()
        };

        if (Engine.Settings.Platform == Platform.GBA)
        {
            GameCubeSparkles = new AnimatedObject(worldPathsResource, worldPathsResource.IsDynamic)
            {
                IsFramed = true,
                BgPriority = 1,
                ObjPriority = 32,
                CurrentAnimation = 7,
                ScreenPos = new Vector2(246 - ScrollX, 84),
                Camera = Scene.Playfield.Camera,
            };
        }

        FullWorldName = new SpriteTextObject()
        {
            Color = TextColor.FullWorldName,
            ScreenPos = new Vector2(120, 60),
            Text = "",
            field_0x14 = true,
            Camera = Scene.HudCamera,
        };

        switch (WorldId)
        {
            case 0:
                Rayman.CurrentAnimation = 15;
                break;

            case 1:
                Rayman.CurrentAnimation = 16;
                break;
            
            case 2:
                Rayman.CurrentAnimation = 17;
                break;
            
            case 3:
                Rayman.CurrentAnimation = 18;
                break;
        }

        if (GameInfo.PersistentInfo.UnlockedWorld2 && !GameInfo.PersistentInfo.PlayedWorld2Unlock)
            CurrentExStepAction = StepEx_UnlockWorld2;
        else if (GameInfo.PersistentInfo.UnlockedWorld3 && !GameInfo.PersistentInfo.PlayedWorld3Unlock)
            CurrentExStepAction = StepEx_UnlockWorld3;
        else if (GameInfo.PersistentInfo.UnlockedWorld4 && !GameInfo.PersistentInfo.PlayedWorld4Unlock)
            CurrentExStepAction = StepEx_UnlockWorld4;
        else
            CurrentExStepAction = StepEx_Play;

        unk4 = 0;

        FUN_0808ee08();
        FUN_0808ef90();
        FUN_0808f2b4();

        UserInfo.WorldNameBar.SetWorld(WorldId);
        UserInfo.WorldNameBar.CanMoveIn = true;
        UserInfo.WorldNameBar.MoveInWorldNameBar();

        CurrentStepAction = Step_Normal;

        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Spirale_Mix01);

        CheatValue = 0;

        if (Engine.Settings.Platform == Platform.NGage)
            NGageUnk1 = 0;
    }

    public override void UnInit()
    {
        Gfx.FadeControl = new FadeControl(FadeMode.BrightnessDecrease);
        Gfx.Fade = 1;

        Scene.UnInit();
        Scene = null;

        Gfx.ClearColor = Color.Black;
    }

    public override void Step()
    {
        if (CurrentStepAction == Step_Normal)
            CurrentExStepAction();

        CurrentStepAction();
    }

    #endregion

    #region Steps

    private void StepEx_UnlockWorld2()
    {
        // TODO: Implement
    }

    private void StepEx_UnlockWorld3()
    {
        // TODO: Implement
    }

    private void StepEx_UnlockWorld4()
    {
        // TODO: Implement
    }

    private void StepEx_Play()
    {
        // TODO: Implement
    }

    private void Step_Normal()
    {
        Scene.Step();

        // TODO: Update circle wipe

        Scene.Playfield.Step();
        Scene.AnimationPlayer.Execute();
        LevelMusicManager.Step();

        // TODO: Handle pausing
    }

    #endregion
}