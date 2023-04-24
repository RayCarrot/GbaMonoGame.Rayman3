using System.Runtime.CompilerServices;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using OnyxCs.Gba.Sdk;

// Handles drawing a background
public partial class MonoGameVram
{
    private void DrawBackground(Background bg)
    {
        if (!bg.IsEnabled || bg.Width == 0 || bg.Height == 0 || bg.Map == null || bg.TileSet == null || bg.Palette == null)
            return;

        if (bg.OverflowProcess == OverflowProcess.Wrap)
        {
            int screenWidth = bg.Width * Constants.TileSize;
            int screenHeight = bg.Height * Constants.TileSize;

            Vec2Int wrappedPos = new(-bg.Offset.X % screenWidth, -bg.Offset.Y % screenHeight);

            int startX = 0 - screenWidth + (wrappedPos.X == 0 ? screenWidth : wrappedPos.X);
            int startY = 0 - screenHeight + (wrappedPos.Y == 0 ? screenHeight : wrappedPos.Y);

            for (int y = startY; y < VisibleScreenRect.Height; y += screenHeight)
            {
                for (int x = startX; x < VisibleScreenRect.Width; x += screenWidth)
                {
                    Vec2Int pos = new(x, y);

                    if (bg.Is8Bit)
                        DrawRegularBackground_8bpp(bg, pos);
                    else
                        DrawRegularBackground_4bpp(bg, pos);
                }
            }
        }
        else
        {
            Vec2Int pos = new(-bg.Offset.X, -bg.Offset.Y);

            if (bg.Is8Bit)
                DrawRegularBackground_8bpp(bg, pos);
            else
                DrawRegularBackground_4bpp(bg, pos);
        }
    }

