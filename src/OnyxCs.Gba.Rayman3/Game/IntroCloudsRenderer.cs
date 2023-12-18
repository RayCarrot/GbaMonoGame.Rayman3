using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba.Rayman3;

public class IntroCloudsRenderer : IScreenRenderer
{
    public IntroCloudsRenderer(Texture2D texture)
    {
        Texture = texture;
    }

    public Texture2D Texture { get; }

    public Vector2 GetSize(GfxScreen screen) => new(Texture.Width, Texture.Height);
    private void DrawCloud(GfxRenderer renderer, Vector2 position, Color color, int index, float offsetX)
    {
        position += new Vector2(offsetX, 85 * index);
        Rectangle rect = new(0, 85 * index, Texture.Width, 85);

        renderer.Draw(Texture, position, rect, color);
    }

    public void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position, Color color)
    {
        byte scroll0 = (byte)(GameTime.ElapsedFrames >> 1);
        byte scroll1 = (byte)~(byte)(GameTime.ElapsedFrames >> 2);
        byte scroll2 = (byte)(GameTime.ElapsedFrames >> 3);

        DrawCloud(renderer, position, color, 0, -scroll0);
        DrawCloud(renderer, position, color, 1, -scroll1);
        DrawCloud(renderer, position, color, 2, -scroll2);

        DrawCloud(renderer, position, color, 0, Texture.Width - scroll0);
        DrawCloud(renderer, position, color, 1, Texture.Width - scroll1);
        DrawCloud(renderer, position, color, 2, Texture.Width - scroll2);
    }
}