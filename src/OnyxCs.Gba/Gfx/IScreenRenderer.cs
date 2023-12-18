using Microsoft.Xna.Framework;

namespace OnyxCs.Gba;

public interface IScreenRenderer
{
    Vector2 GetSize(GfxScreen screen);
    void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position, Color color);
}