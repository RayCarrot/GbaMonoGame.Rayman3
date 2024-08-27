using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework;

namespace GbaMonoGame;

/// <summary>
/// Manages the graphics to be drawn on screen. This is in place of GBA VRAM,
/// where screens are the background and sprites are the objects.
/// </summary>
public static class Gfx
{
    /// <summary>
    /// The game screens. These are the equivalent of backgrounds on the GBA
    /// and there are always 4 of these.
    /// </summary>
    private static Dictionary<int, GfxScreen> Screens { get; } = new();

    /// <summary>
    /// The game sprites. These are the equivalent of objects on the GBA.
    /// Unlike the GBA there is no defined maximum sprite count.
    /// </summary>
    private static List<Sprite> Sprites { get; } = new();

    /// <summary>
    /// Same as <see cref="Sprites"/>, but for sprites which are added in
    /// last. Sometimes the game adds sprites at the end of OAM to make
    /// sure they get a different priority.
    /// </summary>
    private static List<Sprite> BackSprites { get; } = new();

    public static float Fade { get; set; } = 0; // The equivalent of BLDY on GBA. This is not implemented on the N-Gage.
    public static FadeControl FadeControl { get; set; } // The equivalent of BLDCNT on GBA. This is not implemented on the N-Gage.

    public static void AddScreen(GfxScreen screen) => Screens.Add(screen.Id, screen);
    public static GfxScreen GetScreen(int id) => Screens[id];
    public static IEnumerable<GfxScreen> GetScreens() => Screens.Values;
    public static void ClearScreens() => Screens.Clear();

    public static void AddSprite(Sprite sprite) => Sprites.Add(sprite);
    public static void AddBackSprite(Sprite sprite) => BackSprites.Add(sprite);
    public static void ClearSprites()
    {
        Sprites.Clear();
        BackSprites.Clear();
    }

    private static void DrawFade(GfxRenderer renderer)
    {
        // TODO: Add config option to use GBA fading on N-Gage
        if (Engine.Settings.Platform == Platform.GBA && FadeControl.Mode != FadeMode.None && Fade is > 0 and <= 1)
        {
            renderer.BeginRender(new RenderOptions(false, Engine.ScreenCamera));

            switch (FadeControl.Mode)
            {
                case FadeMode.AlphaBlending:
                    throw new NotImplementedException();

                case FadeMode.BrightnessIncrease:
                    renderer.DrawFilledRectangle(Vector2.Zero, Engine.ScreenCamera.Resolution, Color.White * Fade);
                    break;
                
                case FadeMode.BrightnessDecrease:
                    renderer.DrawFilledRectangle(Vector2.Zero, Engine.ScreenCamera.Resolution, Color.Black * Fade);
                    break;
            }
        }
    }

    public static void Draw(GfxRenderer renderer)
    {
        // Draw each game layer (3-0)
        for (int i = 3; i >= 0; i--)
        {
            // Draw screens
            foreach (GfxScreen screen in Screens.Values.Where(x => x.IsEnabled && x.Priority == i))
                screen.Draw(renderer);

            if ((FadeControl.Flags & (FadeFlags)(1 << i)) != 0)
                DrawFade(renderer);

            // Draw sprites
            foreach (Sprite sprite in BackSprites.Where(x => x.Priority == i))
                sprite.Draw(renderer);
            foreach (Sprite sprite in Sprites.Where(x => x.Priority == i).Reverse())
                sprite.Draw(renderer);

            if ((FadeControl.Flags & (FadeFlags)(1 << (i + 4))) != 0)
                DrawFade(renderer);
        }

        if (FadeControl.Flags == FadeFlags.Default)
            DrawFade(renderer);
    }
}