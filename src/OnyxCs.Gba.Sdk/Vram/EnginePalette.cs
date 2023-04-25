namespace OnyxCs.Gba.Sdk;

public class Palette
{
    public Palette(PaletteResource palette)
    {
        PaletteResource = palette;
        Colors = new Color[palette.Colors.Length];

        for (int i = 0; i < Colors.Length; i++)
            Colors[i] = new Color(palette.Colors[i].Red, palette.Colors[i].Green, palette.Colors[i].Blue);
    }

    public PaletteResource PaletteResource { get; }
    public Color[] Colors { get; }
    public bool IsDirty { get; set; }
}