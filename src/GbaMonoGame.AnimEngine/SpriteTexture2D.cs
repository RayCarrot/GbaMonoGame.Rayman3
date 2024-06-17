using BinarySerializer;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame;

public class SpriteTexture2D : Texture2D
{
    public SpriteTexture2D(AnimatedObjectResource resource, int spriteShape, int spriteSize, Palette palette, int tileIndex) :
        this(resource, Constants.GetSpriteShape(spriteShape, spriteSize), palette, tileIndex) { }

    public SpriteTexture2D(AnimatedObjectResource resource, Constants.Size shape, Palette palette, int tileIndex) :
        base(Engine.GraphicsDevice, shape.Width, shape.Height)
    {
        Color[] texColors = new Color[Width * Height];
        byte[] tileSet = resource.SpriteTable.Data;
        int tileSetIndex = tileIndex * 0x20;
        bool is8Bit = resource.Is8Bit;

        int absTileY = 0;

        // TODO: Optimize like how we did with TiledTexture2D - but run stopwatch to make sure it's actually faster
        for (int tileY = 0; tileY < shape.TilesHeight; tileY++)
        {
            int absTileX = 0;

            for (int tileX = 0; tileX < shape.TilesWidth; tileX++)
            {
                for (int y = 0; y < Constants.TileSize; y++)
                {
                    for (int x = 0; x < Constants.TileSize; x++)
                    {
                        int absX = absTileX + x;
                        int absY = absTileY + y;

                        int colorIndex = tileSet[tileSetIndex];

                        if (!is8Bit)
                            colorIndex = BitHelpers.ExtractBits(colorIndex, 4, x % 2 == 0 ? 0 : 4);

                        // 0 is transparent, so ignore
                        if (colorIndex != 0)
                        {
                            // Set the pixel
                            texColors[absY * Width + absX] = palette.Colors[colorIndex];
                        }

                        if (is8Bit || x % 2 == 1)
                            tileSetIndex++;
                    }
                }

                absTileX += Constants.TileSize;
            }

            absTileY += Constants.TileSize;
        }

        SetData(texColors);
    }
}