using Microsoft.Xna.Framework;

namespace OnyxCs.Gba;

public class Palette
{
    public Palette(PaletteResource palette)
    {
        Colors = new Color[palette.Colors.Length];

        for (int i = 0; i < Colors.Length; i++)
            Colors[i] = new Color(palette.Colors[i].Red, palette.Colors[i].Green, palette.Colors[i].Blue);
    }

    public Palette(Color[] colors)
    {
        Colors = colors;
    }

    public Color[] Colors { get; }
}