using System;
using BinarySerializer.Ubisoft.GbaEngine;

namespace GbaMonoGame.TgxEngine;

public class GbaVram
{
    private GbaVram(byte[] tileSet, Palette palette)
    {
        TileSet = tileSet;
        Palette = palette;
    }

    private const int TileSize4bpp = 0x20;
    private const int TileSize8bpp = 0x40;

    public byte[] TileSet { get; }
    public Palette Palette { get; }

    public static GbaVram AllocateStatic(TileKit tileKit, TileMappingTable tileMappingTable, int vramLength8bpp, bool isAffine, int defaultPalette)
    {
        // If affine then the game starts at 513 instead of 512 (it always sets 512 to a blank tile)
        int base8bpp = isAffine ? 513 : 512;

        // 8-bit tiles start at its base and go on for the specified length before wrapping around
        // to 0. 4-bit tiles start at 0. Any data after this is dynamic.
        byte[] tileSet = new byte[(base8bpp + vramLength8bpp) * TileSize8bpp];

        // Allocate 4-bit tiles
        int offset = 2; // First 0x40 bytes are always empty. For 8-bit that's one tile, but for 4-bit it's 2 tiles.
        for (int i = 0; i < tileMappingTable.Table4bpp.Length; i++)
        {
            int value = tileMappingTable.Table4bpp[i] - 1;
            Array.Copy(tileKit.Tiles4bpp, value * TileSize4bpp, tileSet, (i + offset) * TileSize4bpp, TileSize4bpp);
        }

        // Allocate 8-bit tiles
        for (int i = 0; i < tileMappingTable.Table8bpp.Length; i++)
        {
            offset = i < vramLength8bpp ? base8bpp : -vramLength8bpp + 1;
            int value = tileMappingTable.Table8bpp[i] - 1;
            Array.Copy(tileKit.Tiles8bpp, value * TileSize8bpp, tileSet, (i + offset) * TileSize8bpp, TileSize8bpp);
        }

        // Load palette
        Palette pal = Engine.PaletteCache.GetOrCreateObject(
            pointer: tileKit.Offset,
            id: defaultPalette,
            data: tileKit.Palettes[defaultPalette].Palette,
            createObjFunc: p => new Palette(p));
        
        return new GbaVram(tileSet, pal);
    }
}