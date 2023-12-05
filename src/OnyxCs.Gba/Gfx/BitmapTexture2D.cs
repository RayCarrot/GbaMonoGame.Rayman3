﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba;

public class BitmapTexture2D : Texture2D
{
    public BitmapTexture2D(int width, int height, byte[] bitmap, Palette palette) : base(Engine.GraphicsDevice, width, height)
    {
        Color[] texColors = new Color[width * height];

        int colorIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texColors[colorIndex] = palette.Colors[bitmap[colorIndex]];
                colorIndex++;
            }
        }

        SetData(texColors);
    }
}