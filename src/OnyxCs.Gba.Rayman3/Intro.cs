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
    private Action _currentStepAction;

    public FrameMngr FrameMngr { get; set; }
    public MenuAll Menu { get; set; }
    public AnimatedObject PressStartObj { get; set; }
    public AnimatedObject BlackLumAndLogoObj { get; set; }
    public int Timer { get; set; }
    public int ScrollY { get; set; }

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

        _currentStepAction = Step_1;
    }

    public override void UnInit()
    {
        throw new NotImplementedException();
    }

    private void Step_1()
    {
        Timer++;

        if (Timer > 0x3C)
        {
            Timer = 0;
            _currentStepAction = Step_2;

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
            _currentStepAction = Step_3;
        }
    }

    private void Step_3()
    {
        Timer++;

        if (Timer > (0x10 * 4))
        {
            Timer = 0;
            _currentStepAction = Step_4;

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

        // TODO: This is just temporary code to make it work for now. Replace once we implement the camera!
        Background bg = Engine.Instance.Vram.GetBackground(1);
        if (bg.Offset.Y == bg.Height * 8 - 160)
        {
            _currentStepAction = Step_5;
        }
    }

    private void Step_5()
    {
        if (BlackLumAndLogoObj.EndOfAnimation)
        {
            if (BlackLumAndLogoObj.AnimationIndex == 7)
            {
                BlackLumAndLogoObj.AnimationIndex = 8;
                _currentStepAction = Step_6;
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
    }

    public override void Step()
    {
        // TODO: Update animated tiles
        AnimationPlayer.Instance.Execute(Engine.Instance.Vram);

        _currentStepAction();
    }
}