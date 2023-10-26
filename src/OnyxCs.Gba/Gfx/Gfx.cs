using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba;

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
        DebugBoxes = new List<DebugBox>();
    }

    public static IReadOnlyList<GfxScreen> Screens { get; }
    public static List<Sprite> Sprites { get; }
    public static List<DebugBox> DebugBoxes { get; }

    public static GraphicsDevice GraphicsDevice { get; set; }
    public static GfxCamera GfxCamera { get; set; }

    public static void AddSprite(Sprite sprite) => Sprites.Add(sprite);
    public static void AddDebugBox(DebugBox debugBox) => DebugBoxes.Add(debugBox);

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

        // Draw debug boxes
        foreach (DebugBox debugBox in DebugBoxes)
            renderer.DrawFilledRectangle(debugBox.Rectangle, new Color(debugBox.Color, 0.3f), 0);
    }

    public static void ClearSprites()
    {
        Sprites.Clear();
        DebugBoxes.Clear();
    }
}