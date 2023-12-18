using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba.TgxEngine;

public class TextureScreenRenderer : IScreenRenderer
{
    public TextureScreenRenderer(Texture2D texture)
    {
        Texture = texture;
    }

    public Texture2D Texture { get; }

    public Vector2 GetSize(GfxScreen screen) => new(Texture.Width, Texture.Height);
    public void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position, Color color)
    {
        renderer.Draw(Texture, position, color);
    }
}