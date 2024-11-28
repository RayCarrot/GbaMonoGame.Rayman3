using System.Numerics;
using BinarySerializer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame;

public class PaletteTexture2D : Texture2D
{
    public PaletteTexture2D(RGB555Color[] palette, bool is8Bit) :
        base(Engine.GraphicsDevice, GetWidth(is8Bit), 1)
    {
        Color[] texColors = new Color[Width * Height];
        
        // Set the palette colors, skipping the first color since it's the transparent color
        for (int colorIndex = 1; colorIndex < palette.Length; colorIndex++)
            texColors[colorIndex] = palette[colorIndex].ToColor();

        SetData(texColors);
    }

    public PaletteTexture2D(PaletteResource[] palettes, bool is8Bit) :
        base(Engine.GraphicsDevice, GetWidth(is8Bit), GetHeight(palettes.Length))
    {
        Color[] texColors = new Color[Width * Height];
        
        // Enumerate each palette
        int texColorIndex = 0;
        foreach (PaletteResource pal in palettes)
        {
            // Skip the first color in each palette since it's the transparent color
            texColorIndex++;

            // Set the palette colors
            for (int colorIndex = 1; colorIndex < pal.Colors.Length; colorIndex++)
            {
                texColors[texColorIndex] = pal.Colors[colorIndex].ToColor();
                texColorIndex++;
            }
        }

        SetData(texColors);
    }

    // The width is the full size of the palette, so we fit one palette per row
    private static int GetWidth(bool is8Bit) => is8Bit ? 256 : 16;

    // The height is the number of palettes, rounded up to power of 2 to avoid issues in the shader
    private static int GetHeight(int palettesCount) => (int)BitOperations.RoundUpToPowerOf2((uint)palettesCount);
}