using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba;

/// <summary>
/// Manages the graphics to be drawn on screen. This is in place of GBA VRAM,
/// where screens are the background and sprites are the objects.
/// </summary>
public static class Gfx
{
    static Gfx()
    {
        Screens = new[]
        {
            new GfxScreen(0),
            new GfxScreen(1),
            new GfxScreen(2),
            new GfxScreen(3),
        };
        Sprites = new List<Sprite>();
    }

    /// <summary>
    /// The game screens. These are the equivalent of backgrounds on the GBA
    /// and there are always 4 of these.
    /// </summary>
    public static IReadOnlyList<GfxScreen> Screens { get; }

    /// <summary>
    /// The game sprites. These are the equivalent of objects on the GBA.
    /// Unlike the GBA there is no defined maximum sprite count.
    /// </summary>
    public static List<Sprite> Sprites { get; }

    /// <summary>
    /// The graphics device to use for creating textures.
    /// </summary>
    public static GraphicsDevice GraphicsDevice { get; set; }

    /// <summary>
    /// The graphics camera to use when rendering the game.
    /// </summary>
    public static GfxCamera GfxCamera { get; set; }

    public static Platform Platform { get; set; }

    public static void AddSprite(Sprite sprite) => Sprites.Add(sprite);

    public static void Draw(GfxRenderer renderer)
    {
        // Draw each game layer
        for (int i = 3; i >= 0; i--)
        {
            // Draw screens
            foreach (GfxScreen screen in Screens.Where(x => x.IsEnabled && x.Priority == i))
                screen.Draw(renderer);

            // Draw sprites
            foreach (Sprite sprite in Sprites.Where(x => x.Priority == i).Reverse())
                sprite.Draw(renderer);
        }
    }

    public static void ClearScreens()
    {
        foreach (GfxScreen screen in Screens)
        {
            screen.IsEnabled = false;
            screen.Renderer = null;
        }
    }

    public static void ClearSprites()
    {
        Sprites.Clear();
    }
}