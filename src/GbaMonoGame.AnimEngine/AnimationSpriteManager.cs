using System.Collections.Generic;
using System.IO;
using BinarySerializer;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame.AnimEngine;

public class AnimationSpriteManager
{
    // TODO: Maybe it'd be worth having these be static?
    private Dictionary<AnimatedObjectResource, Palette[]> Palettes { get; } = new();
    private Dictionary<AnimatedObjectResource, Dictionary<int, Texture2D>> SpriteTextures { get; } = new();

    private Texture2D CreateSpriteTexture(byte spriteShape, byte spriteSize, bool is8Bit, Palette palette, byte[] tileSet, int tileIndex)
    {
        Constants.Size shape = Constants.GetSpriteShape(spriteShape, spriteSize);
        Texture2D tex = new(Engine.GraphicsDevice, shape.Width, shape.Height);
        Color[] texColors = new Color[tex.Width * tex.Height];
        int tileSetIndex = tileIndex * 0x20;

        int absTileY = 0;

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
                            texColors[absY * tex.Width + absX] = palette.Colors[colorIndex];
                        }

                        if (is8Bit || x % 2 == 1)
                            tileSetIndex++;
                    }
                }

                absTileX += Constants.TileSize;
            }

            absTileY += Constants.TileSize;
        }

        tex.SetData(texColors);
        return tex;
    }

    public Texture2D GetSpriteTexture(AnimatedObjectResource resource, byte spriteShape, byte spriteSize, int tileIndex, int paletteIndex)
    {
        if (!SpriteTextures.TryGetValue(resource, out Dictionary<int, Texture2D> sprites))
        {
            sprites = new Dictionary<int, Texture2D>();
            SpriteTextures.Add(resource, sprites);
        }

        int key = tileIndex * resource.PalettesCount + paletteIndex;

        if (!sprites.TryGetValue(key, out Texture2D tex))
        {
            if (!Palettes.TryGetValue(resource, out Palette[] palettes))
            {
                palettes = new Palette[resource.PalettesCount];

                for (int i = 0; i < resource.Palettes.Palettes.Length; i++)
                    palettes[i] = new Palette(resource.Palettes.Palettes[i]);

                Palettes.Add(resource, palettes);
            }

            tex = CreateSpriteTexture(spriteShape, spriteSize, resource.Is8Bit, palettes[paletteIndex], resource.SpriteTable.Data, tileIndex);

            if (Engine.Config.DumpSprites)
            {
                string outputDir = Path.Combine("Sprites", resource.Offset.StringAbsoluteOffset);
                Directory.CreateDirectory(outputDir);
                using Stream fileStream = File.Create(Path.Combine(outputDir, $"{tileIndex}_{paletteIndex}.png"));
                tex.SaveAsPng(fileStream, tex.Width, tex.Height);
            }

            sprites.Add(key, tex);
        }

        return tex;
    }
}