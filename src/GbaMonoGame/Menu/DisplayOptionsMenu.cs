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

        IsFullscreen = OriginalIsFullscreen;

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
    }

    private GbaGame Game { get; }

    private string[] AvailableFullscreenResolutionNames { get; }
    private Point[] AvailableFullscreenResolutions { get; }
    private int OriginalFullscreenResolutionSelectedIndex { get; set; }
    private int FullscreenResolutionSelectedIndex { get; set; }

    private bool OriginalIsFullscreen => Engine.Config.IsFullscreen;
    private bool IsFullscreen { get; set; }

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

            float scale = windowRes.ToVector2().X / Engine.GameWindow.RequestedGameResolution.X;

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
        IsFullscreen = menu.Selection(new[]
        {
            "Windowed",
            "Fullscreen",
        }, IsFullscreen ? 1 : 0) == 1;

        menu.Text("Window resolution");
        WindowResolutionScale = menu.Selection(new[]
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
        }, WindowResolutionScale);

        menu.Text("Internal resolution");
        InternalResolutionSelectedIndex = menu.Selection(AvailableInternalResolutionNames, InternalResolutionSelectedIndex);

        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        bool hasChanges = FullscreenResolutionSelectedIndex != OriginalFullscreenResolutionSelectedIndex ||
                          IsFullscreen != OriginalIsFullscreen ||
                          WindowResolutionScale != OriginalWindowResolutionScale||
                          InternalResolutionSelectedIndex != OriginalInternalResolutionSelectedIndex;

        if (menu.Button("Apply changes", hasChanges))
        {
            Engine.Config.FullscreenResolution = AvailableFullscreenResolutions[FullscreenResolutionSelectedIndex];
            Engine.Config.IsFullscreen = IsFullscreen;

            Engine.Config.InternalResolution = AvailableInternalResolutions[InternalResolutionSelectedIndex];
            Engine.GameWindow.SetRequestedResolution(Engine.Config.InternalResolution?.ToVector2());

            if (WindowResolutionScale != 0)
            {
                switch (Engine.Settings.Platform)
                {
                    case Platform.GBA:
                        Engine.Config.GbaWindowResolution = (Engine.GameWindow.RequestedGameResolution * WindowResolutionScale).ToPoint();
                        break;

                    case Platform.NGage:
                        Engine.Config.NGageWindowResolution = (Engine.GameWindow.RequestedGameResolution * WindowResolutionScale).ToPoint();
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