using System;
using System.Linq;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame;

public class DisplayOptionsMenu : Menu
{
    public DisplayOptionsMenu(GbaGame game)
    {
        Game = game;

        GraphicsAdapter adapter = Game.GraphicsDevice.Adapter;

        AvailableFullscreenResolutionNames = adapter.SupportedDisplayModes.Select(x => $"{x.Width} x {x.Height}").ToArray();
        AvailableFullscreenResolutions = adapter.SupportedDisplayModes.Select(x => new Point(x.Width, x.Height)).ToArray();
        OriginalFullscreenResolutionSelectedIndex = Array.IndexOf(AvailableFullscreenResolutions, Engine.Config.FullscreenResolution);
        FullscreenResolutionSelectedIndex = OriginalFullscreenResolutionSelectedIndex == -1
            ? AvailableFullscreenResolutions.Length - 1
            : OriginalFullscreenResolutionSelectedIndex;

        AvailableIsFullscreenNames = new[]
        {
            "Windowed",
            "Fullscreen",
        };
        IsFullscreen = OriginalIsFullscreen;

        AvailableWindowResolutionNames = new[]
        {
            "Custom",
            "1x",
            "2x",
            "3x",
            "4x",
            "5x",
            "6x",
            "7x",
            "8x",
        };
        WindowResolutionScale = OriginalWindowResolutionScale;

        AvailableInternalResolutionNames = new[]
        {
            "Original",
            "GBA (240x160)",
            "N-Gage (176x208)",
            "Widescreen (288x162)",
        };
        AvailableInternalResolutions = new Point?[]
        {
            null,
            new(240, 160),
            new(176, 208),
            new(288, 162),
        };
        OriginalInternalResolutionSelectedIndex = Array.IndexOf(AvailableInternalResolutions, Engine.Config.InternalResolution);
        InternalResolutionSelectedIndex = OriginalInternalResolutionSelectedIndex == -1
            ? AvailableInternalResolutions.Length - 1
            : OriginalInternalResolutionSelectedIndex;

        AvailableDynamicPlayfieldCameraScaleNames = new[]
        {
            "Off",
            "On",
        };
        OriginalDynamicPlayfieldCameraScale = Engine.Config.DynamicPlayfieldCameraScale;

        AvailablePlayfieldCameraScales = new[]
        {
            0.90f,
            0.95f,
            1.00f,
            1.05f,
            1.10f,
            1.15f,
            1.20f,
            1.25f,
            1.30f,
            1.35f,
            1.40f,
            1.45f,
            1.50f,
            1.60f,
            1.70f,
            1.80f,
            1.90f,
            2.00f,
        };
        AvailablePlayfieldCameraScaleNames = AvailablePlayfieldCameraScales.Select(x => $"{x:0.00}").ToArray();
        OriginalPlayfieldCameraScale = Array.IndexOf(AvailablePlayfieldCameraScales, Engine.Config.PlayfieldCameraScale);
        PlayfieldCameraScale = OriginalPlayfieldCameraScale == -1
            ? Array.IndexOf(AvailablePlayfieldCameraScales, 1.00f)
            : OriginalPlayfieldCameraScale;
    }

    private GbaGame Game { get; }

    private string[] AvailableFullscreenResolutionNames { get; }
    private Point[] AvailableFullscreenResolutions { get; }
    private int OriginalFullscreenResolutionSelectedIndex { get; set; }
    private int FullscreenResolutionSelectedIndex { get; set; }

    private string[] AvailableIsFullscreenNames { get; }
    private bool OriginalIsFullscreen => Engine.Config.IsFullscreen;
    private bool IsFullscreen { get; set; }

