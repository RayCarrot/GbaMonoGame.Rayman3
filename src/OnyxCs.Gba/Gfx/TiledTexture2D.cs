using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba;

public class TiledTexture2D : Texture2D
{
    #region Constructor

    public TiledTexture2D(byte[] tileSet, int tileIndex, int paletteIndex, Palette palette, bool is8Bit) : 
        base(Engine.GraphicsDevice, Constants.TileSize, Constants.TileSize)
    {
        Color[] texColors = new Color[Width * Height];

        if (is8Bit)
        {
            TileHelpers.DrawTile_8bpp(texColors, 0, 0, Width, tileSet, tileIndex * 0x40, palette);
        }
        else
        {
            TileHelpers.DrawTile_4bpp(texColors, 0, 0, Width, tileSet, tileIndex * 0x20, palette, paletteIndex * 16);
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
                        TileHelpers.DrawTile_8bpp_FlipXY(texColors, absTileX, absTileY, Width, tileSet, tilePixelIndex, palette);
                    else if (tile.FlipX)
                        TileHelpers.DrawTile_8bpp_FlipX(texColors, absTileX, absTileY, Width, tileSet, tilePixelIndex, palette);
                    else if (tile.FlipY)
                        TileHelpers.DrawTile_8bpp_FlipY(texColors, absTileX, absTileY, Width, tileSet, tilePixelIndex, palette);
                    else
                        TileHelpers.DrawTile_8bpp(texColors, absTileX, absTileY, Width, tileSet, tilePixelIndex, palette);

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
                        TileHelpers.DrawTile_4bpp_FlipXY(texColors, absTileX, absTileY, Width, tileSet, tilePixelIndex, palette, palOffset);
                    else if (tile.FlipX)
                        TileHelpers.DrawTile_4bpp_FlipX(texColors, absTileX, absTileY, Width, tileSet, tilePixelIndex, palette, palOffset);
                    else if (tile.FlipY)
                        TileHelpers.DrawTile_4bpp_FlipY(texColors, absTileX, absTileY, Width, tileSet, tilePixelIndex, palette, palOffset);
                    else
                        TileHelpers.DrawTile_4bpp(texColors, absTileX, absTileY, Width, tileSet, tilePixelIndex, palette, palOffset);

                    absTileX += Constants.TileSize;
                }

                absTileY += Constants.TileSize;
            }
        }

        SetData(texColors);
    }
    
    #endregion
}