#nullable disable // TODO: Would be nice to be able to enable it, but doesn't work well here
using System;
using System.Diagnostics;
using BinarySerializer.Onyx.Gba;
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.Sdk;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public class Intro : Frame
{
    #region Private Constant Fields

    private const int MinFadeTime = 0;
    private const int MaxFadeTime = 16;

    #endregion

    #region Public Properties

    public FrameMngr FrameMngr { get; set; }
    public Action CurrentStepAction { get; set; }
    public MenuAll Menu { get; set; }
    public AnimatedObject PressStartObj { get; set; }
    public AnimatedObject BlackLumAndLogoObj { get; set; }
    public int Timer { get; set; }
    public int ScrollY { get; set; }
    public bool IsSkipping { get; set; }
    public int FadeTime { get; set; }
    public int SkippedTimer { get; set; }

    #endregion

    #region Private Methods

    private void FadePalette(Palette pal)
    {
        for (int i = 0; i < pal.Colors.Length; i++)
        {
            pal.Colors[i] = new Color(
                pal.PaletteResource.Colors[i].Red * (FadeTime / (float)MaxFadeTime),
                pal.PaletteResource.Colors[i].Green * (FadeTime / (float)MaxFadeTime),
                pal.PaletteResource.Colors[i].Blue * (FadeTime / (float)MaxFadeTime));
        }

        pal.IsDirty = true;
    }

    private void Skip()
    {
        long time = Engine.Instance.GameTime.GetElapsedFrames();

        if ((time & 1) == 0)
        {
            foreach (Palette pal in Engine.Instance.Vram.GetSpritePalettes())
                FadePalette(pal);

            if (FadeTime == MinFadeTime)
            {
                CurrentStepAction = Step_Skip_1;
                SkippedTimer = 0;
            }
        }
        else
        {
            FadeTime--;

            foreach (Palette pal in Engine.Instance.Vram.GetBackgroundPalettes())
                FadePalette(pal);
        }
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

            TgxPlayfield2D playfield = TgxPlayfield2D.Instance;
            playfield.TileLayers[0].Screen.SetIsEnabled(true);
            playfield.TileLayers[1].Screen.SetIsEnabled(true);
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

            TgxPlayfield2D playfield = TgxPlayfield2D.Instance;
            playfield.TileLayers[2].Screen.SetIsEnabled(false);
            playfield.TileLayers[3].Screen.SetIsEnabled(true);
        }
    }

    private void Step_4()
    {
        // TODO: This should be changed to use the camera
        ScrollY++;
        Engine.Instance.Vram.GetBackground(1).Offset = new Vec2Int(0, ScrollY);
        Engine.Instance.Vram.GetBackground(3).Offset = new Vec2Int(0, ScrollY);

        if (ScrollY > 0xAF)
        {
            if (BlackLumAndLogoObj.EndOfAnimation)
            {
                if (BlackLumAndLogoObj.AnimationIndex < 4)
                {
                    BlackLumAndLogoObj.AnimationIndex++;
                    BlackLumAndLogoObj.ScreenPos = new Vec2Int(0x78, 0x80);
                }
                else
                {
                    BlackLumAndLogoObj.AnimationIndex = 6;
                    BlackLumAndLogoObj.ScreenPos = new Vec2Int(0x78, 0x46);
                }
            }

            if (BlackLumAndLogoObj.AnimationIndex < 5)
            {
                if ((Timer & 1) != 0)
                {
                    BlackLumAndLogoObj.ScreenPos += new Vec2Int(0, -2);
                }
                Timer++;
                AnimationPlayer.Instance.AddObject1(BlackLumAndLogoObj);
            }
        }

        if (Engine.Instance.JoyPad.Check(Input.Start) && ScrollY <= 0x35f)
            IsSkipping = true;

        if (IsSkipping)
            Skip();

        // TODO: This is just temporary code to make it work for now. Replace once we implement the camera!
        Background bg = Engine.Instance.Vram.GetBackground(1);
        if (bg.Offset.Y == bg.Height * 8 - 160)
        {
            CurrentStepAction = Step_5;
        }
    }

    private void Step_5()
    {
        if (BlackLumAndLogoObj.EndOfAnimation)
        {
            if (BlackLumAndLogoObj.AnimationIndex == 7)
            {
                BlackLumAndLogoObj.AnimationIndex = 8;
                CurrentStepAction = Step_6;
            }
            else
            {
                BlackLumAndLogoObj.AnimationIndex++;
            }
        }

        AnimationPlayer.Instance.AddObject1(BlackLumAndLogoObj);
    }

    private void Step_6()
    {
        if ((Engine.Instance.GameTime.GetElapsedFrames() & 0x10) != 0)
            AnimationPlayer.Instance.AddObject1(PressStartObj);

        AnimationPlayer.Instance.AddObject1(BlackLumAndLogoObj);

        // TODO: This is supposed to start the menu, but we loop the intro for now
        if (Engine.Instance.JoyPad.Check(Input.Start))
            Engine.Instance.FrameMngr.SetNextFrame(new Intro());
    }

    private void Step_Skip_1()
    {
        SkippedTimer++;

        if (SkippedTimer == 10)
        {
            BlackLumAndLogoObj.AnimationIndex = 8;
            BlackLumAndLogoObj.FrameIndex = 5;
            BlackLumAndLogoObj.ScreenPos = new Vec2Int(0x78, 0x46);

            // TODO: This should be changed to use the camera
            Engine.Instance.Vram.GetBackground(1).Offset = new Vec2Int(0, 880);
            Engine.Instance.Vram.GetBackground(3).Offset = new Vec2Int(0, 880);
            ScrollY = 879;
        }
        else if (SkippedTimer == 20)
        {
            CurrentStepAction = Step_Skip_2;
        }
    }

    private void Step_Skip_2()
    {
        long time = Engine.Instance.GameTime.GetElapsedFrames();

        if ((time & 1) == 0)
        {
            if (FadeTime < MaxFadeTime + 1)
            {
                foreach (Palette pal in Engine.Instance.Vram.GetBackgroundPalettes())
                    FadePalette(pal);
            }

            if (FadeTime == MaxFadeTime + 1)
                CurrentStepAction = Step_6;
        }
        else
        {
            FadeTime++;

            foreach (Palette pal in Engine.Instance.Vram.GetSpritePalettes())
                FadePalette(pal);
        }

        if ((Engine.Instance.GameTime.GetElapsedFrames() & 0x10) != 0)
            AnimationPlayer.Instance.AddObject1(PressStartObj);

        AnimationPlayer.Instance.AddObject1(BlackLumAndLogoObj);
    }

    #endregion

    #region Pubic Override Methods

    public override void Init(FrameMngr frameMngr)
    {
        Engine.Instance.Vram.ClearSprites();
        Engine.Instance.Vram.ClearBackgrounds();

        FrameMngr = frameMngr;
        Menu = new MenuAll(0);

        // TODO: Create GameInfo
        // TODO: Load GameInfo
        // TODO: Load menu data

        AnimationPlayer.Instance.Init(true, x =>
        {
            // TODO: Implement a proper sound callback
            Debug.WriteLine("Sound request: {0}", x);
        });

        AnimatedObjectResource introAnimResource = Storage.ReadResource<AnimatedObjectResource>(0x76);

        PressStartObj = new AnimatedObject(introAnimResource, false)
        {
            AnimationIndex = 9,
            Priority = 0,
            ScreenPos = new Vec2Int(0x78, 0x96)
        };

        // TODO: Game uses a different type of AnimatedObject here which seems to have additional support for not showing off-screen channels?
        BlackLumAndLogoObj = new AnimatedObject(introAnimResource, false)
        {
            AnimationIndex = 0,
            Priority = 0,
            ScreenPos = new Vec2Int(0x78, 0x80)
        };

        PlayfieldResource introPlayfield = Storage.ReadResource<Playfield>(0x75);

        TgxPlayfield.Load(introPlayfield);
        TgxPlayfield2D playfield = TgxPlayfield2D.Instance;
        playfield.TileLayers[0].Screen.SetIsEnabled(false);
        playfield.TileLayers[1].Screen.SetIsEnabled(false);
        playfield.TileLayers[3].Screen.SetIsEnabled(false);

        CurrentStepAction = Step_1;

        Timer = 0;
        ScrollY = 0;


        IsSkipping = false;
        FadeTime = 0x10;
    }

    public override void UnInit() { }

    public override void Step()
    {
        // TODO: Update animated tiles
        AnimationPlayer.Instance.Execute(Engine.Instance.Vram);

        CurrentStepAction();
    }

    #endregion
}