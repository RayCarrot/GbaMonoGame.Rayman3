﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba;

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

    public static void AddScreen(GfxScreen screen) => Screens.Add(screen.Id, screen);
    public static GfxScreen GetScreen(int id) => Screens[id];
    public static IEnumerable<GfxScreen> GetScreens() => Screens.Values;
    public static void ClearScreens() => Screens.Clear();

    public static void AddSprite(Sprite sprite) => Sprites.Add(sprite);
    public static void ClearSprites() => Sprites.Clear();

    public static void Draw(GfxRenderer renderer)
    {
        // Draw each game layer
        for (int i = 3; i >= 0; i--)
        {
            // Draw screens
            foreach (GfxScreen screen in Screens.Values.Where(x => x.IsEnabled && x.Priority == i))
                screen.Draw(renderer);

            // Draw sprites
            foreach (Sprite sprite in Sprites.Where(x => x.Priority == i))
                sprite.Draw(renderer);
        }
    }
}