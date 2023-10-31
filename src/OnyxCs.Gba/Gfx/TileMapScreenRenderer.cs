using System;
using System.Collections.Generic;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba;

public class TileMapScreenRenderer : IScreenRenderer
{
    public TileMapScreenRenderer(int width, int height, MapTile[] tileMap, byte[] tileSet, Palette palette, bool is8Bit)
    {
        Width = width;
        Height = height;
        TileMap = tileMap;
        TileSet = tileSet;
        Palette = palette;
        Is8Bit = is8Bit;

        TileTextures = new Dictionary<int, Texture2D>();
    }

    public int Width { get; }
    public int Height { get; }
    public MapTile[] TileMap { get; }
    public byte[] TileSet { get; }
    public Palette Palette { get; }
    public bool Is8Bit { get; }
    public Dictionary<int, Texture2D> TileTextures { get; }

    public Vector2 Size => new(Width * Constants.TileSize, Height * Constants.TileSize);

    private Rectangle GetVisibleTilesArea(Vector2 position)
    {
        Rectangle rect = new(position.ToPoint(), Size.ToPoint());

        int xStart = (Math.Max(0, rect.Left) - rect.X) / Constants.TileSize;
        int yStart = (Math.Max(0, rect.Top) - rect.Y) / Constants.TileSize;
        int xEnd = (int)Math.Ceiling((Math.Min((double)Gfx.GfxCamera.GameResolution.X, rect.Right) - rect.X) / Constants.TileSize);
        int yEnd = (int)Math.Ceiling((Math.Min((double)Gfx.GfxCamera.GameResolution.Y, rect.Bottom) - rect.Y) / Constants.TileSize);

        return new Rectangle(xStart, yStart, xEnd - xStart, yEnd - yStart);
    }

    private Texture2D CreateTileTexture(MapTile tile)
    {
        if (tile.TileIndex == 0)
            return null;

        return new TiledTexture2D(TileSet, tile.TileIndex - 1, tile.PaletteIndex, Palette, Is8Bit);
    }

    public void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position)
    {
        Rectangle visibleTilesArea = GetVisibleTilesArea(position);

        float absTileY = position.Y + visibleTilesArea.Y * Constants.TileSize;

        for (int tileY = visibleTilesArea.Top; tileY < visibleTilesArea.Bottom; tileY++)
        {
            float absTileX = position.X + visibleTilesArea.X * Constants.TileSize;

            for (int tileX = visibleTilesArea.Left; tileX < visibleTilesArea.Right; tileX++)
            {
                MapTile tile = TileMap[tileY * Width + tileX];

                int texKey = tile.TileIndex * 16 + tile.TileIndex;

                if (!TileTextures.TryGetValue(texKey, out Texture2D tex))
                {
                    tex = CreateTileTexture(tile);
                    TileTextures.Add(texKey, tex);
                }

                if (tex != null)
                {
                    SpriteEffects effects = SpriteEffects.None;

                    if (tile.FlipX)
                        effects |= SpriteEffects.FlipHorizontally;
                    if (tile.FlipY)
                        effects |= SpriteEffects.FlipVertically;

                    renderer.Draw(tex, new Rectangle(new Vector2(absTileX, absTileY).ToPoint(), tex.Bounds.Size), null, effects, Color.White);
                }

                absTileX += Constants.TileSize;
            }

            absTileY += Constants.TileSize;
        }
    }
}