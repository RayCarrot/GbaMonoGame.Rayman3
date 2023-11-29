using BinarySerializer.Onyx.Gba;
using BinarySerializer.Onyx.Gba.Rayman3;
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.TgxEngine;
using Action = System.Action;

namespace OnyxCs.Gba.Rayman3;

// TODO: Implement palette fading and alpha effects
public class Intro : Frame, IHasPlayfield
{
    #region Private Constant Fields

    private const int MinFadeTime = 0;
    private const int MaxFadeTime = 16;

    #endregion

    #region Public Properties

    private AnimationPlayer AnimationPlayer { get; set; }
    private TgxPlayfield2D Playfield { get; set; }

    private Action CurrentStepAction { get; set; }
    private MenuAll Menu { get; set; }
    private AnimatedObject PressStartObj { get; set; }
    private AnimatedObject GameloftLogoObj { get; set; }
    private AnimatedObject BlackLumAndLogoObj { get; set; }
    private int Timer { get; set; }
    private int ScrollY { get; set; }
    private bool IsSkipping { get; set; }
    private int FadeTime { get; set; }
    private int SkippedTimer { get; set; }

    #endregion

    #region Interface Properties

    TgxPlayfield IHasPlayfield.Playfield => Playfield;

    #endregion

    #region Private Methods

    private void PreLoadMenu()
    {
        Menu = new MenuAll(Engine.Settings.Platform switch
        {
            Platform.GBA => MenuAll.Page.SelectLanguage,
            Platform.NGage => MenuAll.Page.NGage,
            _ => throw new UnsupportedPlatformException(),
        });
        Menu.LoadGameInfo();
    }

    private void LoadAnimations()
    {
        AnimatedObjectResource introAnimResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.IntroAnimations);

        PressStartObj = new AnimatedObject(introAnimResource, false)
        {
            Priority = 0,
            ScreenPos = Engine.Settings.Platform switch
            {
                Platform.GBA => new Vector2(120, 150),
                Platform.NGage => new Vector2(88, 150),
                _ => throw new UnsupportedPlatformException(),
            }
        };

        PressStartObj.SetCurrentAnimation(9 + Localization.LanguageUiIndex);

        if (Engine.Settings.Platform == Platform.NGage)
        {
            GameloftLogoObj = new AnimatedObject(introAnimResource, false)
            {
                Priority = 0,
                ScreenPos = new Vector2(88, 208)
            };

            GameloftLogoObj.SetCurrentAnimation(23);
        }

        BlackLumAndLogoObj = new AnimatedObject(introAnimResource, false)
        {
            Priority = 0,
            ScreenPos = Engine.Settings.Platform switch
            {
                Platform.GBA => new Vector2(120, 128),
                Platform.NGage => new Vector2(88, 128),
                _ => throw new UnsupportedPlatformException(),
            }
        };

