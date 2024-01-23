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

            float scale = windowRes.ToVector2().X / Engine.GameWindow.GameResolution.X;

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

    public override void Update(MenuManager menu)
    {
        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        menu.Text("Display options");
        menu.Empty();

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

        // TODO: Add: Internal resolution    Original GBA N-Gage Widescreen

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

        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        bool hasChanges = FullscreenResolutionSelectedIndex != OriginalFullscreenResolutionSelectedIndex ||
                          IsFullscreen != OriginalIsFullscreen ||
                          WindowResolutionScale != OriginalWindowResolutionScale;

        if (menu.Button("Apply changes", hasChanges))
        {
            Engine.Config.FullscreenResolution = AvailableFullscreenResolutions[FullscreenResolutionSelectedIndex];
            Engine.Config.IsFullscreen = IsFullscreen;

            if (WindowResolutionScale != 0)
            {
                switch (Engine.Settings.Platform)
                {
                    case Platform.GBA:
                        Engine.Config.GbaWindowResolution = (Engine.GameWindow.GameResolution * WindowResolutionScale).ToPoint();
                        break;

                    case Platform.NGage:
                        Engine.Config.NGageWindowResolution = (Engine.GameWindow.GameResolution * WindowResolutionScale).ToPoint();
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
        }

        if (menu.Button(hasChanges ? "Back & discard changes" : "Back"))
            menu.GoBack();
    }
}