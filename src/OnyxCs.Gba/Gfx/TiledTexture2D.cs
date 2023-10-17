using System.Runtime.CompilerServices;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba;

public class TiledTexture2D : Texture2D
{
    #region Constructor

    public TiledTexture2D(byte[] tileSet, int tileIndex, int paletteIndex, Palette palette, bool is8Bit) : 
        base(Gfx.GraphicsDevice, Constants.TileSize, Constants.TileSize)
    {
        Color[] texColors = new Color[Width * Height];

        if (is8Bit)
        {
            DrawTile_8bpp(texColors, 0, 0, tileSet, tileIndex * 0x40, palette);
        }
        else
        {
            DrawTile_4bpp(texColors, 0, 0, tileSet, tileIndex * 0x20, palette, paletteIndex * 16);
        }

        SetData(texColors);
    }

    public TiledTexture2D(int width, int height, byte[] tileSet, MapTile[] tileMap, Palette palette, bool is8Bit) : 
        base(Gfx.GraphicsDevice, width * Constants.TileSize, height * Constants.TileSize)
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
                        DrawTile_8bpp_FlipXY(texColors, absTileX, absTileY, tileSet, tilePixelIndex, palette);
                    else if (tile.FlipX)
                        DrawTile_8bpp_FlipX(texColors, absTileX, absTileY, tileSet, tilePixelIndex, palette);
                    else if (tile.FlipY)
                        DrawTile_8bpp_FlipY(texColors, absTileX, absTileY, tileSet, tilePixelIndex, palette);
                    else
                        DrawTile_8bpp(texColors, absTileX, absTileY, tileSet, tilePixelIndex, palette);

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
                        DrawTile_4bpp_FlipXY(texColors, absTileX, absTileY, tileSet, tilePixelIndex, palette, palOffset);
                    else if (tile.FlipX)
                        DrawTile_4bpp_FlipX(texColors, absTileX, absTileY, tileSet, tilePixelIndex, palette, palOffset);
                    else if (tile.FlipY)
                        DrawTile_4bpp_FlipY(texColors, absTileX, absTileY, tileSet, tilePixelIndex, palette, palOffset);
                    else
                        DrawTile_4bpp(texColors, absTileX, absTileY, tileSet, tilePixelIndex, palette, palOffset);

