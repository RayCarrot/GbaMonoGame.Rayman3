using System;
using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba;

public static class FontManager
{
    private const int CharsPerRow = 16;

    private static LoadedFont Font8 { get; set; }
    private static LoadedFont Font16 { get; set; }
    private static LoadedFont Font32 { get; set; }

    private static Texture2D CreateFontTexture(Font font, Color foreground, Color background)
    {
        int charWidth = font.CharacterHeight; // Use the height as the width in the texture
        int charHeight = font.CharacterHeight;

        // Use a single texture for the entire texture for better performance
        Texture2D tex = new(Engine.GraphicsDevice, charWidth * CharsPerRow, charHeight * (Font.CharactersCount / CharsPerRow));
        Color[] texColors = new Color[tex.Width * tex.Height];

        // Pad out end of array
        byte[] expandedImgData = new byte[font.ImgData.Length + 3];
        Array.Copy(font.ImgData, expandedImgData, font.ImgData.Length);

        if (background != Color.Transparent)
            Array.Fill(texColors, background);

        // Draw every character to the texture
        for (int charIndex = 0; charIndex < Font.CharactersCount; charIndex++)
        {
            int width = font.CharacterWidths[charIndex];

            if (width == 0)
                continue;

            // Get the origin point of the character on the texture
            int originX = (charIndex % CharsPerRow) * charWidth;
            int originY = (charIndex / CharsPerRow) * charHeight;

            int bitIndex = 0;
            uint value = 0;
            
            int imgDataOffset = font.CharacterOffsets[charIndex];
            int pixelOffset = originY * tex.Width + originX;

            // Draw every row
            for (int x = 0; x < width; x++)
            {
                // Get next value
                if (bitIndex == 0 || (bitIndex & 0x30) != 0)
                {
                    value = BitConverter.ToUInt32(expandedImgData, imgDataOffset);
                    bitIndex = 0;
                    imgDataOffset += font.CharacterHeight == 0x20 ? 4 : 2;
                }

                // Draw the column
                int currentPixelOffset = pixelOffset;
                for (int y = 0; y < font.CharacterHeight; y++)
                {
                    if (((value >> bitIndex) & 1) != 0)
                        texColors[currentPixelOffset] = foreground;

                    bitIndex += 1;
                    currentPixelOffset += tex.Width;
                }

                pixelOffset += 1;
            }
        }

        tex.SetData(texColors);

        return tex;
    }

    private static Rectangle[] GetFontCharacterRectangles(Font font)
    {
        int charWidth = font.CharacterHeight; // Use the height as the width in the texture
        int charHeight = font.CharacterHeight;

        Rectangle[] rects = new Rectangle[Font.CharactersCount];

        for (int charIndex = 0; charIndex < rects.Length; charIndex++)
        {
            // Get the origin point of the character on the texture
            int originX = (charIndex % CharsPerRow) * charWidth;
            int originY = (charIndex / CharsPerRow) * charHeight;

            rects[charIndex] = new Rectangle(originX, originY, font.CharacterWidths[charIndex], font.CharacterHeight);
        }

        return rects;
    }

    internal static void Load(Font font8, Font font16, Font font32)
    {
        Font8 = new LoadedFont(font8, CreateFontTexture(font8, Color.Black, Color.Transparent), GetFontCharacterRectangles(font8));
        Font16 = new LoadedFont(font16, CreateFontTexture(font16, Color.Black, Color.Transparent), GetFontCharacterRectangles(font16));
        Font32 = new LoadedFont(font32, CreateFontTexture(font32, Color.Black, Color.Transparent), GetFontCharacterRectangles(font32));
    }

    private record LoadedFont(Font Font, Texture2D Texture, Rectangle[] CharacterRectangles);
}