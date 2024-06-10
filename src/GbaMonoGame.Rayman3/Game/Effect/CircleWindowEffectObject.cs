using System;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame.Rayman3;

public class CircleWindowEffectObject : EffectObject
{
    static CircleWindowEffectObject()
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

    private void DrawCirclePart(Texture2D texture, Vector2 position, bool flipX, bool flipY)
    {
        Gfx.AddSprite(new Sprite
        {
            Texture = texture,
            Position = position,
            FlipX = flipX,
            FlipY = flipY,
            Priority = SpritePriority,
            Color = Color.Black,
            Camera = Engine.ScreenCamera,
        });
    }

    public int Radius { get; set; }
    public Vector2 CirclePosition { get; set; }
    
    public override void Execute(Action<short> soundEventCallback)
    {
        // TODO: Add option to use this on N-Gage
        if (Engine.Settings.Platform == Platform.NGage)
            return;

        // TODO: Could maybe write this a bit cleaner, but essentially the position is in the tgx camera, so we need to use those coordinates
        Vector2 pos = CirclePosition / Engine.Config.PlayfieldCameraScale;

        pos -= new Vector2(Radius);

        if (Radius != 0)
        {
            // Draw circle
            Texture2D tex = GetCircleTexture(Radius);
            DrawCirclePart(tex, pos + new Vector2(0, 0), false, false); // Top-left
            DrawCirclePart(tex, pos + new Vector2(Radius, 0), true, false); // Top-right
            DrawCirclePart(tex, pos + new Vector2(0, Radius), false, true); // Bottom-left
            DrawCirclePart(tex, pos + new Vector2(Radius, Radius), true, true); // Bottom-right
        }

        Vector2 res = Engine.GameViewPort.GameResolution;

        // Draw black around circle to fill screen
        DrawRectangle(Vector2.Zero, new Vector2(res.X, pos.Y), Color.Black); // Top
        DrawRectangle(new Vector2(0, pos.Y + Radius * 2), new Vector2(res.X, res.Y - (pos.Y + Radius * 2)), Color.Black); // Bottom
        DrawRectangle(new Vector2(0, pos.Y), new Vector2(pos.X, Radius * 2), Color.Black); // Left
        DrawRectangle(new Vector2(pos.X + Radius * 2, pos.Y), new Vector2(res.X - (pos.X + Radius * 2), Radius * 2), Color.Black); // Right
    }
}