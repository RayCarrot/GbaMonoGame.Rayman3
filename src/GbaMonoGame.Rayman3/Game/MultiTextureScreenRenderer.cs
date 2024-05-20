using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame.Rayman3;

public class MultiTextureScreenRenderer : IScreenRenderer
{
    public MultiTextureScreenRenderer(Texture2D[] textures)
    {
        Textures = textures;
    }

    public Texture2D[] Textures { get; }
    public int CurrentTextureIndex { get; set; }

    public Vector2 GetSize(GfxScreen screen) => new(Textures[CurrentTextureIndex].Width, Textures[CurrentTextureIndex].Height);

    public void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position, Color color)
    {
        renderer.Draw(Textures[CurrentTextureIndex], position, color);
    }
}