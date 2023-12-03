using Microsoft.Xna.Framework;

namespace OnyxCs.Gba;

public interface IScreenRenderer
{
    Vector2 Size { get; }

    void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position, Color color);
}