﻿using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.TgxEngine;
using Action = System.Action;

namespace GbaMonoGame.Rayman3;

public class Intro : Frame, IHasPlayfield
{
    #region Public Properties

    private AnimationPlayer AnimationPlayer { get; set; }
    private TgxPlayfield2D Playfield { get; set; }

    private Action CurrentStepAction { get; set; }
    private MenuAll Menu { get; set; }
    private AnimatedObject PressStartObj { get; set; }
    private AnimatedObject GameloftLogoObj { get; set; }
    private AnimatedObject BlackLumAndLogoObj { get; set; }
    private int Timer { get; set; }
    private int AlphaTimer { get; set; }
    private int ScrollY { get; set; }
    private bool IsSkipping { get; set; }
    private PaletteFadeEffectObject PaletteFade { get; set; }
    private int PaletteFadeTimer { get; set; }
    private int SkippedTimer { get; set; }

    #endregion

    #region Interface Properties

    TgxPlayfield IHasPlayfield.Playfield => Playfield;

    #endregion

    #region Private Methods

    private void LoadAnimations()
    {
        AnimatedObjectResource introAnimResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.IntroAnimations);

        PressStartObj = new AnimatedObject(introAnimResource, false)
        {
            IsFramed = true,
            SpritePriority = 0,
            YPriority = 0,
            ScreenPos = Engine.Settings.Platform switch
            {
                Platform.GBA => new Vector2(120, 150),
                Platform.NGage => new Vector2(88, 150),
                _ => throw new UnsupportedPlatformException(),
            },
            CurrentAnimation = 9 + Localization.LanguageUiIndex
        };

        if (Engine.Settings.Platform == Platform.NGage)
        {
            GameloftLogoObj = new AnimatedObject(introAnimResource, false)
            {
                IsFramed = true,
                SpritePriority = 0,
                YPriority = 0,
                ScreenPos = new Vector2(88, 208),
                CurrentAnimation = 23
            };
        }

