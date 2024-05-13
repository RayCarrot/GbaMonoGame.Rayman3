using System;
using Microsoft.Xna.Framework;

namespace GbaMonoGame;

/// <summary>
/// A graphics screen. This is the equivalent of a GBA background.
/// </summary>
public class GfxScreen : IDisposable
{
    public GfxScreen(int id)
    {
        Id = id;
    }

    /// <summary>
    /// The screen id, a value between 0 and 3.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// The screen drawing priority, a value between 0 and 3, or -1 for always on top
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Indicates the overflow mode for the screen, if it should wrap its content or not.
    /// </summary>
    public bool Wrap { get; set; }

    /// <summary>
    /// Indicates the color mode for the screen, if it's 8-bit or 4-bit.
    /// </summary>
    public bool? Is8Bit { get; set; }

    /// <summary>
    /// The scrolled screen offset.
    /// </summary>
    public Vector2 Offset { get; set; }

    /// <summary>
    /// The camera to render the screen to.
    /// </summary>
    public GfxCamera Camera { get; set; } = Engine.ScreenCamera;

    public bool IsAlphaBlendEnabled { get; set; }
    public float Alpha { get; set; }
    public float GbaAlpha
    {
        get => Alpha * 16;
        set => Alpha = value / 16;
    }

    /// <summary>
    /// Indicates if the screen is enabled and should be drawn.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// The renderer to draw the screen with. This is used in place of tilemap and tileset
    /// data since the screen might need to be drawn differently in different situations.
    /// </summary>
    public IScreenRenderer Renderer { get; set; }

    public void Draw(GfxRenderer renderer)
    {
        if (Renderer == null)
            return;

        Color color = Color.White;

        renderer.BeginRender(new RenderOptions(IsAlphaBlendEnabled, Camera));

        if (IsAlphaBlendEnabled)
            color = new Color(color, Alpha);

        if (Wrap)
        {
            Vector2 size = Renderer.GetSize(this);
            Vector2 wrappedPos = new(-Offset.X % size.X, -Offset.Y % size.Y);

            float startX = 0 - size.X + (wrappedPos.X == 0 ? size.X : wrappedPos.X);
            float startY = 0 - size.Y + (wrappedPos.Y == 0 ? size.Y : wrappedPos.Y);

            for (float y = startY; y < Camera.Resolution.Y; y += size.Y)
            {
                for (float x = startX; x < Camera.Resolution.X; x += size.X)
                {
                    Renderer?.Draw(renderer, this, new Vector2(x, y), color);
                }
            }
        }
        else
        {
            Renderer?.Draw(renderer, this, -Offset, color);
        }
    }

    public void Dispose()
    {
        Renderer?.Dispose();
    }
}