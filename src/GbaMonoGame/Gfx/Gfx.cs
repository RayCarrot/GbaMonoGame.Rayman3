using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame;

/// <summary>
/// Manages the graphics to be drawn on screen. This is in place of GBA VRAM,
/// where screens are the background and sprites are the objects.
/// </summary>
public static class Gfx
{
    static Gfx()
    {
        Pixel = new Texture2D(Engine.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        Pixel.SetData([Color.White]);
    }

    public static Texture2D Pixel { get; }
    public static Effect PaletteShader { get; private set; }

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

    private static ScreenEffect ScreenEffect { get; set; }

    public static Color Color { get; set; } = Color.White; // Color of all textures, can be used to simulate fading a palette
    public static Color ClearColor { get; set; } = Color.Black; // Background palette color 0 on GBA. This is not implemented on the N-Gage.
    public static float Fade { get; set; } = 0; // The equivalent of BLDY on GBA. This is not implemented on the N-Gage.
    public static float GbaFade
    {
        get => Fade * 16;
        set => Fade = value / 16;
    }
    public static FadeControl FadeControl { get; set; } // The equivalent of BLDCNT on GBA. This is not implemented on the N-Gage.

    public static void Load(Effect paletteShader)
    {
        PaletteShader = paletteShader;
    }

    public static void AddScreen(GfxScreen screen) => Screens.Add(screen.Id, screen);
    public static GfxScreen GetScreen(int id) => Screens[id];
    public static IEnumerable<GfxScreen> GetScreens() => Screens.Values;
    public static void ClearScreens() => Screens.Clear();

    public static void AddSprite(Sprite sprite) => Sprites.Add(sprite);
    public static void AddBackSprite(Sprite sprite) => BackSprites.Add(sprite);
    public static IEnumerable<Sprite> GetSprites() => Sprites;
    public static void ClearSprites()
    {
        Sprites.Clear();
        BackSprites.Clear();
    }

    // Used in place for screen effects made using GBA features such as windows
    public static ScreenEffect GetScreenEffect() => ScreenEffect;
    public static void SetScreenEffect(ScreenEffect screenEffect) => ScreenEffect = screenEffect;
    public static void ClearScreenEffect() => ScreenEffect = null;

    private static void DrawFade(GfxRenderer renderer)
    {
        // TODO: Add config option to use GBA fading on N-Gage
        if (Engine.Settings.Platform == Platform.GBA && FadeControl.Mode != FadeMode.None && Fade is > 0 and <= 1)
        {
            renderer.BeginRender(new RenderOptions(false, null, Engine.ScreenCamera));

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
        // Draw clear color on GBA
        if (Engine.Settings.Platform == Platform.GBA)
        {
            renderer.BeginRender(new RenderOptions(false, null, Engine.ScreenCamera));
            renderer.DrawFilledRectangle(Vector2.Zero, Engine.ScreenCamera.Resolution, ClearColor);
        }

        // Draw each game layer (3-0)
        for (int i = 3; i >= 0; i--)
        {
            // Draw screens
            foreach (GfxScreen screen in Screens.Values.Where(x => x.IsEnabled && x.Priority == i))
                screen.Draw(renderer, Color);

            if ((FadeControl.Flags & (FadeFlags)(1 << i)) != 0)
                DrawFade(renderer);

            // Draw sprites
            foreach (Sprite sprite in BackSprites.Where(x => x.Priority == i))
                sprite.Draw(renderer, Color);
            foreach (Sprite sprite in Sprites.Where(x => x.Priority == i).Reverse())
                sprite.Draw(renderer, Color);

            if ((FadeControl.Flags & (FadeFlags)(1 << (i + 4))) != 0)
                DrawFade(renderer);
        }

        // Draw screen fade if no special flag is set
        if (FadeControl.Flags == FadeFlags.Default)
            DrawFade(renderer);

        // TODO: Add option to use this on N-Gage
        // Draw the screen effect on GBA if there is one
        if (Engine.Settings.Platform == Platform.GBA)
            ScreenEffect?.Draw(renderer);
    }
}