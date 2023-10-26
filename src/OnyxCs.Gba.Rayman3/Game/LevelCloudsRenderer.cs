using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba.Rayman3;

// For now the width is hard-coded to 256 since the map for some reason has a lot of empty space to the right and
// we do wrapping based on the entire map rather than what's loaded into vram (like the gba does).
public class LevelCloudsRenderer : IScreenRenderer
{
    public LevelCloudsRenderer(Texture2D texture)
    {
        Texture = texture;
    }

    public Texture2D Texture { get; }
    public Vector2 Size => new(256, Texture.Height);

    private void DrawCloud(GfxRenderer renderer, Vector2 position, int offsetX, int offsetY, int height)
    {
        position += new Vector2(offsetX, offsetY);
        Rectangle rect = new(0, offsetY, 256, height);

        renderer.Draw(Texture, position, rect, Color.White);
    }

    public void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position)
    {
        byte scroll0 = (byte)(GameTime.ElapsedFrames >> 1);
        byte scroll1 = (byte)(GameTime.ElapsedFrames >> 2);
        byte scroll2 = (byte)(GameTime.ElapsedFrames >> 3);

        DrawCloud(renderer, position, -scroll0, 0, 32);
        DrawCloud(renderer, position, -scroll1, 32, 88);
        DrawCloud(renderer, position, -scroll2, 120, 108);

        DrawCloud(renderer, position, 256 - scroll0, 0, 32);
        DrawCloud(renderer, position, 256 - scroll1, 32, 88);
        DrawCloud(renderer, position, 256 - scroll2, 120, 108);
    }
}