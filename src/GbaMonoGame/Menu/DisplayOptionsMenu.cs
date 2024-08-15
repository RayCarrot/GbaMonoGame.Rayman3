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
        Vector2 originalRes = Engine.GameViewPort.OriginalGameResolution;
        Vector2 screenRes = new(adapter.CurrentDisplayMode.Width, adapter.CurrentDisplayMode.Height);

        AvailableIsFullscreenNames = new[]
        {
            "Windowed",
            "Fullscreen",
        };
        IsFullscreen = OriginalIsFullscreen;

        AvailableFullscreenResolutionNames = adapter.SupportedDisplayModes.Select(x => $"{x.Width} x {x.Height}").ToArray();
        AvailableFullscreenResolutions = adapter.SupportedDisplayModes.Select(x => new Point(x.Width, x.Height)).ToArray();
        OriginalFullscreenResolutionSelectedIndex = Array.IndexOf(AvailableFullscreenResolutions, Engine.Config.FullscreenResolution);
        FullscreenResolutionSelectedIndex = OriginalFullscreenResolutionSelectedIndex == -1
            ? AvailableFullscreenResolutions.Length - 1
            : OriginalFullscreenResolutionSelectedIndex;

        int windowResCount = Math.Min((int)(screenRes.X / originalRes.X), (int)(screenRes.Y / originalRes.Y));
        AvailableWindowResolutionNames = new[]
        {
            "Custom",
        }.Concat(Enumerable.Range(0, windowResCount).Select(x => $"{x + 1}x")).ToArray();
        WindowResolutionScale = OriginalWindowResolutionScale;

        // TODO: Add other aspect ratios too? Allow setting custom value in the UI?
        AvailableInternalResolutionNames = new[]
        {
            $"Original ({originalRes.X}x{originalRes.Y})",
            "Widescreen (288x162)", // 16:9
        };
        AvailableInternalResolutions = new Point?[]
        {
            null,
            new(288, 162),
        };
        OriginalInternalResolutionSelectedIndex = Array.IndexOf(AvailableInternalResolutions, Engine.Config.InternalResolution);
        InternalResolutionSelectedIndex = OriginalInternalResolutionSelectedIndex == -1
            ? AvailableInternalResolutions.Length - 1
            : OriginalInternalResolutionSelectedIndex;

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

        AvailableHudCameraScales = new[]
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
        AvailableHudCameraScaleNames = AvailableHudCameraScales.Select(x => $"{x:0.00}").ToArray();
        OriginalHudCameraScale = Array.IndexOf(AvailableHudCameraScales, Engine.Config.HudCameraScale);
        HudCameraScale = OriginalHudCameraScale == -1
            ? Array.IndexOf(AvailableHudCameraScales, 1.00f)
            : OriginalHudCameraScale;
    }

    private GbaGame Game { get; }

    private string[] AvailableIsFullscreenNames { get; }
    private bool OriginalIsFullscreen => Engine.Config.IsFullscreen;
    private bool IsFullscreen { get; set; }

    private string[] AvailableFullscreenResolutionNames { get; }
    private Point[] AvailableFullscreenResolutions { get; }
    private int OriginalFullscreenResolutionSelectedIndex { get; set; }
    private int FullscreenResolutionSelectedIndex { get; set; }

    private string[] AvailableWindowResolutionNames { get; }
    private int OriginalWindowResolutionScale
    {
        get
        {
            Rectangle windowBounds = Engine.Settings.Platform switch
            {
                Platform.GBA => Engine.Config.GbaWindowBounds,
                Platform.NGage => Engine.Config.NGageWindowBounds,
                _ => throw new UnsupportedPlatformException()
            };

            float scale = windowBounds.Size.ToVector2().X / Engine.GameViewPort.RequestedGameResolution.X;

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

    private string[] AvailablePlayfieldCameraScaleNames { get; }
    private float[] AvailablePlayfieldCameraScales { get; }
    private int OriginalPlayfieldCameraScale { get; set; }
    private int PlayfieldCameraScale { get; set; }

    private string[] AvailableHudCameraScaleNames { get; }
    private float[] AvailableHudCameraScales { get; }
    private int OriginalHudCameraScale { get; set; }
    private int HudCameraScale { get; set; }

    public override void Update(MenuManager menu)
    {
        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        menu.Text("Display options");
        menu.Spacing();

        menu.SetColumns(1, 0.9f);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Left);

        menu.Text("Mode");
        IsFullscreen = menu.Selection(AvailableIsFullscreenNames, IsFullscreen ? 1 : 0) == 1;

        menu.Text("Fullscreen resolution");
        FullscreenResolutionSelectedIndex = menu.Selection(AvailableFullscreenResolutionNames, FullscreenResolutionSelectedIndex);

        menu.Text("Window resolution");
        WindowResolutionScale = menu.Selection(AvailableWindowResolutionNames, WindowResolutionScale);

        menu.Text("Internal resolution");
        InternalResolutionSelectedIndex = menu.Selection(AvailableInternalResolutionNames, InternalResolutionSelectedIndex);

        menu.Text("Camera scale");
        PlayfieldCameraScale = menu.Selection(AvailablePlayfieldCameraScaleNames, PlayfieldCameraScale);

        menu.Text("HUD scale");
        HudCameraScale = menu.Selection(AvailableHudCameraScaleNames, HudCameraScale);

        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        bool hasChanges = FullscreenResolutionSelectedIndex != OriginalFullscreenResolutionSelectedIndex ||
                          IsFullscreen != OriginalIsFullscreen ||
                          WindowResolutionScale != OriginalWindowResolutionScale||
                          InternalResolutionSelectedIndex != OriginalInternalResolutionSelectedIndex ||
                          PlayfieldCameraScale != OriginalPlayfieldCameraScale||
                          HudCameraScale != OriginalHudCameraScale;

        if (menu.Button("Apply changes", hasChanges))
        {
            Engine.Config.FullscreenResolution = AvailableFullscreenResolutions[FullscreenResolutionSelectedIndex];
            Engine.Config.IsFullscreen = IsFullscreen;

            Engine.Config.InternalResolution = AvailableInternalResolutions[InternalResolutionSelectedIndex];
            Engine.Config.PlayfieldCameraScale = AvailablePlayfieldCameraScales[PlayfieldCameraScale];
            Engine.Config.HudCameraScale = AvailableHudCameraScales[HudCameraScale];
            Engine.GameViewPort.SetRequestedResolution(Engine.Config.InternalResolution?.ToVector2());

            if (WindowResolutionScale != 0)
            {
                Point windowRes = (Engine.GameViewPort.RequestedGameResolution * WindowResolutionScale).ToPoint();

                switch (Engine.Settings.Platform)
                {
                    case Platform.GBA:
                        Engine.Config.GbaWindowBounds = Engine.Config.GbaWindowBounds with { Size = windowRes };
                        break;

                    case Platform.NGage:
                        Engine.Config.NGageWindowBounds = Engine.Config.GbaWindowBounds with { Size = windowRes };
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