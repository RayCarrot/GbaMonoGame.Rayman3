﻿using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame;

public class TiledTexture2D : Texture2D
{
    public TiledTexture2D(byte[] tileSet, int tileIndex, int paletteIndex, Palette palette, bool is8Bit) :
        base(Engine.GraphicsDevice, Constants.TileSize, Constants.TileSize)
    {
        Color[] texColors = new Color[Width * Height];

        if (is8Bit)
        {
            int tilePixelIndex = tileIndex * 0x40;
            DrawHelpers.DrawTile_8bpp(texColors, 0, 0, Width, tileSet, ref tilePixelIndex, palette);
        }
        else
        {
            int tilePixelIndex = tileIndex * 0x20;
            DrawHelpers.DrawTile_4bpp(texColors, 0, 0, Width, tileSet, ref tilePixelIndex, palette, paletteIndex * 16);
        }

        SetData(texColors);
    }

    public TiledTexture2D(int width, int height, byte[] tileSet, MapTile[] tileMap, Palette palette, bool is8Bit) :
        base(Engine.GraphicsDevice, width * Constants.TileSize, height * Constants.TileSize)
    {
        Color[] texColors = new Color[Width * Height];

        if (is8Bit)
        {
            int absTileY = 0;

            for (int tileY = 0; tileY < height; tileY++)
            {
                int absTileX = 0;

                for (int tileX = 0; tileX < width; tileX++)
                {
                    MapTile tile = tileMap[tileY * width + tileX];

                    int tilePixelIndex = tile.TileIndex * 0x40;

                    if (tile.FlipX && tile.FlipY)
                        DrawHelpers.DrawTile_8bpp_FlipXY(texColors, absTileX, absTileY, Width, tileSet, ref tilePixelIndex, palette);
                    else if (tile.FlipX)
                        DrawHelpers.DrawTile_8bpp_FlipX(texColors, absTileX, absTileY, Width, tileSet, ref tilePixelIndex, palette);
                    else if (tile.FlipY)
                        DrawHelpers.DrawTile_8bpp_FlipY(texColors, absTileX, absTileY, Width, tileSet, ref tilePixelIndex, palette);
                    else
                        DrawHelpers.DrawTile_8bpp(texColors, absTileX, absTileY, Width, tileSet, ref tilePixelIndex, palette);

                    absTileX += Constants.TileSize;
                }

                absTileY += Constants.TileSize;
            }
        }
        else
        {
            int absTileY = 0;

            for (int tileY = 0; tileY < height; tileY++)
            {
                int absTileX = 0;

                for (int tileX = 0; tileX < width; tileX++)
                {
                    MapTile tile = tileMap[tileY * width + tileX];

                    int tilePixelIndex = tile.TileIndex * 0x20;
                    int palOffset = tile.PaletteIndex * 16;

                    if (tile.FlipX && tile.FlipY)
                        DrawHelpers.DrawTile_4bpp_FlipXY(texColors, absTileX, absTileY, Width, tileSet, ref tilePixelIndex, palette, palOffset);
                    else if (tile.FlipX)
                        DrawHelpers.DrawTile_4bpp_FlipX(texColors, absTileX, absTileY, Width, tileSet, ref tilePixelIndex, palette, palOffset);
                    else if (tile.FlipY)
                        DrawHelpers.DrawTile_4bpp_FlipY(texColors, absTileX, absTileY, Width, tileSet, ref tilePixelIndex, palette, palOffset);
                    else
                        DrawHelpers.DrawTile_4bpp(texColors, absTileX, absTileY, Width, tileSet, ref tilePixelIndex, palette, palOffset);

                    absTileX += Constants.TileSize;
                }

                absTileY += Constants.TileSize;
            }
        }

        SetData(texColors);
    }
}