                    absTileX += Constants.TileSize;
                }

                absTileY += Constants.TileSize;
            }
        }

        SetData(texColors);
    }

    #endregion

    #region Draw Tile 4bpp

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawTile_4bpp(Color[] texColors, int xPos, int yPos, byte[] tileSet, int tileSetIndex, Palette pal, int palOffset)
    {
        int imgBufferOffset = yPos * Width + xPos;

        for (int y = 0; y < Constants.TileSize; y++)
        {
            for (int x = 0; x < Constants.TileSize; x += 2)
            {
                int value = tileSet[tileSetIndex];

                int v1 = value & 0xF;
                int v2 = value >> 4;

                // Set the pixel if not 0 (transparent)
                if (v1 != 0)
                    texColors[imgBufferOffset] = pal.Colors[palOffset + v1];
                imgBufferOffset++;

                if (v2 != 0)
                    texColors[imgBufferOffset] = pal.Colors[palOffset + v2];
                imgBufferOffset++;

                tileSetIndex++;
            }

            imgBufferOffset += Width - Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawTile_4bpp_FlipX(Color[] texColors, int xPos, int yPos, byte[] tileSet, int tileSetIndex, Palette pal, int palOffset)
    {
        int imgBufferOffset = yPos * Width + xPos + Constants.TileSize - 1;

        for (int y = 0; y < Constants.TileSize; y++)
        {
            for (int x = 0; x < Constants.TileSize; x += 2)
            {
                int value = tileSet[tileSetIndex];

                int v1 = value >> 4;
                int v2 = value & 0xF;

                // Set the pixel if not 0 (transparent)
                if (v1 != 0)
                    texColors[imgBufferOffset] = pal.Colors[palOffset + v1];
                imgBufferOffset--;

                if (v2 != 0)
                    texColors[imgBufferOffset] = pal.Colors[palOffset + v2];
                imgBufferOffset--;

                tileSetIndex++;
            }

            imgBufferOffset += Width + Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawTile_4bpp_FlipY(Color[] texColors, int xPos, int yPos, byte[] tileSet, int tileSetIndex, Palette pal, int palOffset)
    {
        int imgBufferOffset = (yPos + Constants.TileSize - 1) * Width + xPos;

        for (int y = 0; y < Constants.TileSize; y++)
        {
            for (int x = 0; x < Constants.TileSize; x += 2)
            {
                int value = tileSet[tileSetIndex];

                int v1 = value & 0xF;
                int v2 = value >> 4;

                // Set the pixel if not 0 (transparent)
                if (v1 != 0)
                    texColors[imgBufferOffset] = pal.Colors[palOffset + v1];
                imgBufferOffset++;

                if (v2 != 0)
                    texColors[imgBufferOffset] = pal.Colors[palOffset + v2];
                imgBufferOffset++;

                tileSetIndex++;
            }

            imgBufferOffset -= Width + Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawTile_4bpp_FlipXY(Color[] texColors, int xPos, int yPos, byte[] tileSet, int tileSetIndex, Palette pal, int palOffset)
    {
        int imgBufferOffset = (yPos + Constants.TileSize - 1) * Width + xPos + Constants.TileSize - 1;

        for (int y = 0; y < Constants.TileSize; y++)
        {
            for (int x = 0; x < Constants.TileSize; x += 2)
            {
                int value = tileSet[tileSetIndex];

                int v1 = value >> 4;
                int v2 = value & 0xF;

                // Set the pixel if not 0 (transparent)
                if (v1 != 0)
                    texColors[imgBufferOffset] = pal.Colors[palOffset + v1];
                imgBufferOffset--;

                if (v2 != 0)
                    texColors[imgBufferOffset] = pal.Colors[palOffset + v2];
                imgBufferOffset--;

                tileSetIndex++;
            }

            imgBufferOffset -= Width - Constants.TileSize;
        }
    }

    #endregion

    #region Draw Tile 8bpp

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawTile_8bpp(Color[] texColors, int xPos, int yPos, byte[] tileSet, int tileSetIndex, Palette pal)
    {
        int imgBufferOffset = yPos * Width + xPos;

        for (int y = 0; y < Constants.TileSize; y++)
        {
            for (int x = 0; x < Constants.TileSize; x++)
            {
                int value = tileSet[tileSetIndex];

                // Set the pixel if not 0 (transparent)
                if (value != 0)
                    texColors[imgBufferOffset] = pal.Colors[value];

                imgBufferOffset++;
                tileSetIndex++;
            }

            imgBufferOffset += Width - Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawTile_8bpp_FlipX(Color[] texColors, int xPos, int yPos, byte[] tileSet, int tileSetIndex, Palette pal)
    {
        int imgBufferOffset = yPos * Width + xPos + Constants.TileSize - 1;

        for (int y = 0; y < Constants.TileSize; y++)
        {
            for (int x = 0; x < Constants.TileSize; x++)
            {
                int value = tileSet[tileSetIndex];

                // Set the pixel if not 0 (transparent)
                if (value != 0)
                    texColors[imgBufferOffset] = pal.Colors[value];

                imgBufferOffset--;
                tileSetIndex++;
            }

            imgBufferOffset += Width + Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawTile_8bpp_FlipY(Color[] texColors, int xPos, int yPos, byte[] tileSet, int tileSetIndex, Palette pal)
    {
        int imgBufferOffset = (yPos + Constants.TileSize - 1) * Width + xPos;

        for (int y = 0; y < Constants.TileSize; y++)
        {
            for (int x = 0; x < Constants.TileSize; x++)
            {
                int value = tileSet[tileSetIndex];

                // Set the pixel if not 0 (transparent)
                if (value != 0)
                    texColors[imgBufferOffset] = pal.Colors[value];

                imgBufferOffset++;
                tileSetIndex++;
            }

            imgBufferOffset -= Width + Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawTile_8bpp_FlipXY(Color[] texColors, int xPos, int yPos, byte[] tileSet, int tileSetIndex, Palette pal)
    {
        int imgBufferOffset = (yPos + Constants.TileSize - 1) * Width + xPos + Constants.TileSize - 1;

        for (int y = 0; y < Constants.TileSize; y++)
        {
            for (int x = 0; x < Constants.TileSize; x++)
            {
                int value = tileSet[tileSetIndex];

                // Set the pixel if not 0 (transparent)
                if (value != 0)
                    texColors[imgBufferOffset] = pal.Colors[value];

                imgBufferOffset--;
                tileSetIndex++;
            }

            imgBufferOffset -= Width - Constants.TileSize;
        }
    }

    #endregion
}