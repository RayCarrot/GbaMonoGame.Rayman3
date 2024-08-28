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
        int windowResCount = Math.Min((int)(screenRes.X / originalRes.X), (int)(screenRes.Y / originalRes.Y));

        Options =
        [
            // Display mode
            new MultiSelectionMenuOption<bool>(
                name: "Display mode",
                items:
                [
                    new MultiSelectionMenuOption<bool>.Item("Windowed", false),
                    new MultiSelectionMenuOption<bool>.Item("Fullscreen", true)
                ],
                getData: _ => Engine.Config.IsFullscreen,
                setData: data =>
                {
                    Engine.Config.IsFullscreen = data;
                    Game.SaveWindowState();
                    Game.ApplyDisplayConfig();
                },
                getCustomName: _ => null),

            // TODO: Don't auto-apply
            // Fullscreen resolution
            new MultiSelectionMenuOption<Point>(
                name: "Fullscreen resolution",
                items: adapter.SupportedDisplayModes.
                    Select(x => new MultiSelectionMenuOption<Point>.Item($"{x.Width} x {x.Height}", new Point(x.Width, x.Height))).
                    ToArray(),
                getData: _ => Engine.Config.FullscreenResolution,
                setData: data =>
                {
                    Engine.Config.FullscreenResolution = data;
                    Game.ApplyDisplayConfig();
                },
                getCustomName: data => $"{data.X} x {data.Y}"),

            // Window resolution
            new MultiSelectionMenuOption<float>(
                name: "Window resolution",
                items: Enumerable.Range(1, windowResCount).
                    Select(x => new MultiSelectionMenuOption<float>.Item($"{x}x", x)).
                    ToArray(),
                getData: _ =>
                {
                    Rectangle windowBounds = Engine.Settings.Platform switch
                    {
                        Platform.GBA => Engine.Config.GbaWindowBounds,
                        Platform.NGage => Engine.Config.NGageWindowBounds,
                        _ => throw new UnsupportedPlatformException()
                    };

                    return windowBounds.Size.ToVector2().X / Engine.GameViewPort.RequestedGameResolution.X;
                },
                setData: data =>
                {
                    Point windowRes = (Engine.GameViewPort.RequestedGameResolution * data).ToPoint();

                    switch (Engine.Settings.Platform)
                    {
                        case Platform.GBA:
                            Engine.Config.GbaWindowBounds = Engine.Config.GbaWindowBounds with { Size = windowRes };
                            break;

                        case Platform.NGage:
                            Engine.Config.NGageWindowBounds = Engine.Config.NGageWindowBounds with { Size = windowRes };
                            break;

                        default:
                            throw new UnsupportedPlatformException();
                    }

                    Game.ApplyDisplayConfig();
                    Game.SaveWindowState();
                },
                getCustomName: data => $"{data:0.00}x"),

            // Internal resolution
            new MultiSelectionMenuOption<Point?>(
                name: "Internal resolution",
                items:
                [
                    new MultiSelectionMenuOption<Point?>.Item($"Original ({originalRes.X} x {originalRes.Y})", null),
                    new MultiSelectionMenuOption<Point?>.Item("Widescreen (288 x 162)", new Point(288, 162)), // 16:9
                ],
                getData: _ => Engine.Config.InternalResolution,
                setData: data =>
                {
                    Engine.Config.InternalResolution = data;
                    Engine.GameViewPort.SetRequestedResolution(Engine.Config.InternalResolution?.ToVector2());
                },
                getCustomName: data => data == null ? null : $"{data.Value.X} x {data.Value.Y}"),

            // Camera scale
            new MultiSelectionMenuOption<float>(
                name: "Camera scale",
                items:
                [
                    new MultiSelectionMenuOption<float>.Item("Original (1.00)", 1.00f),
                    new MultiSelectionMenuOption<float>.Item("Low (1.15)", 1.15f),
                    new MultiSelectionMenuOption<float>.Item("Medium (1.35)", 1.35f),
                    new MultiSelectionMenuOption<float>.Item("High (1.50)", 1.50f),
                ],
                getData: _ => Engine.Config.PlayfieldCameraScale,
                setData: data => Engine.Config.PlayfieldCameraScale = data,
                getCustomName: data => $"{data:0.00}"),

            // HUD scale
            new MultiSelectionMenuOption<float>(
                name: "HUD scale",
                items:
                [
                    new MultiSelectionMenuOption<float>.Item("Original (1.00)", 1.00f),
                    new MultiSelectionMenuOption<float>.Item("Low (1.15)", 1.15f),
                    new MultiSelectionMenuOption<float>.Item("Medium (1.35)", 1.35f),
                    new MultiSelectionMenuOption<float>.Item("High (1.50)", 1.50f),
                ],
                getData: _ => Engine.Config.HudCameraScale,
                setData: data => Engine.Config.HudCameraScale = data,
                getCustomName: data => $"{data:0.00}"),
        ];

        foreach (MenuOption option in Options)
            option.Init();
    }

    private GbaGame Game { get; }
    private MenuOption[] Options { get; }

    public override void Update(MenuManager menu)
    {
        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        menu.Text("Display options");
        menu.Spacing();

        menu.SetColumns(1, 0.9f);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Left);

        foreach (MenuOption option in Options)
            option.Update(menu);

        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        if (menu.Button("Back"))
            menu.GoBack();
    }
}