using System;
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

// TODO: Implement palette fading and alpha effects
public class Intro : Frame
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
    private AnimatedObject BlackLumAndLogoObj { get; set; }
    private int Timer { get; set; }
    private int ScrollY { get; set; }
    private bool IsSkipping { get; set; }
    private int FadeTime { get; set; }
    private int SkippedTimer { get; set; }

    #endregion

    #region Private Methods

    private void PreLoadMenu()
    {
        Menu = new MenuAll(MenuAll.Page.Language);
        // TODO: Create GameInfo
        // TODO: Load GameInfo
        // TODO: Load data
    }

    private void LoadAnimations()
    {
        AnimatedObjectResource introAnimResource = Storage.LoadResource<AnimatedObjectResource>(0x76);

        PressStartObj = new AnimatedObject(introAnimResource, false)
        {
            Priority = 0,
            ScreenPos = new Vector2(0x78, 0x96)
        };

        PressStartObj.SetCurrentAnimation(9);

        // TODO: Game uses a different type of AnimatedObject here which seems to have additional support for not showing off-screen channels?
        BlackLumAndLogoObj = new AnimatedObject(introAnimResource, false)
        {
            Priority = 0,
            ScreenPos = new Vector2(0x78, 0x80)
        };

        BlackLumAndLogoObj.SetCurrentAnimation(0);
    }

    private void LoadPlayfield()
    {
        PlayfieldResource introPlayfield = Storage.LoadResource<PlayfieldResource>(0x75);
        Playfield = TgxPlayfield.Load<TgxPlayfield2D>(introPlayfield);

        Playfield.TileLayers[0].Screen.IsEnabled = false;
        Playfield.TileLayers[1].Screen.IsEnabled = false;
        Playfield.TileLayers[3].Screen.IsEnabled = false;

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
        Gfx.Clear();

        PreLoadMenu();

        AnimationPlayer = new AnimationPlayer(true);

        LoadAnimations();
        LoadPlayfield();

        CurrentStepAction = Step_1;

        Timer = 0;
        ScrollY = 0;
        IsSkipping = false;
        FadeTime = MaxFadeTime;
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
        Timer++;

        if (Timer > 0x3C)
        {
            Timer = 0;
            CurrentStepAction = Step_2;

            Playfield.TileLayers[0].Screen.IsEnabled = true;
            Playfield.TileLayers[1].Screen.IsEnabled = true;
        }
    }

    private void Step_2()
    {
        Timer++;

        if (Timer > (0x10 * 4))
        {
            Timer = 0;
            CurrentStepAction = Step_3;
        }
    }

    private void Step_3()
    {
        Timer++;

        if (Timer > (0x10 * 4))
        {
            Timer = 0;
            CurrentStepAction = Step_4;

            Playfield.TileLayers[2].Screen.IsEnabled = false;
            Playfield.TileLayers[3].Screen.IsEnabled = true;
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
            Playfield.Camera.Move(new Vector2(0, 1));
            Gfx.Screens[3].Offset = new Vector2(0, ScrollY);
        }

        if (ScrollY > 0xAF)
        {
            if (BlackLumAndLogoObj.EndOfAnimation)
            {
                if (BlackLumAndLogoObj.AnimationIndex < 4)
                {
                    BlackLumAndLogoObj.SetCurrentAnimation(BlackLumAndLogoObj.AnimationIndex + 1);
                    BlackLumAndLogoObj.ScreenPos = new Vector2(0x78, 0x80);
                }
                else
                {
                    BlackLumAndLogoObj.SetCurrentAnimation(6);
                    BlackLumAndLogoObj.ScreenPos = new Vector2(0x78, 0x46);
                }
            }

            if (BlackLumAndLogoObj.AnimationIndex < 5)
            {
                if ((Timer & 1) != 0)
                {
                    BlackLumAndLogoObj.ScreenPos += new Vector2(0, -2);
                }
                Timer++;
                AnimationPlayer.AddObject1(BlackLumAndLogoObj);
            }
        }

        if (JoyPad.Check(Input.Start) && ScrollY <= 0x35f)
            IsSkipping = true;

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
                CurrentStepAction = Step_6;
            }
            else
            {
                BlackLumAndLogoObj.SetCurrentAnimation(BlackLumAndLogoObj.AnimationIndex + 1);
            }
        }

        AnimationPlayer.AddObject1(BlackLumAndLogoObj);
    }

    private void Step_6()
    {
        if ((GameTime.ElapsedFrames & 0x10) != 0)
            AnimationPlayer.AddObject1(PressStartObj);

        AnimationPlayer.AddObject1(BlackLumAndLogoObj);

        if (JoyPad.Check(Input.Start))
            FrameManager.SetNextFrame(Menu);
    }

    private void Step_Skip_1()
    {
        SkippedTimer++;

        if (SkippedTimer == 10)
        {
            BlackLumAndLogoObj.SetCurrentAnimation(8);
            BlackLumAndLogoObj.SetCurrentFrame(5);
            BlackLumAndLogoObj.ScreenPos = new Vector2(0x78, 0x46);

            // TODO: This should be changed to use the camera
            Gfx.Screens[1].Offset = new Vector2(0, 880);
            Gfx.Screens[3].Offset = new Vector2(0, 880);
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
            AnimationPlayer.AddObject1(PressStartObj);

        AnimationPlayer.AddObject1(BlackLumAndLogoObj);
    }

    #endregion
}