    #region Draw Regular

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawRegularBackground_4bpp(Background bg, Vec2Int pos)
    {
        Rectangle visibleTilesArea = GetVisibleTilesArea(pos, bg.Width, bg.Height);
        Color[] pal = BackgroundPaletteManager.GetPalette(bg.Palette!);

        int absTileY = pos.Y + VisibleScreenRect.Y + (visibleTilesArea.Y * Constants.TileSize);

        for (int tileY = visibleTilesArea.Top; tileY < visibleTilesArea.Bottom; tileY++)
        {
            int absTileX = pos.X + VisibleScreenRect.X + (visibleTilesArea.X * Constants.TileSize);

            for (int tileX = visibleTilesArea.Left; tileX < visibleTilesArea.Right; tileX++)
            {
                MapTile tile = bg.Map![tileY * bg.Width + tileX];

                int tilePixelIndex = tile.TileIndex * 0x20;
                int palOffset = tile.PaletteIndex * 16;

                if (tile.FlipX && tile.FlipY)
                    DrawTile_4bpp_FlipXY(absTileX, absTileY, bg.TileSet, tilePixelIndex, pal, palOffset);
                else if (tile.FlipX)
                    DrawTile_4bpp_FlipX(absTileX, absTileY, bg.TileSet, tilePixelIndex, pal, palOffset);
                else if (tile.FlipY)
                    DrawTile_4bpp_FlipY(absTileX, absTileY, bg.TileSet, tilePixelIndex, pal, palOffset);
                else
                    DrawTile_4bpp(absTileX, absTileY, bg.TileSet, tilePixelIndex, pal, palOffset);

                absTileX += Constants.TileSize;
            }

            absTileY += Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawRegularBackground_8bpp(Background bg, Vec2Int pos)
    {
        Rectangle visibleTilesArea = GetVisibleTilesArea(pos, bg.Width, bg.Height);
        Color[] pal = BackgroundPaletteManager.GetPalette(bg.Palette!);

        int absTileY = pos.Y + VisibleScreenRect.Y + (visibleTilesArea.Y * Constants.TileSize);

        for (int tileY = visibleTilesArea.Top; tileY < visibleTilesArea.Bottom; tileY++)
        {
            int absTileX = pos.X + VisibleScreenRect.X + (visibleTilesArea.X * Constants.TileSize);

            for (int tileX = visibleTilesArea.Left; tileX < visibleTilesArea.Right; tileX++)
            {
                MapTile tile = bg.Map![tileY * bg.Width + tileX];

                int tilePixelIndex = tile.TileIndex * 0x40;

                if (tile.FlipX && tile.FlipY)
                    DrawTile_8bpp_FlipXY(absTileX, absTileY, bg.TileSet, tilePixelIndex, pal);
                else if (tile.FlipX)
                    DrawTile_8bpp_FlipX(absTileX, absTileY, bg.TileSet, tilePixelIndex, pal);
                else if (tile.FlipY)
                    DrawTile_8bpp_FlipY(absTileX, absTileY, bg.TileSet, tilePixelIndex, pal);
                else
                    DrawTile_8bpp(absTileX, absTileY, bg.TileSet, tilePixelIndex, pal);

                absTileX += Constants.TileSize;
            }

            absTileY += Constants.TileSize;
        }
    }

    #endregion

    #region Draw Tile 4bpp

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawTile_4bpp(int xPos, int yPos, byte[] tileSet, int tileSetIndex, Color[] pal, int palOffset)
    {
        int imgBufferOffset = yPos * ImgBufferRect.Width + xPos;

        for (int y = 0; y < Constants.TileSize; y++)
        {
            for (int x = 0; x < Constants.TileSize; x += 2)
            {
                int value = tileSet[tileSetIndex];

                int v1 = value & 0xF;
                int v2 = value >> 4;

                // Set the pixel if not 0 (transparent)
                if (v1 != 0)
                    _imgBuffer[imgBufferOffset] = pal[palOffset + v1];
                imgBufferOffset++;

                if (v2 != 0)
                    _imgBuffer[imgBufferOffset] = pal[palOffset + v2];
                imgBufferOffset++;

                tileSetIndex++;
            }

            imgBufferOffset += ImgBufferRect.Width - Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawTile_4bpp_FlipX(int xPos, int yPos, byte[] tileSet, int tileSetIndex, Color[] pal, int palOffset)
    {
        int imgBufferOffset = yPos * ImgBufferRect.Width + xPos + Constants.TileSize - 1;

        for (int y = 0; y < Constants.TileSize; y++)
        {
            for (int x = 0; x < Constants.TileSize; x += 2)
            {
                int value = tileSet[tileSetIndex];

                int v1 = value >> 4;
                int v2 = value & 0xF;

                // Set the pixel if not 0 (transparent)
                if (v1 != 0)
                    _imgBuffer[imgBufferOffset] = pal[palOffset + v1];
                imgBufferOffset--;

                if (v2 != 0)
                    _imgBuffer[imgBufferOffset] = pal[palOffset + v2];
                imgBufferOffset--;

                tileSetIndex++;
            }

            imgBufferOffset += ImgBufferRect.Width + Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawTile_4bpp_FlipY(int xPos, int yPos, byte[] tileSet, int tileSetIndex, Color[] pal, int palOffset)
    {
        int imgBufferOffset = (yPos + Constants.TileSize - 1) * ImgBufferRect.Width + xPos;

        for (int y = 0; y < Constants.TileSize; y++)
        {
            for (int x = 0; x < Constants.TileSize; x += 2)
            {
                int value = tileSet[tileSetIndex];

                int v1 = value & 0xF;
                int v2 = value >> 4;

                // Set the pixel if not 0 (transparent)
                if (v1 != 0)
                    _imgBuffer[imgBufferOffset] = pal[palOffset + v1];
                imgBufferOffset++;

                if (v2 != 0)
                    _imgBuffer[imgBufferOffset] = pal[palOffset + v2];
                imgBufferOffset++;

                tileSetIndex++;
            }

            imgBufferOffset -= ImgBufferRect.Width + Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawTile_4bpp_FlipXY(int xPos, int yPos, byte[] tileSet, int tileSetIndex, Color[] pal, int palOffset)
    {
        int imgBufferOffset = (yPos + Constants.TileSize - 1) * ImgBufferRect.Width + xPos + Constants.TileSize - 1;

        for (int y = 0; y < Constants.TileSize; y++)
        {
            for (int x = 0; x < Constants.TileSize; x += 2)
            {
                int value = tileSet[tileSetIndex];

                int v1 = value >> 4;
                int v2 = value & 0xF;

                // Set the pixel if not 0 (transparent)
                if (v1 != 0)
                    _imgBuffer[imgBufferOffset] = pal[palOffset + v1];
                imgBufferOffset--;

                if (v2 != 0)
                    _imgBuffer[imgBufferOffset] = pal[palOffset + v2];
                imgBufferOffset--;

                tileSetIndex++;
            }

            imgBufferOffset -= ImgBufferRect.Width - Constants.TileSize;
        }
    }

    #endregion

    #region Draw Tile 8bpp

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawTile_8bpp(int xPos, int yPos, byte[] tileSet, int tileSetIndex, Color[] pal)
    {
        int imgBufferOffset = yPos * ImgBufferRect.Width + xPos;

        for (int y = 0; y < Constants.TileSize; y++)
        {
            for (int x = 0; x < Constants.TileSize; x++)
            {
                int value = tileSet[tileSetIndex];

                // Set the pixel if not 0 (transparent)
                if (value != 0)
                    _imgBuffer[imgBufferOffset] = pal[value];

                imgBufferOffset++;
                tileSetIndex++;
            }

            imgBufferOffset += ImgBufferRect.Width - Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawTile_8bpp_FlipX(int xPos, int yPos, byte[] tileSet, int tileSetIndex, Color[] pal)
    {
        int imgBufferOffset = yPos * ImgBufferRect.Width + xPos + Constants.TileSize - 1;

        for (int y = 0; y < Constants.TileSize; y++)
        {
            for (int x = 0; x < Constants.TileSize; x++)
            {
                int value = tileSet[tileSetIndex];

                // Set the pixel if not 0 (transparent)
                if (value != 0)
                    _imgBuffer[imgBufferOffset] = pal[value];

                imgBufferOffset--;
                tileSetIndex++;
            }

            imgBufferOffset += ImgBufferRect.Width + Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawTile_8bpp_FlipY(int xPos, int yPos, byte[] tileSet, int tileSetIndex, Color[] pal)
    {
        int imgBufferOffset = (yPos + Constants.TileSize - 1) * ImgBufferRect.Width + xPos;

        for (int y = 0; y < Constants.TileSize; y++)
        {
            for (int x = 0; x < Constants.TileSize; x++)
            {
                int value = tileSet[tileSetIndex];

                // Set the pixel if not 0 (transparent)
                if (value != 0)
                    _imgBuffer[imgBufferOffset] = pal[value];

                imgBufferOffset++;
                tileSetIndex++;
            }

            imgBufferOffset -= ImgBufferRect.Width + Constants.TileSize;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawTile_8bpp_FlipXY(int xPos, int yPos, byte[] tileSet, int tileSetIndex, Color[] pal)
    {
        int imgBufferOffset = (yPos + Constants.TileSize - 1) * ImgBufferRect.Width + xPos + Constants.TileSize - 1;

        for (int y = 0; y < Constants.TileSize; y++)
        {
            for (int x = 0; x < Constants.TileSize; x++)
            {
                int value = tileSet[tileSetIndex];

                // Set the pixel if not 0 (transparent)
                if (value != 0)
                    _imgBuffer[imgBufferOffset] = pal[value];

                imgBufferOffset--;
                tileSetIndex++;
            }

            imgBufferOffset -= ImgBufferRect.Width - Constants.TileSize;
        }
    }

    #endregion
}