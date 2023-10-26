namespace OnyxCs.Gba;

public class GfxScreen
{
    public GfxScreen(int id)
    {
        Id = id;
    }

    public int Id { get; }

    public int Priority { get; set; } // 0-3
    public bool Wrap { get; set; } // Overflow mode
    public bool Is8Bit { get; set; } // Color mode
    public Vector2 Offset { get; set; }
    public bool IsEnabled { get; set; }

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

            for (float y = startY; y < Gfx.GfxCamera.GameResolution.Y; y += size.Y)
            {
                for (float x = startX; x < Gfx.GfxCamera.GameResolution.X; x += size.X)
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