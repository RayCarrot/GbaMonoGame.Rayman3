using System;
using System.Collections.Generic;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame.TgxEngine;

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
        ReplacedTiles = new Dictionary<int, int>();
    }

    private Dictionary<int, Texture2D> TileTextures { get; }
    private Dictionary<int, int> ReplacedTiles { get; }

    public int Width { get; }
    public int Height { get; }
    public MapTile[] TileMap { get; }
    public byte[] TileSet { get; }
    public Palette Palette { get; }
    public bool Is8Bit { get; }

    private Rectangle GetVisibleTilesArea(Vector2 position, GfxScreen screen)
    {
        Box renderBox = new(position, GetSize(screen));

        int xStart = (int)((Math.Max(0, renderBox.MinX) - renderBox.MinX) / Constants.TileSize);
        int yStart = (int)((Math.Max(0, renderBox.MinY) - renderBox.MinY) / Constants.TileSize);
        int xEnd = (int)Math.Ceiling((Math.Min(screen.Camera.Resolution.X, renderBox.MaxX) - renderBox.MinX) / Constants.TileSize);
        int yEnd = (int)Math.Ceiling((Math.Min(screen.Camera.Resolution.Y, renderBox.MaxY) - renderBox.MinY) / Constants.TileSize);

        return new Rectangle(xStart, yStart, xEnd - xStart, yEnd - yStart);
    }

    private Texture2D CreateTileTexture(int tileIndex, int paletteIndex)
    {
        if (tileIndex == 0)
            return null;

        return new TiledTexture2D(TileSet, tileIndex - 1, paletteIndex, Palette, Is8Bit);
    }

    public void ReplaceTile(int originalTileIndex, int newTileIndex)
    {
        ReplacedTiles[originalTileIndex] = newTileIndex;
    }

    public Vector2 GetSize(GfxScreen screen) => new(Width * Constants.TileSize, Height * Constants.TileSize);

    public void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position, Color color)
    {
        Rectangle visibleTilesArea = GetVisibleTilesArea(position, screen);

        float absTileY = position.Y + visibleTilesArea.Y * Constants.TileSize;

        for (int tileY = visibleTilesArea.Top; tileY < visibleTilesArea.Bottom; tileY++)
        {
            float absTileX = position.X + visibleTilesArea.X * Constants.TileSize;

            for (int tileX = visibleTilesArea.Left; tileX < visibleTilesArea.Right; tileX++)
            {
                MapTile tile = TileMap[tileY * Width + tileX];

                int tileIndex = tile.TileIndex;

                if (ReplacedTiles.TryGetValue(tileIndex, out int newTileIndex))
                    tileIndex = newTileIndex;

                int texKey = tileIndex * 16 + tile.PaletteIndex;

                if (!TileTextures.TryGetValue(texKey, out Texture2D tex))
                {
                    tex = CreateTileTexture(tileIndex, tile.PaletteIndex);
                    TileTextures.Add(texKey, tex);
                }

                if (tex != null)
                {
                    SpriteEffects effects = SpriteEffects.None;

                    if (tile.FlipX)
                        effects |= SpriteEffects.FlipHorizontally;
                    if (tile.FlipY)
                        effects |= SpriteEffects.FlipVertically;

                    renderer.Draw(tex, new Vector2(absTileX, absTileY), effects, color);
                }

                absTileX += Constants.TileSize;
            }

            absTileY += Constants.TileSize;
        }
    }

    public void Dispose()
    {
        foreach (Texture2D tex in TileTextures.Values)
            tex?.Dispose();
    }
}