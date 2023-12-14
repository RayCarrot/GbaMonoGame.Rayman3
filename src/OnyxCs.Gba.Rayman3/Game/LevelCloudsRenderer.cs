using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba.Rayman3;

// For now the width is hard-coded to 256 since the map for some reason has a lot of empty space to the right and
// we do wrapping based on the entire map rather than what's loaded into vram (like the gba does).
public class LevelCloudsRenderer : IScreenRenderer
{
    public LevelCloudsRenderer(Texture2D texture, int[] splits)
    {
        Texture = texture;
        Splits = splits;
    }

    private int[] Splits { get; }

    public Texture2D Texture { get; }
    public Vector2 Size => new(256, Texture.Height);

    private void DrawCloud(GfxRenderer renderer, Vector2 position, Color color, int offsetX, int offsetY, int height)
    {
        position += new Vector2(offsetX, offsetY);
        Rectangle rect = new(0, offsetY, 256, height);

        renderer.Draw(Texture, position, rect, color);
    }

    public void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position, Color color)
    {
        byte[] scrolls = 
        {
            (byte)(GameTime.ElapsedFrames >> 1),
            (byte)(GameTime.ElapsedFrames >> 2),
            (byte)(GameTime.ElapsedFrames >> 3),
        };

        int offsetY = 0;
        for (int i = 0; i < Splits.Length; i++)
        {
            DrawCloud(renderer, position, color, -scrolls[i], offsetY, Splits[i] - offsetY);
            offsetY = Splits[i];
        }

        // Wrap
        offsetY = 0;
        for (int i = 0; i < Splits.Length; i++)
        {
            DrawCloud(renderer, position, color, 256 - scrolls[i], offsetY, Splits[i] - offsetY);
            offsetY = Splits[i];
        }
    }
}