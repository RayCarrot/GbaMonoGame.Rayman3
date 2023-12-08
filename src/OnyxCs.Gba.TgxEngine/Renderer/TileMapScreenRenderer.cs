using System;
using System.Collections.Generic;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba.TgxEngine;

public class TileMapScreenRenderer : IScreenRenderer
{
    public TileMapScreenRenderer(int width, int height, MapTile[] tileMap, byte[] tileSet, Palette palette, bool is8Bit)
    {
        Width = width;
        Height = height;
        TileMap = tileMap;

        TileSetRectangles = new Dictionary<int, Rectangle>();
        List<Color> texColors = new();
        const int texWidth = 32 * Constants.TileSize;
        int texX = 0;
        int texY = -Constants.TileSize;
        Color[] emptyTileRow = new Color[Constants.TileSize * texWidth];

        foreach (MapTile mapTile in tileMap)
        {
            if (mapTile.TileIndex == 0) 
                continue;
            
            int key = mapTile.TileIndex * 16 + mapTile.PaletteIndex;

            if (TileSetRectangles.ContainsKey(key)) 
                continue;
            
            if (texX % texWidth == 0)
            {
                texColors.AddRange(emptyTileRow);
                texX = 0;
                texY += Constants.TileSize;
            }

            if (is8Bit)
                TileHelpers.DrawTile_8bpp(texColors, texX, texY, texWidth, tileSet, (mapTile.TileIndex - 1) * 0x40, palette);
            else
                TileHelpers.DrawTile_4bpp(texColors, texX, texY, texWidth, tileSet, (mapTile.TileIndex - 1) * 0x20, palette, 16 * mapTile.PaletteIndex);

            TileSetRectangles.Add(key, new Rectangle(texX, texY, Constants.TileSize, Constants.TileSize));

            texX += Constants.TileSize;
        }

        TileSetTexture = new Texture2D(Engine.GraphicsDevice, texWidth, texY + Constants.TileSize);
        TileSetTexture.SetData(texColors.ToArray());
    }

    public int Width { get; }
    public int Height { get; }
    public MapTile[] TileMap { get; }

    public Texture2D TileSetTexture { get; }
    public Dictionary<int, Rectangle> TileSetRectangles { get; }

    public Vector2 Size => new(Width * Constants.TileSize, Height * Constants.TileSize);

    private Rectangle GetVisibleTilesArea(Vector2 position)
    {
        Box renderBox = new(position, Size);

        int xStart = (int)((Math.Max(0, renderBox.MinX) - renderBox.MinX) / Constants.TileSize);
        int yStart = (int)((Math.Max(0, renderBox.MinY) - renderBox.MinY) / Constants.TileSize);
        int xEnd = (int)Math.Ceiling((Math.Min((double)Engine.ScreenCamera.GameResolution.X, renderBox.MaxX) - renderBox.MinX) / Constants.TileSize);
        int yEnd = (int)Math.Ceiling((Math.Min((double)Engine.ScreenCamera.GameResolution.Y, renderBox.MaxY) - renderBox.MinY) / Constants.TileSize);

        return new Rectangle(xStart, yStart, xEnd - xStart, yEnd - yStart);
    }

    public void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position, Color color)
    {
        Rectangle visibleTilesArea = GetVisibleTilesArea(position);

        float absTileY = position.Y + visibleTilesArea.Y * Constants.TileSize;

        for (int tileY = visibleTilesArea.Top; tileY < visibleTilesArea.Bottom; tileY++)
        {
            float absTileX = position.X + visibleTilesArea.X * Constants.TileSize;

            for (int tileX = visibleTilesArea.Left; tileX < visibleTilesArea.Right; tileX++)
            {
                MapTile tile = TileMap[tileY * Width + tileX];

                if (tile.TileIndex != 0)
                {
                    SpriteEffects effects = SpriteEffects.None;

                    if (tile.FlipX)
                        effects |= SpriteEffects.FlipHorizontally;
                    if (tile.FlipY)
                        effects |= SpriteEffects.FlipVertically;

                    renderer.Draw(TileSetTexture, new Vector2(absTileX, absTileY), TileSetRectangles[tile.TileIndex * 16 + tile.PaletteIndex], effects, color);
                }

                absTileX += Constants.TileSize;
            }

            absTileY += Constants.TileSize;
        }
    }
}