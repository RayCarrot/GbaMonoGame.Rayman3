using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba;

public static class TileHelpers
{
    #region Draw Tile 4bpp

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawTile_4bpp(IList<Color> texColors, int xPos, int yPos, int width, byte[] tileSet, int tileSetIndex, Palette pal, int palOffset)
    {
        int imgBufferOffset = yPos * width + xPos;

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

            imgBufferOffset += width - Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawTile_4bpp_FlipX(IList<Color> texColors, int xPos, int yPos, int width, byte[] tileSet, int tileSetIndex, Palette pal, int palOffset)
    {
        int imgBufferOffset = yPos * width + xPos + Constants.TileSize - 1;

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
                imgBufferOffset--;

                if (v2 != 0)
                    texColors[imgBufferOffset] = pal.Colors[palOffset + v2];
                imgBufferOffset--;

                tileSetIndex++;
            }

            imgBufferOffset += width + Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawTile_4bpp_FlipY(IList<Color> texColors, int xPos, int yPos, int width, byte[] tileSet, int tileSetIndex, Palette pal, int palOffset)
    {
        int imgBufferOffset = (yPos + Constants.TileSize - 1) * width + xPos;

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

            imgBufferOffset -= width + Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawTile_4bpp_FlipXY(IList<Color> texColors, int xPos, int yPos, int width, byte[] tileSet, int tileSetIndex, Palette pal, int palOffset)
    {
        int imgBufferOffset = (yPos + Constants.TileSize - 1) * width + xPos + Constants.TileSize - 1;

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
                imgBufferOffset--;

                if (v2 != 0)
                    texColors[imgBufferOffset] = pal.Colors[palOffset + v2];
                imgBufferOffset--;

                tileSetIndex++;
            }

            imgBufferOffset -= width - Constants.TileSize;
        }
    }

    #endregion

    #region Draw Tile 8bpp

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawTile_8bpp(IList<Color> texColors, int xPos, int yPos, int width, byte[] tileSet, int tileSetIndex, Palette pal)
    {
        int imgBufferOffset = yPos * width + xPos;

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

            imgBufferOffset += width - Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawTile_8bpp_FlipX(IList<Color> texColors, int xPos, int yPos, int width, byte[] tileSet, int tileSetIndex, Palette pal)
    {
        int imgBufferOffset = yPos * width + xPos + Constants.TileSize - 1;

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

            imgBufferOffset += width + Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawTile_8bpp_FlipY(IList<Color> texColors, int xPos, int yPos, int width, byte[] tileSet, int tileSetIndex, Palette pal)
    {
        int imgBufferOffset = (yPos + Constants.TileSize - 1) * width + xPos;

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

            imgBufferOffset -= width + Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawTile_8bpp_FlipXY(IList<Color> texColors, int xPos, int yPos, int width, byte[] tileSet, int tileSetIndex, Palette pal)
    {
        int imgBufferOffset = (yPos + Constants.TileSize - 1) * width + xPos + Constants.TileSize - 1;

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

            imgBufferOffset -= width - Constants.TileSize;
        }
    }

    #endregion
}