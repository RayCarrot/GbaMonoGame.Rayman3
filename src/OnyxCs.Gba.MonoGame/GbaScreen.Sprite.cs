using System;
using BinarySerializer;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using OnyxCs.Gba.Sdk;

// TODO: Improve the sprite performance drawing like what was done for the backgrounds

// Handles drawing a sprite
public partial class GbaScreen
{
    private void DrawSprite(Sprite spr)
    {
        if (spr.Mode == OBJ_ATTR_ObjectMode.REG)
            DrawRegularSprite(spr);
        else if (spr.Mode is OBJ_ATTR_ObjectMode.AFF or OBJ_ATTR_ObjectMode.AFF_DBL)
            DrawAffineSprite(spr);
    }

    private void DrawRegularSprite(Sprite spr)
    {
        Constants.Size shape = Constants.GetSpriteShape(spr.SpriteShape, spr.SpriteSize);
        int tileSetIndex = spr.TileIndex * 0x20;
        Color[] pal = _spritePaletteManager.GetPalette(spr.Palette);

        int absTileY = spr.Position.Y + VisibleScreenRect.X;

        if (spr.FlipY)
            absTileY += shape.Height - Constants.TileSize;

        for (int tileY = 0; tileY < shape.TilesHeight; tileY++)
        {
            int absTileX = spr.Position.X + VisibleScreenRect.Y;

            if (spr.FlipX)
                absTileX += shape.Width - Constants.TileSize;

            for (int tileX = 0; tileX < shape.TilesWidth; tileX++)
            {
                for (int y = 0; y < Constants.TileSize; y++)
                {
                    for (int x = 0; x < Constants.TileSize; x++)
                    {
                        int absX = absTileX + (spr.FlipX ? Constants.TileSize - x - 1 : x);
                        int absY = absTileY + (spr.FlipY ? Constants.TileSize - y - 1 : y);

                        // Only draw if on screen
                        if (absX >= 0 && absX < VisibleScreenRect.Right &&
                            absY >= 0 && absY < VisibleScreenRect.Bottom)
                        {
                            int colorIndex = spr.TileSet[tileSetIndex];

                            if (!spr.Is8Bit)
                                colorIndex = BitHelpers.ExtractBits(colorIndex, 4, x % 2 == 0 ? 0 : 4);

                            // 0 is transparent, so ignore
                            if (colorIndex != 0)
                            {
                                // Set the pixel
                                _imgBuffer[absY * ImgBufferRect.Width + absX] = pal[colorIndex];
                            }
                        }

                        if (spr.Is8Bit || x % 2 == 1)
                            tileSetIndex++;
                    }
                }

                if (spr.FlipX)
                    absTileX -= Constants.TileSize;
                else
                    absTileX += Constants.TileSize;
            }

            if (spr.FlipY)
                absTileY -= Constants.TileSize;
            else
                absTileY += Constants.TileSize;
        }
    }

    private void DrawAffineSprite(Sprite spr)
    {
        if (!spr.Is8Bit)
            throw new NotImplementedException("Not implemented 4-bit affine sprite drawing");

        Constants.Size shape = Constants.GetSpriteShape(spr.SpriteShape, spr.SpriteSize);
        bool doubleSize = spr.Mode == OBJ_ATTR_ObjectMode.AFF_DBL;
        int tileSetIndex = spr.TileIndex * 0x20;
        Color[] pal = _spritePaletteManager.GetPalette(spr.Palette);

        int height = doubleSize ? shape.Height * 2 : shape.Height;
        int width = doubleSize ? shape.Width * 2 : shape.Width;

        for (int sprY = 0; sprY < height; sprY++)
        {
            int absY = sprY + spr.Position.Y + VisibleScreenRect.Y;

            int xofs, yofs;
            int xfofs, yfofs;

            if (doubleSize)
            {
                xofs = shape.Width;
                yofs = shape.Height;

                xfofs = -xofs / 2;
                yfofs = -yofs / 2;
            }
            else
            {
                xofs = shape.Width / 2;
                yofs = shape.Height / 2;

                xfofs = 0;
                yfofs = 0;
            }

            // Left edge
            int origXEdge0 = 0 - xofs;
            int origY = sprY - yofs;

            // Calculate starting parameters for matrix multiplications
            int shiftedXOfs = xofs + xfofs << 8;
            int shiftedYOfs = yofs + yfofs << 8;
            int pBYOffset = spr.AffineMatrix.Pb * origY + shiftedXOfs;
            int pDYOffset = spr.AffineMatrix.Pd * origY + shiftedYOfs;

            int objPixelXEdge0 = spr.AffineMatrix.Pa * origXEdge0 + pBYOffset;
            int objPixelYEdge0 = spr.AffineMatrix.Pc * origXEdge0 + pDYOffset;

            for (int sprX = 0; sprX < width; sprX++)
            {
                int absX = sprX + spr.Position.X + VisibleScreenRect.X;

                // Only draw if on screen
                if (absX >= 0 && absX < VisibleScreenRect.Right &&
                    absY >= 0 && absY < VisibleScreenRect.Bottom)
                {
                    int lerpedObjPixelX = objPixelXEdge0 >> 8;
                    int lerpedObjPixelY = objPixelYEdge0 >> 8;

                    if (lerpedObjPixelX >= 0 && lerpedObjPixelX < shape.Width &&
                        lerpedObjPixelY >= 0 && lerpedObjPixelY < shape.Height)
                    {
                        int intraTileX = (lerpedObjPixelX & 7);
                        int intraTileY = (lerpedObjPixelY & 7);

                        int tileX = lerpedObjPixelX / 8;
                        int tileY = lerpedObjPixelY / 8;

                        tileX += tileY * (shape.Width / 8);

                        byte colorIndex = spr.TileSet[tileSetIndex + tileX * 0x40 + intraTileY * 8 + intraTileX];

                        if (colorIndex != 0)
                            _imgBuffer[absY * ImgBufferRect.Width + absX] = pal[colorIndex];
                    }
                }

                objPixelXEdge0 += spr.AffineMatrix.Pa;
                objPixelYEdge0 += spr.AffineMatrix.Pc;
            }
        }
    }
}