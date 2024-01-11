using BinarySerializer;
using Microsoft.Xna.Framework;

namespace GbaMonoGame;

public class Palette
{
    public Palette(RGB555Color[] colors)
    {
        Colors = new Color[colors.Length];

        for (int i = 0; i < Colors.Length; i++)
            Colors[i] = new Color(colors[i].Red, colors[i].Green, colors[i].Blue);
    }

    public Palette(PaletteResource palette) : this(palette.Colors) { }

    public Palette(Color[] colors)
    {
        Colors = colors;
    }

    public Color[] Colors { get; }
}