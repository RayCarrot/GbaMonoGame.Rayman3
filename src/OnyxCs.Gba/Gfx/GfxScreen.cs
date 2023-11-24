namespace OnyxCs.Gba;

/// <summary>
/// A graphics screen. This is the equivalent of a GBA background.
/// </summary>
public class GfxScreen
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
    /// The screen drawing priority, a value between 0 and 3.
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

        if (Wrap)
        {
            Vector2 size = Renderer.Size;
            Vector2 wrappedPos = new(-Offset.X % size.X, -Offset.Y % size.Y);

            float startX = 0 - size.X + (wrappedPos.X == 0 ? size.X : wrappedPos.X);
            float startY = 0 - size.Y + (wrappedPos.Y == 0 ? size.Y : wrappedPos.Y);

            for (float y = startY; y < Engine.ScreenCamera.GameResolution.Y; y += size.Y)
            {
                for (float x = startX; x < Engine.ScreenCamera.GameResolution.X; x += size.X)
                {
                    Renderer?.Draw(renderer, this, new Vector2(x, y));
                }
            }
        }
        else
        {
            Renderer?.Draw(renderer, this, -Offset);
        }
    }
}