    private string[] AvailableWindowResolutionNames { get; }
    private int OriginalWindowResolutionScale
    {
        get
        {
            Point windowRes = Engine.Settings.Platform switch
            {
                Platform.GBA => Engine.Config.GbaWindowResolution,
                Platform.NGage => Engine.Config.NGageWindowResolution,
                _ => throw new UnsupportedPlatformException()
            };

            float scale = windowRes.ToVector2().X / Engine.GameViewPort.RequestedGameResolution.X;

            for (int i = 0; i < 8; i++)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (i == scale)
                    return i;
            }

            return 0;
        }
    }
    private int WindowResolutionScale { get; set; }

    private string[] AvailableInternalResolutionNames { get; }
    private Point?[] AvailableInternalResolutions { get; }
    private int OriginalInternalResolutionSelectedIndex { get; set; }
    private int InternalResolutionSelectedIndex { get; set; }

    private string[] AvailableDynamicPlayfieldCameraScaleNames { get; }
    private bool OriginalDynamicPlayfieldCameraScale { get; }
    private bool DynamicPlayfieldCameraScale { get; set; }

    private string[] AvailablePlayfieldCameraScaleNames { get; }
    private float[] AvailablePlayfieldCameraScales { get; }
    private int OriginalPlayfieldCameraScale { get; set; }
    private int PlayfieldCameraScale { get; set; }

    public override void Update(MenuManager menu)
    {
        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        menu.Text("Display options");
        menu.Spacing();

        menu.SetColumns(1, 0.9f);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Left);

        menu.Text("Fullscreen resolution");
        FullscreenResolutionSelectedIndex = menu.Selection(AvailableFullscreenResolutionNames, FullscreenResolutionSelectedIndex);

        menu.Text("Mode");
        IsFullscreen = menu.Selection(AvailableIsFullscreenNames, IsFullscreen ? 1 : 0) == 1;

        menu.Text("Window resolution");
        WindowResolutionScale = menu.Selection(AvailableWindowResolutionNames, WindowResolutionScale);

        menu.Text("Internal resolution");
        InternalResolutionSelectedIndex = menu.Selection(AvailableInternalResolutionNames, InternalResolutionSelectedIndex);

        menu.Text("Dynamic camera scale");
        DynamicPlayfieldCameraScale = menu.Selection(AvailableDynamicPlayfieldCameraScaleNames, DynamicPlayfieldCameraScale ? 1 : 0) == 1;

        menu.Text("Camera scale");
        PlayfieldCameraScale = menu.Selection(AvailablePlayfieldCameraScaleNames, PlayfieldCameraScale);

        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        bool hasChanges = FullscreenResolutionSelectedIndex != OriginalFullscreenResolutionSelectedIndex ||
                          IsFullscreen != OriginalIsFullscreen ||
                          WindowResolutionScale != OriginalWindowResolutionScale||
                          InternalResolutionSelectedIndex != OriginalInternalResolutionSelectedIndex ||
                          DynamicPlayfieldCameraScale != OriginalDynamicPlayfieldCameraScale ||
                          PlayfieldCameraScale != OriginalPlayfieldCameraScale;

        if (menu.Button("Apply changes", hasChanges))
        {
            Engine.Config.FullscreenResolution = AvailableFullscreenResolutions[FullscreenResolutionSelectedIndex];
            Engine.Config.IsFullscreen = IsFullscreen;

            Engine.Config.InternalResolution = AvailableInternalResolutions[InternalResolutionSelectedIndex];
            Engine.Config.DynamicPlayfieldCameraScale = DynamicPlayfieldCameraScale;
            Engine.Config.PlayfieldCameraScale = AvailablePlayfieldCameraScales[PlayfieldCameraScale];
            Engine.GameViewPort.SetRequestedResolution(Engine.Config.InternalResolution?.ToVector2());

            if (WindowResolutionScale != 0)
            {
                switch (Engine.Settings.Platform)
                {
                    case Platform.GBA:
                        Engine.Config.GbaWindowResolution = (Engine.GameViewPort.RequestedGameResolution * WindowResolutionScale).ToPoint();
                        break;

                    case Platform.NGage:
                        Engine.Config.NGageWindowResolution = (Engine.GameViewPort.RequestedGameResolution * WindowResolutionScale).ToPoint();
                        break;

                    default:
                        throw new UnsupportedPlatformException();
                }
                Game.ApplyDisplayConfig();
            }

            Game.SaveWindowState();
            Engine.SaveConfig();
            Game.ApplyDisplayConfig();

            OriginalFullscreenResolutionSelectedIndex = FullscreenResolutionSelectedIndex;
            OriginalInternalResolutionSelectedIndex = InternalResolutionSelectedIndex;
        }

        if (menu.Button(hasChanges ? "Back & discard changes" : "Back"))
            menu.GoBack();
    }
}