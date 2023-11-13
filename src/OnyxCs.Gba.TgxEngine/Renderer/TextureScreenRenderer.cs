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
    public Vector2 Size => new(Texture.Width, Texture.Height);

    public void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position)
    {
        renderer.Draw(Texture, position);
    }
}