        BlackLumAndLogoObj = new AnimatedObject(introAnimResource, false)
        {
            IsFramed = true,
            SpritePriority = 0,
            YPriority = 0,
            ScreenPos = Engine.Settings.Platform switch
            {
                Platform.GBA => new Vector2(120, 128),
                Platform.NGage => new Vector2(88, 128),
                _ => throw new UnsupportedPlatformException(),
            },
            CurrentAnimation = 0
        };
    }

    private void LoadPlayfield()
    {
        PlayfieldResource introPlayfield = Storage.LoadResource<PlayfieldResource>(GameResource.IntroPlayfield);
        Playfield = TgxPlayfield.Load<TgxPlayfield2D>(introPlayfield);
        Engine.GameViewPort.SetResolutionBoundsToOriginalResolution();
        Playfield.Camera.FixedResolution = true;

        Playfield.Camera.Position = Vector2.Zero;
        Playfield.Step();

        if (Engine.Settings.Platform == Platform.GBA)
        {
            Playfield.TileLayers[0].Screen.IsEnabled = false;
            Playfield.TileLayers[1].Screen.IsEnabled = false;
            Playfield.TileLayers[3].Screen.IsEnabled = false;
        }
        else if (Engine.Settings.Platform == Platform.NGage)
        {
            Playfield.TileLayers[0].Screen.IsEnabled = true;
            Playfield.TileLayers[1].Screen.IsEnabled = true;
            Playfield.TileLayers[2].Screen.IsEnabled = false;
            Playfield.TileLayers[3].Screen.IsEnabled = false;
        }
        else
        {
            throw new UnsupportedPlatformException();
        }

        // TODO: Allow scrolling on N-Gage too?
        if (Engine.Settings.Platform == Platform.GBA)
            Playfield.TileLayers[3].Screen.Renderer = new IntroCloudsRenderer(((TextureScreenRenderer)Playfield.TileLayers[3].Screen.Renderer).Texture);
    }

    private void Skip()
    {
        // NOTE: We might be 1 game frame off here since the game uses GameTime.ElapsedFrames to alternate between fading
        //       the background and object palettes, with only the object palette fading checking if finished.
        PaletteFade.SetFadeFromTimer(PaletteFadeTimer);
        if (PaletteFadeTimer == PaletteFade.MinFadeTime)
        {
            CurrentStepAction = Step_Skip_1;
            SkippedTimer = 0;
        }
        else
        {
            PaletteFadeTimer--;
        }
        AnimationPlayer.PlayFront(PaletteFade);
    }

    #endregion

    #region Pubic Override Methods

    public override void Init()
    {
        SoundEngineInterface.SetNbVoices(10);

        // Pre-load the menu
        Menu = new MenuAll(Engine.Settings.Platform switch
        {
            Platform.GBA => MenuAll.Page.SelectLanguage,
            Platform.NGage => MenuAll.Page.NGage,
            _ => throw new UnsupportedPlatformException(),
        });
        Menu.LoadGameInfo();

        AnimationPlayer = new AnimationPlayer(true, SoundEventsManager.ProcessEvent);

        LoadAnimations();
        LoadPlayfield();

        PaletteFade = new PaletteFadeEffectObject()
        {
            SpritePriority = 0,
        };

        CurrentStepAction = Step_1;

        AlphaTimer = 0;
        Timer = 0;
        ScrollY = 0;
        IsSkipping = false;
        PaletteFadeTimer = PaletteFade.MaxFadeTime;

        SoundEventsManager.SetVolumeForType(SoundType.Music, 0);
        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__sadslide);
    }

    public override void UnInit()
    {
        Playfield.UnInit();
        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__sadslide);
        Gfx.SetFullFade();
        SoundEventsManager.SetVolumeForType(SoundType.Music, SoundEngineInterface.MaxVolume);
    }

    public override void Step()
    {
        Playfield.Step();
        AnimationPlayer.Execute();

        // Fade in music
        if (GameTime.ElapsedFrames <= 64)
            SoundEventsManager.SetVolumeForType(SoundType.Music, GameTime.ElapsedFrames * 2);

        CurrentStepAction();
    }

    #endregion

    #region Steps

    private void Step_1()
    {
        if (Engine.Settings.Platform == Platform.GBA)
        {
            Timer++;

            if (Timer > 60)
            {
                Timer = 0;
                CurrentStepAction = Step_2;

                Playfield.TileLayers[0].Screen.IsAlphaBlendEnabled = true;
                Playfield.TileLayers[0].Screen.GbaAlpha = 0;

                Playfield.TileLayers[0].Screen.IsEnabled = true;
                Playfield.TileLayers[1].Screen.IsEnabled = true;
            }
        }
        else if (Engine.Settings.Platform == Platform.NGage)
        {
            CurrentStepAction = Step_2;
        }
        else
        {
            throw new UnsupportedPlatformException();
        }
    }

    private void Step_2()
    {
        if (Engine.Settings.Platform == Platform.GBA)
        {
            Timer++;

            if (Timer >= 68)
            {
                Timer = 0;
                CurrentStepAction = Step_3;

                Playfield.TileLayers[0].Screen.IsAlphaBlendEnabled = false;
                Playfield.TileLayers[2].Screen.IsAlphaBlendEnabled = true;
                Playfield.TileLayers[2].Screen.GbaAlpha = 16;
            }
            else
            {
                Playfield.TileLayers[0].Screen.GbaAlpha = Timer / 4f;
            }
        }
        else if (Engine.Settings.Platform == Platform.NGage)
        {
            CurrentStepAction = Step_3;
        }
        else
        {
            throw new UnsupportedPlatformException();
        }
    }

    private void Step_3()
    {
        Timer++;

        if (Timer >= 68)
        {
            Timer = 0;
            AlphaTimer = 0;
            CurrentStepAction = Step_4;

            Playfield.TileLayers[2].Screen.IsAlphaBlendEnabled = false;
            Playfield.TileLayers[3].Screen.IsAlphaBlendEnabled = true;
            Playfield.TileLayers[3].Screen.GbaAlpha = 0;

            Playfield.TileLayers[2].Screen.IsEnabled = false;

            // Only show clouds on GBA
            if (Engine.Settings.Platform == Platform.GBA)
                Playfield.TileLayers[3].Screen.IsEnabled = true;
        }
        else
        {
            Playfield.TileLayers[2].Screen.GbaAlpha = 16 - (Timer / 4f);
        }

        // N-Gage allows the intro to be skipped from here
        if (Engine.Settings.Platform == Platform.NGage)
        {
            // TODO: Check every button input
            if (JoyPad.IsButtonPressed(GbaInput.Start))
                IsSkipping = true;

            if (IsSkipping)
                Skip();
        }
    }

    private void Step_4()
    {
        // Get the main cluster
        TgxCluster mainCluster = Playfield.Camera.GetMainCluster();

        // Scroll if we haven't yet reached the bottom
        if (!mainCluster.IsOnLimit(Edge.Bottom))
        {
            ScrollY++;
            Playfield.Camera.Position += new Vector2(0, 1);
            Gfx.GetScreen(3).Offset = new Vector2(0, ScrollY);
        }

        if (ScrollY > 600 && AlphaTimer <= 0x80)
        {
            Playfield.TileLayers[3].Screen.GbaAlpha = AlphaTimer / 32f;
            AlphaTimer++;
        }

        if (ScrollY > 175)
        {
            if (BlackLumAndLogoObj.EndOfAnimation)
            {
                if (BlackLumAndLogoObj.CurrentAnimation < 4)
                {
                    BlackLumAndLogoObj.CurrentAnimation++;
                    BlackLumAndLogoObj.ScreenPos = Engine.Settings.Platform switch
                    {
                        Platform.GBA => new Vector2(120, 128),
                        Platform.NGage => new Vector2(88, 128),
                        _ => throw new UnsupportedPlatformException(),
                    };
                }
                else
                {
                    BlackLumAndLogoObj.CurrentAnimation = 6;
                    BlackLumAndLogoObj.ScreenPos = Engine.Settings.Platform switch
                    {
                        Platform.GBA => new Vector2(120, 70),
                        Platform.NGage => new Vector2(88, 70),
                        _ => throw new UnsupportedPlatformException(),
                    };
                }
            }

            if (BlackLumAndLogoObj.CurrentAnimation < 5)
            {
                BlackLumAndLogoObj.ScreenPos -= new Vector2(0, 1); // NOTE: Game moves by 2 every second frame
                Timer++;

                BlackLumAndLogoObj.FrameChannelSprite();
                AnimationPlayer.PlayFront(BlackLumAndLogoObj);
            }
        }

        if (Engine.Settings.Platform == Platform.GBA)
        {
            if (JoyPad.IsButtonPressed(GbaInput.Start) && ScrollY <= 863)
                IsSkipping = true;
        }
        else if (Engine.Settings.Platform == Platform.NGage)
        {
            // TODO: Check every button input
            if (JoyPad.IsButtonPressed(GbaInput.Start))
                IsSkipping = true;
        }
        else
        {
            throw new UnsupportedPlatformException();
        }

        if (IsSkipping)
            Skip();
        else if (mainCluster.IsOnLimit(Edge.Bottom))
            CurrentStepAction = Step_5;
    }

    private void Step_5()
    {
        if (BlackLumAndLogoObj.EndOfAnimation)
        {
            if (BlackLumAndLogoObj.CurrentAnimation == 7)
            {
                BlackLumAndLogoObj.CurrentAnimation = 8;
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__raytheme__After__sadslide);
                CurrentStepAction = Step_6;
            }
            else
            {
                BlackLumAndLogoObj.CurrentAnimation++;
            }
        }

        BlackLumAndLogoObj.FrameChannelSprite();
        AnimationPlayer.PlayFront(BlackLumAndLogoObj);
    }

    private void Step_6()
    {
        if ((GameTime.ElapsedFrames & 0x10) != 0)
            AnimationPlayer.PlayFront(PressStartObj);

        if (Engine.Settings.Platform == Platform.NGage)
            AnimationPlayer.PlayFront(GameloftLogoObj);

        BlackLumAndLogoObj.FrameChannelSprite();
        AnimationPlayer.PlayFront(BlackLumAndLogoObj);

        // TODO: Check every button input on N-Gage
        if (JoyPad.IsButtonPressed(GbaInput.Start))
        {
            Gfx.SetFullFade();
            FrameManager.SetNextFrame(Menu);
            Random.SetSeed(GameTime.ElapsedFrames);
        }
    }

    private void Step_Skip_1()
    {
        SkippedTimer++;

        if (SkippedTimer == 10)
        {
            AlphaTimer = 0x80;
            Playfield.TileLayers[3].Screen.GbaAlpha = AlphaTimer / 32f;
            BlackLumAndLogoObj.CurrentAnimation = 8;
            BlackLumAndLogoObj.CurrentFrame = 5;
            BlackLumAndLogoObj.ScreenPos = Engine.Settings.Platform switch
            {
                Platform.GBA => new Vector2(120, 70),
                Platform.NGage => new Vector2(88, 70),
                _ => throw new UnsupportedPlatformException(),
            };

            Playfield.Camera.Position = new Vector2(0, 880);
            ScrollY = 879;
        }
        else if (SkippedTimer == 20)
        {
            CurrentStepAction = Step_Skip_2;
        }

        AnimationPlayer.PlayFront(PaletteFade);
    }

    private void Step_Skip_2()
    {
        // NOTE: We might be 1 game frame off here since the game uses GameTime.ElapsedFrames to alternate between fading
        //       the background and object palettes, with only the object palette fading checking if finished.
        PaletteFade.SetFadeFromTimer(PaletteFadeTimer);
        PaletteFadeTimer++;
        if (PaletteFadeTimer == PaletteFade.MaxFadeTime - 2)
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__raytheme__After__sadslide);
        else if (PaletteFadeTimer > PaletteFade.MaxFadeTime)
            CurrentStepAction = Step_6;

        if ((GameTime.ElapsedFrames & 0x10) != 0)
            AnimationPlayer.PlayFront(PressStartObj);

        if (Engine.Settings.Platform == Platform.NGage)
            AnimationPlayer.PlayFront(GameloftLogoObj);

        BlackLumAndLogoObj.FrameChannelSprite();
        AnimationPlayer.PlayFront(BlackLumAndLogoObj);

        AnimationPlayer.PlayFront(PaletteFade);
    }

    #endregion
}