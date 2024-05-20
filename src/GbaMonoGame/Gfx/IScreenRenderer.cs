using Microsoft.Xna.Framework;

namespace GbaMonoGame;

public interface IScreenRenderer
{
    Vector2 GetSize(GfxScreen screen);
    void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position, Color color);
}