using System.Runtime.CompilerServices;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;

namespace GbaMonoGame;

public static class DrawHelpers
{
    #region Draw Tile 4bpp

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawTile_4bpp(Color[] texColors, int xPos, int yPos, int width, byte[] tileSet, ref int tileSetIndex, Palette pal, int palOffset)
    {
        int imgBufferOffset = yPos * width + xPos;

        for (int y = 0; y < Tile.Size; y++)
        {
            for (int x = 0; x < Tile.Size; x += 2)
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

            imgBufferOffset += width - Tile.Size;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawTile_4bpp_FlipX(Color[] texColors, int xPos, int yPos, int width, byte[] tileSet, ref int tileSetIndex, Palette pal, int palOffset)
    {
        int imgBufferOffset = yPos * width + xPos + Tile.Size - 1;

        for (int y = 0; y < Tile.Size; y++)
        {
            for (int x = 0; x < Tile.Size; x += 2)
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

            imgBufferOffset += width + Tile.Size;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawTile_4bpp_FlipY(Color[] texColors, int xPos, int yPos, int width, byte[] tileSet, ref int tileSetIndex, Palette pal, int palOffset)
    {
        int imgBufferOffset = (yPos + Tile.Size - 1) * width + xPos;

        for (int y = 0; y < Tile.Size; y++)
        {
            for (int x = 0; x < Tile.Size; x += 2)
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

            imgBufferOffset -= width + Tile.Size;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawTile_4bpp_FlipXY(Color[] texColors, int xPos, int yPos, int width, byte[] tileSet, ref int tileSetIndex, Palette pal, int palOffset)
    {
        int imgBufferOffset = (yPos + Tile.Size - 1) * width + xPos + Tile.Size - 1;

        for (int y = 0; y < Tile.Size; y++)
        {
            for (int x = 0; x < Tile.Size; x += 2)
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

            imgBufferOffset -= width - Tile.Size;
        }
    }

    #endregion

    #region Draw Tile 8bpp

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawTile_8bpp(Color[] texColors, int xPos, int yPos, int width, byte[] tileSet, ref int tileSetIndex, Palette pal)
    {
        int imgBufferOffset = yPos * width + xPos;

        for (int y = 0; y < Tile.Size; y++)
        {
            for (int x = 0; x < Tile.Size; x++)
            {
                int value = tileSet[tileSetIndex];

                // Set the pixel if not 0 (transparent)
                if (value != 0)
                    texColors[imgBufferOffset] = pal.Colors[value];

                imgBufferOffset++;
                tileSetIndex++;
            }

            imgBufferOffset += width - Tile.Size;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawTile_8bpp_FlipX(Color[] texColors, int xPos, int yPos, int width, byte[] tileSet, ref int tileSetIndex, Palette pal)
    {
        int imgBufferOffset = yPos * width + xPos + Tile.Size - 1;

        for (int y = 0; y < Tile.Size; y++)
        {
            for (int x = 0; x < Tile.Size; x++)
            {
                int value = tileSet[tileSetIndex];

                // Set the pixel if not 0 (transparent)
                if (value != 0)
                    texColors[imgBufferOffset] = pal.Colors[value];

                imgBufferOffset--;
                tileSetIndex++;
            }

            imgBufferOffset += width + Tile.Size;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawTile_8bpp_FlipY(Color[] texColors, int xPos, int yPos, int width, byte[] tileSet, ref int tileSetIndex, Palette pal)
    {
        int imgBufferOffset = (yPos + Tile.Size - 1) * width + xPos;

        for (int y = 0; y < Tile.Size; y++)
        {
            for (int x = 0; x < Tile.Size; x++)
            {
                int value = tileSet[tileSetIndex];

                // Set the pixel if not 0 (transparent)
                if (value != 0)
                    texColors[imgBufferOffset] = pal.Colors[value];

                imgBufferOffset++;
                tileSetIndex++;
            }

            imgBufferOffset -= width + Tile.Size;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawTile_8bpp_FlipXY(Color[] texColors, int xPos, int yPos, int width, byte[] tileSet, ref int tileSetIndex, Palette pal)
    {
        int imgBufferOffset = (yPos + Tile.Size - 1) * width + xPos + Tile.Size - 1;

        for (int y = 0; y < Tile.Size; y++)
        {
            for (int x = 0; x < Tile.Size; x++)
            {
                int value = tileSet[tileSetIndex];

                // Set the pixel if not 0 (transparent)
                if (value != 0)
                    texColors[imgBufferOffset] = pal.Colors[value];

                imgBufferOffset--;
                tileSetIndex++;
            }

            imgBufferOffset -= width - Tile.Size;
        }
    }

    #endregion
}