        BlackLumAndLogoObj.SetCurrentAnimation(0);
    }

    private void LoadPlayfield()
    {
        PlayfieldResource introPlayfield = Storage.LoadResource<PlayfieldResource>(GameResource.IntroPlayfield);
        Playfield = TgxPlayfield.Load<TgxPlayfield2D>(introPlayfield);

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

    //private void FadePalette(Palette pal)
    //{
    //    for (int i = 0; i < pal.Colors.Length; i++)
    //    {
    //        pal.Colors[i] = new Color(
    //            pal.PaletteResource.Colors[i].Red * (FadeTime / (float)MaxFadeTime),
    //            pal.PaletteResource.Colors[i].Green * (FadeTime / (float)MaxFadeTime),
    //            pal.PaletteResource.Colors[i].Blue * (FadeTime / (float)MaxFadeTime));
    //    }

    //    pal.IsDirty = true;
    //}

    private void Skip()
    {
        if ((GameTime.ElapsedFrames & 1) == 0)
        {
            //foreach (Palette pal in Engine.Vram.GetSpritePalettes())
            //    FadePalette(pal);

            if (FadeTime == MinFadeTime)
            {
                CurrentStepAction = Step_Skip_1;
                SkippedTimer = 0;
            }
        }
        else
        {
            FadeTime--;

            //foreach (Palette pal in Engine.Vram.GetBackgroundPalettes())
            //    FadePalette(pal);
        }
    }

    #endregion

    #region Pubic Override Methods

    public override void Init()
    {
        PreLoadMenu();

        AnimationPlayer = new AnimationPlayer(true);

        LoadAnimations();
        LoadPlayfield();

        CurrentStepAction = Step_1;

        Timer = 0;
        ScrollY = 0;
        IsSkipping = false;
        FadeTime = MaxFadeTime;

        SoundManager.Play(Rayman3SoundEvent.Play__sadslide);
    }

    public override void UnInit()
    {
        SoundManager.Play(Rayman3SoundEvent.Stop__sadslide);
    }

    public override void Step()
    {
        // TODO: Update animated tiles
        AnimationPlayer.Execute();

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

            if (Timer > 64)
            {
                Timer = 0;
                CurrentStepAction = Step_3;
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
        if (Engine.Settings.Platform == Platform.GBA)
        {
            Timer++;

            if (Timer == 64)
            {
                Timer = 0;
                CurrentStepAction = Step_4;

                Playfield.TileLayers[2].Screen.IsEnabled = false;
                Playfield.TileLayers[3].Screen.IsEnabled = true;
            }
        }
        else if (Engine.Settings.Platform == Platform.NGage)
        {
            Timer++;

            if (Timer == 64)
            {
                Timer = 0;
                CurrentStepAction = Step_4;

                Playfield.TileLayers[2].Screen.IsEnabled = false;
            }

            // TODO: Check every button input
            if (JoyPad.Check(GbaInput.Start))
                IsSkipping = true;

            if (IsSkipping)
                Skip();
        }
        else
        {
            throw new UnsupportedPlatformException();
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

        if (ScrollY > 175)
        {
            if (BlackLumAndLogoObj.EndOfAnimation)
            {
                if (BlackLumAndLogoObj.AnimationIndex < 4)
                {
                    BlackLumAndLogoObj.SetCurrentAnimation(BlackLumAndLogoObj.AnimationIndex + 1);
                    BlackLumAndLogoObj.ScreenPos = Engine.Settings.Platform switch
                    {
                        Platform.GBA => new Vector2(120, 128),
                        Platform.NGage => new Vector2(88, 128),
                        _ => throw new UnsupportedPlatformException(),
                    };
                }
                else
                {
                    BlackLumAndLogoObj.SetCurrentAnimation(6);
                    BlackLumAndLogoObj.ScreenPos = Engine.Settings.Platform switch
                    {
                        Platform.GBA => new Vector2(120, 70),
                        Platform.NGage => new Vector2(88, 70),
                        _ => throw new UnsupportedPlatformException(),
                    };
                }
            }

            if (BlackLumAndLogoObj.AnimationIndex < 5)
            {
                if ((Timer & 1) != 0)
                {
                    BlackLumAndLogoObj.ScreenPos += new Vector2(0, -2);
                }
                Timer++;
                AnimationPlayer.AddPrimaryObject(BlackLumAndLogoObj);
            }
        }

        if (Engine.Settings.Platform == Platform.GBA)
        {
            if (JoyPad.Check(GbaInput.Start) && ScrollY <= 863)
                IsSkipping = true;
        }
        else if (Engine.Settings.Platform == Platform.NGage)
        {
            // TODO: Check every button input
            if (JoyPad.Check(GbaInput.Start))
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
            if (BlackLumAndLogoObj.AnimationIndex == 7)
            {
                BlackLumAndLogoObj.SetCurrentAnimation(8);
                SoundManager.Play(Rayman3SoundEvent.Play__raytheme__After__sadslide);
                CurrentStepAction = Step_6;
            }
            else
            {
                BlackLumAndLogoObj.SetCurrentAnimation(BlackLumAndLogoObj.AnimationIndex + 1);
            }
        }

        AnimationPlayer.AddPrimaryObject(BlackLumAndLogoObj);
    }

    private void Step_6()
    {
        if ((GameTime.ElapsedFrames & 0x10) != 0)
            AnimationPlayer.AddPrimaryObject(PressStartObj);

        if (Engine.Settings.Platform == Platform.NGage)
            AnimationPlayer.AddPrimaryObject(GameloftLogoObj);

        AnimationPlayer.AddPrimaryObject(BlackLumAndLogoObj);

        // TODO: Check every button input on N-Gage
        if (JoyPad.Check(GbaInput.Start))
            FrameManager.SetNextFrame(Menu);
    }

    private void Step_Skip_1()
    {
        SkippedTimer++;

        if (SkippedTimer == 10)
        {
            BlackLumAndLogoObj.SetCurrentAnimation(8);
            BlackLumAndLogoObj.SetCurrentFrame(5);
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
    }

    private void Step_Skip_2()
    {
        if ((GameTime.ElapsedFrames & 1) == 0)
        {
            if (FadeTime < MaxFadeTime + 1)
            {
                //foreach (Palette pal in Engine.Vram.GetBackgroundPalettes())
                //    FadePalette(pal);
            }

            if (FadeTime == MaxFadeTime)
                SoundManager.Play(Rayman3SoundEvent.Play__raytheme__After__sadslide);

            if (FadeTime == MaxFadeTime + 1)
                CurrentStepAction = Step_6;
        }
        else
        {
            FadeTime++;

            //foreach (Palette pal in Engine.Vram.GetSpritePalettes())
            //    FadePalette(pal);
        }

        if ((GameTime.ElapsedFrames & 0x10) != 0)
            AnimationPlayer.AddPrimaryObject(PressStartObj);

        if (Engine.Settings.Platform == Platform.NGage)
            AnimationPlayer.AddPrimaryObject(GameloftLogoObj);

        AnimationPlayer.AddPrimaryObject(BlackLumAndLogoObj);
    }

    #endregion
}