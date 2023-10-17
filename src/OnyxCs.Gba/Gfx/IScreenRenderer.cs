using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba;

public interface IScreenRenderer
{
    Vector2 Size { get; }

    void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position);
}