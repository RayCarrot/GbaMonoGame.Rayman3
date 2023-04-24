using System;
using System.Diagnostics;
using System.Linq;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OnyxCs.Gba.Sdk;

/// <summary>
/// Handles updating and drawing the GBA screen similarly to how the PPU would
/// </summary>
public partial class MonoGameVram : Vram
{
    public MonoGameVram(GraphicsDevice graphicsDevice, int width, int height)
    {
        SetSize(graphicsDevice, width, height);

        SpritePaletteManager = new MonoGamePaletteManager();
        BackgroundPaletteManager = new MonoGamePaletteManager();
    }

    private Texture2D _frame;
    private Color[] _imgBuffer;

    public override MonoGamePaletteManager SpritePaletteManager { get; }
    public override MonoGamePaletteManager BackgroundPaletteManager { get; }

    public Rectangle VisibleScreenRect { get; private set; }
    public Rectangle ImgBufferRect { get; private set; }

    private void Clear() => Array.Fill(_imgBuffer, Color.Black);

    private Rectangle GetVisibleTilesArea(Vec2Int pos, int tilesWidth, int tilesHeight)
    {
        Rectangle rect = new(
            x: pos.X + VisibleScreenRect.X, 
            y: pos.Y + VisibleScreenRect.Y, 
            width: tilesWidth * Constants.TileSize, 
            height: tilesHeight * Constants.TileSize);

        int xStart = (Math.Max(VisibleScreenRect.Left, rect.Left) - rect.X) / Constants.TileSize;
        int yStart = (Math.Max(VisibleScreenRect.Top, rect.Top) - rect.Y) / Constants.TileSize;
        int xEnd = (int)Math.Ceiling((Math.Min(VisibleScreenRect.Right, rect.Right) - rect.X) / (float)Constants.TileSize);
        int yEnd = (int)Math.Ceiling((Math.Min(VisibleScreenRect.Bottom, rect.Bottom) - rect.Y) / (float)Constants.TileSize);

        return new Rectangle(xStart, yStart, xEnd - xStart, yEnd - yStart);
    }

    public void SetSize(GraphicsDevice graphicsDevice, int width, int height)
    {
        // We add an overflow border around the screen so that when
        // rendering tiles we don't have to worry about part of them
        // being off screen.
        const int overflow = Constants.TileSize - 1;
        VisibleScreenRect = new Rectangle(overflow, overflow, width, height);
        ImgBufferRect = new Rectangle(0, 0, width + overflow * 2, height + overflow * 2);

        _frame = new Texture2D(graphicsDevice, ImgBufferRect.Width, ImgBufferRect.Height, false, SurfaceFormat.Color);
        _imgBuffer = new Color[ImgBufferRect.Width * ImgBufferRect.Height];
    }

    public void Update()
    {
#if DEBUG
        Stopwatch sw = Stopwatch.StartNew();
#endif

        // Clear the screen
        Clear();

        // Draw objects
        for (int i = 3; i >= 0; i--)
        {
            // Draw backgrounds
            foreach (Background bg in GetBackgrounds().Where(x => x.Priority == i))
                DrawBackground(bg);

            // Draw sprites
            foreach (Sprite sprite in GetSprites().Where(x => x.Priority == i).Reverse())
                DrawSprite(sprite);
        }

        // Update the frame texture
        _frame.SetData(_imgBuffer);

#if DEBUG
        sw.Stop();
        Debug.WriteLine($"Screen update: {sw.ElapsedTicks / 10_000f:N3}");
#endif
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 scale)
    {
        spriteBatch.Draw(_frame, position, VisibleScreenRect, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
    }
}