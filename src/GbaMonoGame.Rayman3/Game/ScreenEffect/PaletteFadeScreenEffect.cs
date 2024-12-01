using Microsoft.Xna.Framework;

namespace GbaMonoGame.Rayman3;

/// <summary>
/// A custom object for emulating editing the GBA palettes to fade the game to black. Although
/// the game mainly uses BLD for this there are cases where it modifies the palette instead.
/// Unlike the BLD effect this is also fully supported on N-Gage.
/// </summary>
public class PaletteFadeScreenEffect : ScreenEffect
{
    // Palette fading runs during 34 frames (17*2) since the game always alternates
    // fading the background and object palettes, with each fading 0-16.
    public int MinFadeTime => 0;
    public int MaxFadeTime => 33;

    public float Fade { get; set; }

    public void SetFadeFromTimer(int timer) => Fade = 1 - timer / (float)MaxFadeTime;

    public override void Draw(GfxRenderer renderer)
    {
        renderer.BeginRender(new RenderOptions(false, null, Camera));

        // We draw a black, faded, texture over the screen to emulate fading the palette
        if (Fade is > 0 and <= 1)
            renderer.DrawFilledRectangle(Vector2.Zero, Camera.Resolution, Color.Black * Fade);
    }
}