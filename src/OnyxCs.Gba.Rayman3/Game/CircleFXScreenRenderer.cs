using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba.Rayman3;

public class CircleFXScreenRenderer : IScreenRenderer
{
    static CircleFXScreenRenderer()
    {
        // In the game this table is pre-calculated (located at 0x0820e9a4 in the EU version), but we can calculate it during runtime
        RadiusWidthsTable = new byte[MaxRadius][];
        for (int i = 0; i < RadiusWidthsTable.Length; i++)
        {
            int radius = i + 1;

            byte[] widths = new byte[radius];
            RadiusWidthsTable[i] = widths;

            for (int y = 0; y < radius; y++)
                widths[radius - y - 1] = (byte)Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(y, 2));
        }
    }

    private const int MaxRadius = 252;

    private static Texture2D[] CachedCircleTextures { get; } = new Texture2D[MaxRadius];
    private static byte[][] RadiusWidthsTable { get; }

    private static Texture2D GetCircleTexture(int radius)
    {
        if (CachedCircleTextures[radius - 1] == null)
        {
            Texture2D tex = new(Engine.GraphicsDevice, radius, radius);

            Color[] colors = new Color[radius * radius];

            for (int y = 0; y < radius; y++)
                Array.Fill(colors, Color.White, y * radius, radius - RadiusWidthsTable[radius - 1][y]);

            tex.SetData(colors);

            CachedCircleTextures[radius - 1] = tex;
        }

        return CachedCircleTextures[radius - 1];
    }

    public Vector2 Size => Engine.ScreenCamera.ScaledGameResolution;

    public int Radius { get; set; }
    public Vector2 CirclePosition { get; set; }
    
    public void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position, Color color)
    {
        Vector2 pos = CirclePosition - new Vector2(Radius);

        if (Radius != 0)
        {
            // Draw circle
            Texture2D tex = GetCircleTexture(Radius);
            renderer.Draw(tex, pos + new Vector2(0, 0), SpriteEffects.None, Color.Black); // Top-left
            renderer.Draw(tex, pos + new Vector2(Radius, 0), SpriteEffects.FlipHorizontally, Color.Black); // Top-right
            renderer.Draw(tex, pos + new Vector2(0, Radius), SpriteEffects.FlipVertically, Color.Black); // Bottom-left
            renderer.Draw(tex, pos + new Vector2(Radius, Radius), SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, Color.Black); // Bottom-right
        }

        Vector2 res = Engine.ScreenCamera.ScaledGameResolution;

        // Draw black around circle to fill screen
        renderer.DrawFilledRectangle(Vector2.Zero, new Vector2(res.X, pos.Y), Color.Black); // Top
        renderer.DrawFilledRectangle(new Vector2(0, pos.Y + Radius * 2), new Vector2(res.X, res.Y - (pos.Y + Radius * 2)), Color.Black); // Bottom
        renderer.DrawFilledRectangle(new Vector2(0, pos.Y), new Vector2(pos.X, Radius * 2), Color.Black); // Left
        renderer.DrawFilledRectangle(new Vector2(pos.X + Radius * 2, pos.Y), new Vector2(res.X - (pos.X + Radius * 2), Radius * 2), Color.Black); // Right
    }
}