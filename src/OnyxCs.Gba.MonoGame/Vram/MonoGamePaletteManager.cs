using System.Collections.Generic;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.MonoGame;

public class MonoGamePaletteManager : PaletteManager
{
    private readonly Dictionary<Palette, Color[]> Palettes = new();

    public override void Load(Palette palette)
    {
        if (Palettes.ContainsKey(palette))
            return;

        Color[] colors = new Color[palette.Colors.Length];

        for (int i = 0; i < colors.Length; i++)
            colors[i] = new Color(palette.Colors[i].Red, palette.Colors[i].Green, palette.Colors[i].Blue);

        Palettes.Add(palette, colors);
    }

    public override void UnloadAll()
    {
        Palettes.Clear();
    }

    public Color[] GetPalette(Palette palette) => Palettes[palette];
}