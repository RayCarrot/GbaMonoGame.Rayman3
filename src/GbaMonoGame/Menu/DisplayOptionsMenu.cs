using System;
using System.Linq;
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
    }

    private GbaGame Game { get; }

    private string[] AvailableFullscreenResolutionNames { get; }
    private Point[] AvailableFullscreenResolutions { get; }
    private int OriginalFullscreenResolutionSelectedIndex { get; set; }
    private int FullscreenResolutionSelectedIndex { get; set; }

    private bool OriginalIsFullscreen => Engine.Config.IsFullscreen;
    private bool IsFullscreen { get; set; }

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
        // TODO: Add: Window resolution    1x 2x 3x 4x 5x 6x 7x 8x

        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        bool hasChanges = FullscreenResolutionSelectedIndex != OriginalFullscreenResolutionSelectedIndex ||
                          IsFullscreen != OriginalIsFullscreen;

        if (menu.Button("Apply changes", hasChanges))
        {
            Engine.Config.FullscreenResolution = AvailableFullscreenResolutions[FullscreenResolutionSelectedIndex];
            Engine.Config.IsFullscreen = IsFullscreen;

            Game.SaveWindowState();
            Engine.SaveConfig();
            Game.UpdateResolutionAndWindowState();

            OriginalFullscreenResolutionSelectedIndex = FullscreenResolutionSelectedIndex;
        }

        if (menu.Button(hasChanges ? "Back & discard changes" : "Back"))
            menu.GoBack();
    }
}