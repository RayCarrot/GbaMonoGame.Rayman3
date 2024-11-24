using System;
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

        CurrentMovement = WorldMapMovement.None;
        WorldId = GameInfo.WorldId;
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

    public ushort Timer { get; set; }
    public float ScrollX { get; set; }
    public byte NGageScrollCooldown { get; set; }
    public byte SpikyBagSinValue { get; set; }
    public bool SpikyBagScrollDirection { get; set; }
    public byte CircleWipeFXMode { get; set; } // TODO: Enum
    public WorldMapMovement CurrentMovement { get; set; }
    public byte CheatValue { get; set; }
    public WorldId WorldId { get; set; }

    public Vector2 BaseObjPos => Engine.Settings.Platform switch
    {
        Platform.GBA => new Vector2(246, 84),
        Platform.NGage => new Vector2(175, 114),
        _ => throw new UnsupportedPlatformException()
    };

    // TODO: Name
    public byte unk2 { get; set; }
    public short unk5 { get; set; }

    #endregion

    #region Interface Properties

    Scene2D IHasScene.Scene => Scene;
    TgxPlayfield IHasPlayfield.Playfield => Scene.Playfield;

    #endregion

    #region Private Methods

    private void InitSpikyBag()
    {
        GfxScreen spikyBagScreen = Gfx.GetScreen(3);
        float offsetX = Engine.Settings.Platform switch
        {
            Platform.GBA => 16,
            Platform.NGage => 72,
            _ => throw new UnsupportedPlatformException()
        };

        // In the original game it keeps the wrap, but creates a window to hide the screen on the left side of the worldmap
        if (Engine.Settings.Platform == Platform.GBA)
        {
            spikyBagScreen.Wrap = false;
            offsetX += spikyBagScreen.Renderer.GetSize(spikyBagScreen).X;
        }

        spikyBagScreen.Offset = spikyBagScreen.Offset with { X = ScrollX + offsetX };

        SpikyBagSinValue = 0;
    }

    private void StepSpikyBag()
    {
        GfxScreen spikyBagScreen = Gfx.GetScreen(3);
        float offsetX = Engine.Settings.Platform switch
        {
            Platform.GBA => 16,
            Platform.NGage => 165,
            _ => throw new UnsupportedPlatformException()
        };

        // In the original game it keeps the wrap, but creates a window to hide the screen on the left side of the worldmap
        if (Engine.Settings.Platform == Platform.GBA)
            offsetX -= spikyBagScreen.Renderer.GetSize(spikyBagScreen).X;

        spikyBagScreen.Offset = spikyBagScreen.Offset with { X = ScrollX - (4 * MathHelpers.Sin256(SpikyBagSinValue) - offsetX) };

        if (!SpikyBagScrollDirection)
        {
            SpikyBagSinValue++;
            if (SpikyBagSinValue == 63)
                SpikyBagScrollDirection = true;
        }
        else
        {
            SpikyBagSinValue--;
            if (SpikyBagSinValue == 192)
                SpikyBagScrollDirection = false;
        }
    }

    private void InitLightning()
    {
        // TODO: Implement
    }

    private void StepLightning()
    {
        // TODO: Implement
    }

    private void InitVolcanoGlow()
    {
        // TODO: Implement
    }

    private void StepVolcanoGlow()
    {
        // TODO: Implement
    }

    private bool ProcessCheatInput(GbaInput input)
    {
        GbaInput[] cheatInputs =
        [
            GbaInput.A,
            GbaInput.B,
            GbaInput.Right,
            GbaInput.Left,
            GbaInput.Up,
            GbaInput.Down,
            GbaInput.R,
            GbaInput.L
        ];

        if (JoyPad.IsButtonJustPressed(input))
        {
            CheatValue++;
            return true;
        }
        else
        {
            foreach (GbaInput cheatInput in cheatInputs)
            {
                if (cheatInput != input && JoyPad.IsButtonJustPressed(cheatInput))
                {
                    CheatValue = 0;
                    break;
                }
            }

            return false;
        }
    }

    private void ManageCheats()
    {
        // Make sure select is held down
        if (JoyPad.IsButtonReleased(GbaInput.Select))
        {
            CheatValue = 0;
            return;
        }

        // TODO: There is currently no feedback to the player if a cheat has been entered. In the original game
        //       you notice it since the game freezes for a second when saving. Perhaps we should play some sound
        //       effect here?
        switch (CheatValue)
        {
            // Start
            case 0:
                if (JoyPad.IsButtonJustPressed(GbaInput.R))
                    CheatValue = 1;

                if (JoyPad.IsButtonJustPressed(GbaInput.B))
                    CheatValue = 32;

                if (JoyPad.IsButtonJustPressed(GbaInput.Up))
                    CheatValue = 64;

                if (JoyPad.IsButtonJustPressed(GbaInput.Down))
                    CheatValue = 100;
                break;

            // All powers (R + A + A + A)
            case 1:
                ProcessCheatInput(GbaInput.A);
                break;
            
            case 2:
                ProcessCheatInput(GbaInput.A);
                break;

            case 3:
                if (ProcessCheatInput(GbaInput.A))
                {
                    GameInfo.EnableCheat(Scene, Cheat.AllPowers);
                    
                    CheatValue = 0;
                    
                    GameInfo.Save(GameInfo.CurrentSlot);
                }
                break;

            // 99 lives (B + B + B + A + A + A)
            case 32:
                ProcessCheatInput(GbaInput.B);
                break;

            case 33:
                ProcessCheatInput(GbaInput.B);
                break;

            case 34:
                ProcessCheatInput(GbaInput.A);
                break;

            case 35:
                ProcessCheatInput(GbaInput.A);
                break;

            case 36:
                if (ProcessCheatInput(GbaInput.A))
                {
                    GameInfo.EnableCheat(Scene, Cheat.InfiniteLives);
                    
                    CheatValue = 0;
                }
                break;

            // Unlock all levels (Up + Left + Down + Right + B + A + L + R)
            case 64:
                ProcessCheatInput(GbaInput.Left);
                break;

            case 65:
                ProcessCheatInput(GbaInput.Down);
                break;

            case 66:
                ProcessCheatInput(GbaInput.Right);
                break;

            case 67:
                ProcessCheatInput(GbaInput.B);
                break;

            case 68:
                ProcessCheatInput(GbaInput.A);
                break;

            case 69:
                ProcessCheatInput(GbaInput.L);
                break;

            case 70:
                if (ProcessCheatInput(GbaInput.R))
                {
                    GameInfo.EnableCheat(Scene, Cheat.AllPowers);
                    UnlockAllLevels();
                    
                    CheatValue = 0;
                 
                    GameInfo.Save(GameInfo.CurrentSlot);
                }
                break;

            // 100% (Down + Up + Down + Up + A + Left + B + Right)
            case 100:
                ProcessCheatInput(GbaInput.Up);
                break;

            case 101:
                ProcessCheatInput(GbaInput.Down);
                break;

            case 102:
                ProcessCheatInput(GbaInput.Up);
                break;

            case 103:
                ProcessCheatInput(GbaInput.A);
                break;

            case 104:
                ProcessCheatInput(GbaInput.Left);
                break;

            case 105:
                ProcessCheatInput(GbaInput.B);
                break;

            case 106:
                if (ProcessCheatInput(GbaInput.Right))
                {
                    GameInfo.PersistentInfo.FinishedLyChallenge1 = true;
                    GameInfo.PersistentInfo.FinishedLyChallenge2 = true;

                    if (Engine.Settings.Platform == Platform.GBA)
                        GameInfo.PersistentInfo.FinishedLyChallengeGCN = true;
                    
                    GameInfo.PersistentInfo.PlayedAct4 = true;
                    GameInfo.PersistentInfo.PlayedMurfyWorldHelp = true;
                    GameInfo.PersistentInfo.UnlockedFinalBoss = true;

                    if (Engine.Settings.Platform == Platform.GBA)
                        GameInfo.PersistentInfo.CompletedGCNBonusLevels = 10;

                    GameInfo.SetAllCagesAsCollected();
                    GameInfo.SetAllYellowLumsAsCollected();

                    GameInfo.EnableCheat(Scene, Cheat.InfiniteLives);
                    GameInfo.EnableCheat(Scene, Cheat.AllPowers);
                    UnlockAllLevels();
                    
                    CheatValue = 0;

                    UserInfo.Lums1000Bar.Set();
                    UserInfo.Cages50Bar.Set();

                    GameInfo.Save(GameInfo.CurrentSlot);
                }
                break;
        }
    }

    private void UnlockAllLevels()
    {
        MapId mapId = GameInfo.MapId;
        GameInfo.MapId = MapId.BossFinal_M2;
        GameInfo.UpdateLastCompletedLevel();
        GameInfo.MapId = mapId;

        GameInfo.PersistentInfo.UnlockedWorld2 = true;
        GameInfo.PersistentInfo.PlayedWorld2Unlock = true;
        GameInfo.PersistentInfo.UnlockedWorld3 = true;
        GameInfo.PersistentInfo.PlayedWorld3Unlock = true;
        GameInfo.PersistentInfo.UnlockedWorld4 = true;
        GameInfo.PersistentInfo.PlayedWorld4Unlock = true;
        GameInfo.PersistentInfo.PlayedAct4 = true;

        WorldPaths[0].CurrentAnimation = 3;
    }

    #endregion

    #region Public Methods

    public override void Init()
    {
        switch (WorldId)
        {
            case WorldId.World1:
                ScrollX = 0;
                break;

            case WorldId.World2:
                ScrollX = Engine.Settings.Platform switch
                {
                    Platform.GBA => 128,
                    Platform.NGage => 72,
                    _ => throw new UnsupportedPlatformException()
                };
                break;
            
            case WorldId.World3:
            case WorldId.World4:
                ScrollX = Engine.Settings.Platform switch
                {
                    Platform.GBA => MathHelpers.FromFixedPoint(0xdf3544), // ???
                    Platform.NGage => 176,
                    _ => throw new UnsupportedPlatformException()
                };
                break;

            default:
                throw new Exception("Invalid world id");
        }

        unk5 = 0xFF;
        //CircleWipeFXMode = 2;
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
            ScreenPos = BaseObjPos - new Vector2(ScrollX, 0),
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
        WorldPaths[0].ScreenPos = BaseObjPos - new Vector2(ScrollX, 0);

        WorldPaths[1].CurrentAnimation = 4;
        WorldPaths[1].ScreenPos = BaseObjPos - new Vector2(ScrollX, 0);

        WorldPaths[2].CurrentAnimation = 5;
        WorldPaths[2].ScreenPos = BaseObjPos - new Vector2(ScrollX, 0);

        if (Engine.Settings.Platform == Platform.GBA)
        {
            GameCubeSparkles = new AnimatedObject(worldPathsResource, worldPathsResource.IsDynamic)
            {
                IsFramed = true,
                BgPriority = 1,
                ObjPriority = 32,
                CurrentAnimation = 7,
                ScreenPos = BaseObjPos - new Vector2(ScrollX, 0),
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

        Rayman.CurrentAnimation = WorldId switch
        {
            WorldId.World1 => 15,
            WorldId.World2 => 16,
            WorldId.World3 => 17,
            WorldId.World4 => 18,
            _ => throw new Exception("Invalid world id")
        };

        if (GameInfo.PersistentInfo.UnlockedWorld2 && !GameInfo.PersistentInfo.PlayedWorld2Unlock)
            CurrentExStepAction = StepEx_UnlockWorld2;
        else if (GameInfo.PersistentInfo.UnlockedWorld3 && !GameInfo.PersistentInfo.PlayedWorld3Unlock)
            CurrentExStepAction = StepEx_UnlockWorld3;
        else if (GameInfo.PersistentInfo.UnlockedWorld4 && !GameInfo.PersistentInfo.PlayedWorld4Unlock)
            CurrentExStepAction = StepEx_UnlockWorld4;
        else
            CurrentExStepAction = StepEx_Play;

        Timer = 0;

        InitSpikyBag();
        InitLightning();
        InitVolcanoGlow();

        UserInfo.WorldNameBar.SetWorld(WorldId);
        UserInfo.WorldNameBar.CanMoveIn = true;
        UserInfo.WorldNameBar.MoveInWorldNameBar();

        CurrentStepAction = Step_Normal;

        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Spirale_Mix01);

        CheatValue = 0;

        if (Engine.Settings.Platform == Platform.NGage)
            NGageScrollCooldown = 0;
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
        float xPos = BaseObjPos.X - ScrollX;
        Rayman.ScreenPos = Rayman.ScreenPos with { X = xPos };
        WorldPaths[0].ScreenPos = WorldPaths[0].ScreenPos with { X = xPos };

        if (Engine.Settings.Platform == Platform.GBA)
            GameCubeSparkles.ScreenPos = GameCubeSparkles.ScreenPos with { X = xPos };

        if (Timer <= 120)
        {
            if (Timer == 120)
            {
                WorldPaths[0].CurrentAnimation = 0;
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__PathFX_Mix01);
                Scene.AnimationPlayer.Play(WorldPaths[0]);
            }

            Timer++;
        }
        else
        {
            if (WorldPaths[0].EndOfAnimation)
            {
                WorldPaths[0].CurrentAnimation = 3;
                CurrentExStepAction = StepEx_Play;
                GameInfo.PersistentInfo.PlayedWorld2Unlock = true;
            }

            Scene.AnimationPlayer.Play(WorldPaths[0]);
        }

        Scene.AnimationPlayer.Play(Rayman);

        if (Engine.Settings.Platform == Platform.GBA)
            Scene.AnimationPlayer.Play(GameCubeSparkles);

        StepSpikyBag();
        StepLightning();
        StepVolcanoGlow();
    }

    private void StepEx_UnlockWorld3()
    {
        float xPos = BaseObjPos.X - ScrollX;
        Rayman.ScreenPos = Rayman.ScreenPos with { X = xPos };
        WorldPaths[0].ScreenPos = WorldPaths[0].ScreenPos with { X = xPos };
        WorldPaths[1].ScreenPos = WorldPaths[1].ScreenPos with { X = xPos };

        if (Engine.Settings.Platform == Platform.GBA)
            GameCubeSparkles.ScreenPos = GameCubeSparkles.ScreenPos with { X = xPos };

        if (Timer <= 120)
        {
            if (Timer == 120)
            {
                WorldPaths[1].CurrentAnimation = 1;
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__PathFX_Mix01);
                Scene.AnimationPlayer.Play(WorldPaths[1]);
            }

            Timer++;
        }
        else
        {
            if (WorldPaths[1].EndOfAnimation)
            {
                WorldPaths[1].CurrentAnimation = 4;
                CurrentExStepAction = StepEx_Play;
                GameInfo.PersistentInfo.PlayedWorld3Unlock = true;
            }

            Scene.AnimationPlayer.Play(WorldPaths[1]);
        }

        Scene.AnimationPlayer.Play(Rayman);
        Scene.AnimationPlayer.Play(WorldPaths[0]);

        if (Engine.Settings.Platform == Platform.GBA)
            Scene.AnimationPlayer.Play(GameCubeSparkles);

        StepSpikyBag();
        StepLightning();
        StepVolcanoGlow();
    }

    private void StepEx_UnlockWorld4()
    {
        float xPos = BaseObjPos.X - ScrollX;
        Rayman.ScreenPos = Rayman.ScreenPos with { X = xPos };
        WorldPaths[0].ScreenPos = WorldPaths[0].ScreenPos with { X = xPos };
        WorldPaths[1].ScreenPos = WorldPaths[1].ScreenPos with { X = xPos };
        WorldPaths[2].ScreenPos = WorldPaths[2].ScreenPos with { X = xPos };

        if (Engine.Settings.Platform == Platform.GBA)
            GameCubeSparkles.ScreenPos = GameCubeSparkles.ScreenPos with { X = xPos };

        if (Timer <= 120)
        {
            if (Timer == 120)
            {
                WorldPaths[2].CurrentAnimation = 2;
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__PathFX_Mix01);
                Scene.AnimationPlayer.Play(WorldPaths[2]);
            }

            Timer++;
        }
        else
        {
            if (WorldPaths[2].EndOfAnimation)
            {
                WorldPaths[2].CurrentAnimation = 5;
                CurrentExStepAction = StepEx_Play;
                GameInfo.PersistentInfo.PlayedWorld4Unlock = true;
            }

            Scene.AnimationPlayer.Play(WorldPaths[2]);
        }

        Scene.AnimationPlayer.Play(Rayman);
        Scene.AnimationPlayer.Play(WorldPaths[0]);
        Scene.AnimationPlayer.Play(WorldPaths[1]);

        if (Engine.Settings.Platform == Platform.GBA)
            Scene.AnimationPlayer.Play(GameCubeSparkles);

        StepSpikyBag();
        StepLightning();
        StepVolcanoGlow();
    }

    private void StepEx_Play()
    {
        if (unk2 == 0 && CircleWipeFXMode == 0)
        {
            // Move forward
            if ((JoyPad.IsButtonJustPressed(GbaInput.Right) || 
                 (JoyPad.IsButtonJustPressed(GbaInput.Up) && WorldId is WorldId.World1 or WorldId.World3)) && 
                CurrentMovement == WorldMapMovement.None &&
                JoyPad.IsButtonReleased(GbaInput.Select))
            {
                switch (WorldId)
                {
                    case WorldId.World1:
                        if (GameInfo.PersistentInfo.UnlockedWorld2)
                        {
                            Rayman.CurrentAnimation = 0;
                            CurrentMovement = WorldMapMovement.World1To2;
                            UserInfo.WorldNameBar.CanMoveIn = false;
                            UserInfo.WorldNameBar.MoveOutWorldNameBar();
                        }
                        break;

                    case WorldId.World2:
                        if (GameInfo.PersistentInfo.UnlockedWorld3)
                        {
                            Rayman.CurrentAnimation = 2;
                            CurrentMovement = WorldMapMovement.World2To3;
                            UserInfo.WorldNameBar.CanMoveIn = false;
                            UserInfo.WorldNameBar.MoveOutWorldNameBar();
                        }
                        break;

                    case WorldId.World3:
                        if (GameInfo.PersistentInfo.UnlockedWorld4)
                        {
                            Rayman.CurrentAnimation = 6;
                            CurrentMovement = WorldMapMovement.World3To4;
                            UserInfo.WorldNameBar.CanMoveIn = false;
                            UserInfo.WorldNameBar.MoveOutWorldNameBar();
                        }
                        break;
                }
            }
            // Move back
            else if ((JoyPad.IsButtonJustPressed(GbaInput.Left) || 
                      (JoyPad.IsButtonJustPressed(GbaInput.Down) && WorldId == WorldId.World2) || 
                      (JoyPad.IsButtonJustPressed(GbaInput.Up) && WorldId == WorldId.World4)) && 
                     CurrentMovement == WorldMapMovement.None && 
                     JoyPad.IsButtonReleased(GbaInput.Select))
            {
                switch (WorldId)
                {
                    case WorldId.World1:
                        if (Engine.Settings.Platform == Platform.GBA)
                        {
                            Rayman.CurrentAnimation = 22;
                            CurrentMovement = WorldMapMovement.World1ToGameCube;
                            unk2 = 2;
                            UserInfo.WorldNameBar.CanMoveIn = false;
                            UserInfo.WorldNameBar.MoveOutWorldNameBar();
                        }
                        break;

                    case WorldId.World2:
                        Rayman.CurrentAnimation = 13;
                        CurrentMovement = WorldMapMovement.World2To1;
                        UserInfo.WorldNameBar.CanMoveIn = false;
                        UserInfo.WorldNameBar.MoveOutWorldNameBar();
                        break;

                    case WorldId.World3:
                        Rayman.CurrentAnimation = 10;
                        CurrentMovement = WorldMapMovement.World3To2;
                        UserInfo.WorldNameBar.CanMoveIn = false;
                        UserInfo.WorldNameBar.MoveOutWorldNameBar();
                        break;

                    case WorldId.World4:
                        Rayman.CurrentAnimation = 8;
                        CurrentMovement = WorldMapMovement.World4To3;
                        UserInfo.WorldNameBar.CanMoveIn = false;
                        UserInfo.WorldNameBar.MoveOutWorldNameBar();
                        break;
                }
            }
            // Select world
            else if (JoyPad.IsButtonJustPressed(GbaInput.A) &&
                     CurrentMovement == WorldMapMovement.None &&
                     JoyPad.IsButtonReleased(GbaInput.Select))
            {
                unk2 = 2;
                CircleWipeFXMode = 1;
                // windowCircleWipeFX->field1_0x4 = 1; // TODO: Implement
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Spirale_Mix01);
            }
        }

        Vector2 camDelta = Vector2.Zero;

        switch (CurrentMovement)
        {
            case WorldMapMovement.World1To2:
                if (Rayman.CurrentAnimation == 0)
                {
                    if (Rayman.EndOfAnimation)
                        Rayman.CurrentAnimation = 1;
                }
                else
                {
                    if (Rayman.EndOfAnimation)
                    {
                        CurrentMovement = WorldMapMovement.None;
                        WorldId = WorldId.World2;
                        UserInfo.WorldNameBar.SetWorld(WorldId);
                        UserInfo.WorldNameBar.CanMoveIn = true;
                        UserInfo.WorldNameBar.MoveInWorldNameBar();
                        Rayman.CurrentAnimation = 16;
                    }
                }

                if ((Engine.Settings.Platform == Platform.GBA && ScrollX < 128) ||
                    (Engine.Settings.Platform == Platform.NGage && ScrollX < 72))
                    camDelta = new Vector2(1, 0);
                break;

            case WorldMapMovement.World2To3:
                if (Rayman.CurrentAnimation == 2 && Rayman.EndOfAnimation)
                    Rayman.CurrentAnimation = 3;

                if (Rayman.CurrentAnimation == 3 && Rayman.EndOfAnimation)
                    Rayman.CurrentAnimation = 4;

                if (Rayman.CurrentAnimation == 4)
                {
                    if (Rayman.EndOfAnimation)
                        Rayman.CurrentAnimation = 5;
                }
                else
                {
                    if (Rayman.EndOfAnimation)
                    {
                        CurrentMovement = WorldMapMovement.None;
                        WorldId = WorldId.World3;
                        UserInfo.WorldNameBar.SetWorld(WorldId);
                        UserInfo.WorldNameBar.CanMoveIn = true;
                        UserInfo.WorldNameBar.MoveInWorldNameBar();
                        Rayman.CurrentAnimation = 17;
                    }
                }

                if (Rayman.CurrentAnimation != 2)
                    camDelta = new Vector2(1, 0);
                break;

            case WorldMapMovement.World3To4:
                if (Rayman.CurrentAnimation == 6)
                {
                    if (Rayman.EndOfAnimation)
                        Rayman.CurrentAnimation = 7;
                }
                else
                {
                    if (Rayman.EndOfAnimation)
                    {
                        CurrentMovement = WorldMapMovement.None;
                        WorldId = WorldId.World4;
                        UserInfo.WorldNameBar.SetWorld(WorldId);
                        UserInfo.WorldNameBar.CanMoveIn = true;
                        UserInfo.WorldNameBar.MoveInWorldNameBar();
                        Rayman.CurrentAnimation = 18;
                    }
                }
                break;

            case WorldMapMovement.World4To3:
                if (Rayman.CurrentAnimation == 8)
                {
                    if (Rayman.EndOfAnimation)
                        Rayman.CurrentAnimation = 9;
                }
                else
                {
                    if (Rayman.EndOfAnimation)
                    {
                        CurrentMovement = WorldMapMovement.None;
                        WorldId = WorldId.World3;
                        UserInfo.WorldNameBar.SetWorld(WorldId);
                        UserInfo.WorldNameBar.CanMoveIn = true;
                        UserInfo.WorldNameBar.MoveInWorldNameBar();
                        Rayman.CurrentAnimation = 17;
                    }
                }
                break;

            case WorldMapMovement.World3To2:
                if (Rayman.CurrentAnimation == 10 && Rayman.EndOfAnimation)
                    Rayman.CurrentAnimation = 11;

                if (Rayman.CurrentAnimation == 11)
                {
                    if (Rayman.EndOfAnimation)
                        Rayman.CurrentAnimation = 12;
                }
                else
                {
                    if (Rayman.EndOfAnimation)
                    {
                        CurrentMovement = WorldMapMovement.None;
                        WorldId = WorldId.World2;
                        UserInfo.WorldNameBar.SetWorld(WorldId);
                        UserInfo.WorldNameBar.CanMoveIn = true;
                        UserInfo.WorldNameBar.MoveInWorldNameBar();
                        Rayman.CurrentAnimation = 16;
                    }
                }

                if ((Engine.Settings.Platform == Platform.GBA && ScrollX > 128) ||
                    (Engine.Settings.Platform == Platform.NGage && ScrollX > 72))
                    camDelta = new Vector2(-1, 0);
                break;

            case WorldMapMovement.World2To1:
                if (Rayman.CurrentAnimation == 13 && Rayman.EndOfAnimation)
                    Rayman.CurrentAnimation = 14;

                if (Rayman.CurrentAnimation == 14)
                {
                    if (Rayman.EndOfAnimation)
                        Rayman.CurrentAnimation = 15;
                }
                else
                {
                    if (Rayman.EndOfAnimation)
                    {
                        CurrentMovement = WorldMapMovement.None;
                        WorldId = WorldId.World1;
                        UserInfo.WorldNameBar.SetWorld(WorldId);
                        UserInfo.WorldNameBar.CanMoveIn = true;
                        UserInfo.WorldNameBar.MoveInWorldNameBar();
                        Rayman.CurrentAnimation = 15;
                    }
                }

                camDelta = new Vector2(-1, 0);
                break;

            case WorldMapMovement.World1ToGameCube:
                camDelta = new Vector2(-1, 0);
                break;
        }

        TgxCamera2D cam = ((TgxPlayfield2D)Scene.Playfield).Camera;
        TgxCluster mainCluster = cam.GetMainCluster();
        if ((camDelta.X > 0 && !mainCluster.IsOnLimit(Edge.Right)) || 
            (camDelta.X < 0 && !mainCluster.IsOnLimit(Edge.Left)))
        {
            if (Engine.Settings.Platform == Platform.NGage)
            {
                if (NGageScrollCooldown < 1)
                {
                    cam.Position += camDelta;
                    ScrollX += camDelta.X;
                    NGageScrollCooldown = 1;
                }
                else
                {
                    NGageScrollCooldown--;
                }
            }
            else
            {
                cam.Position += camDelta;
                ScrollX += camDelta.X;
            }
        }

        float xPos = BaseObjPos.X - ScrollX;

        Rayman.ScreenPos = Rayman.ScreenPos with { X = xPos };

        foreach (AnimatedObject worldPath in WorldPaths)
            worldPath.ScreenPos = worldPath.ScreenPos with { X = xPos };

        if (Engine.Settings.Platform == Platform.GBA)
            GameCubeSparkles.ScreenPos = GameCubeSparkles.ScreenPos with { X = xPos };

        if (unk2 != 1)
            Scene.AnimationPlayer.Play(Rayman);

        Scene.AnimationPlayer.Play(WorldPaths[0]);

        if (GameInfo.PersistentInfo.UnlockedWorld3)
            Scene.AnimationPlayer.Play(WorldPaths[1]);

        if (GameInfo.PersistentInfo.UnlockedWorld4)
            Scene.AnimationPlayer.Play(WorldPaths[2]);
        
        if (Engine.Settings.Platform == Platform.GBA)
            Scene.AnimationPlayer.Play(GameCubeSparkles);

        StepSpikyBag();
        StepLightning();
        StepVolcanoGlow();

        // TODO: Manage loading world or gamecube menu

        ManageCheats();
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

    #region Data Types

    public enum WorldMapMovement
    {
        None = 0,
        World1To2 = 1,
        World2To3 = 2,
        World3To4 = 3,
        World4To3 = 4,
        World3To2 = 5,
        World2To1 = 6,
        World1ToGameCube = 7,
    }

    #endregion
}