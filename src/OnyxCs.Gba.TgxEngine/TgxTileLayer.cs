using System;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Onyx.Gba;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace OnyxCs.Gba.TgxEngine;

public class TgxTileLayer : TgxGameLayer
{
    public TgxTileLayer(GameLayerResource gameLayerResource) : base(gameLayerResource)
    {
        TileLayerResource tileLayerResource = gameLayerResource.TileLayer;

        TileMap = tileLayerResource.TileMap;
        LayerId = tileLayerResource.LayerId;
        Is8Bit = tileLayerResource.Is8Bit;
        IsDynamic = tileLayerResource.IsDynamic;

        Screen = new GfxScreen(LayerId)
        {
            IsEnabled = true,
            Offset = Vector2.Zero,
            Priority = 3 - LayerId,
            Wrap = true,
            Is8Bit = tileLayerResource.Is8Bit,
            IsAlphaBlendEnabled = tileLayerResource.HasAlphaBlending,
            GbaAlpha = tileLayerResource.AlphaCoeff,
        };

        Gfx.AddScreen(Screen);
    }

    public GfxScreen Screen { get; }
    public MapTile[] TileMap { get; }
    public byte LayerId { get; }
    public bool Is8Bit { get; }
    public bool IsDynamic { get; }

    public override void SetOffset(Vector2 offset)
    {
        Screen.Offset = offset;
    }

    public void LoadTileKit(TileKit tileKit, TileMappingTable tileMappingTable, int defaultPalette, int vramLength = 0x180)
    {
        if (tileKit.Idx_AnimatedTileKit != 0xFF)
        {
            // TODO: Load animated tiles
        }

        byte[] tileSet;

        // The game has two ways of allocating tilesets. If it's dynamic then it reserves space in vram for dynamically loading
        // the tile graphics as they're being displayed on screen (as the camera scrolls). If it's static then it's all pre-loaded.
        if (IsDynamic)
        {
            if (Is8Bit)
            {
                tileSet = new byte[0x40 + tileKit.Tiles8bpp.Length];
                Array.Copy(tileKit.Tiles8bpp, 0, tileSet, 0x40, tileKit.Tiles8bpp.Length);
            }
            else
            {
                tileSet = new byte[0x20 + tileKit.Tiles4bpp.Length];
                Array.Copy(tileKit.Tiles4bpp, 0, tileSet, 0x20, tileKit.Tiles4bpp.Length);
            }

            //byte[] tileSet = Screen.Is8Bit ? tileKit.Tiles8bpp : tileKit.Tiles4bpp;
            //Screen.Renderer = new TileMapScreenRenderer(Width, Height, TileMap, tileSet, new Palette(tileKit.Palettes[defaultPalette].Palette), Screen.Is8Bit);
        }
        else
        {
            // If the tile layer is static then we have to allocate the tileset in the same order the game does, or else
            // the tile indices won't match! The game has a rather complicated way of handling it.
            if (Is8Bit)
            {
                tileSet = new byte[1024 * 0x40];
                for (int i = 0; i < tileMappingTable.Table8bpp.Length; i++)
                {
                    int offset = i < vramLength ? 512 : -vramLength + 1;
                    int value = tileMappingTable.Table8bpp[i] - 1;
                    Array.Copy(tileKit.Tiles8bpp, value * 0x40, tileSet, (i + offset) * 0x40, 0x40);
                }
            }
            else
            {
                // First 0x40 bytes are always empty. For 8-bit that's one tile, but for 4-bit it's 2 tiles.
                const int offset = 2;

                tileSet = new byte[1024 * 0x20];
                for (int i = 0; i < tileMappingTable.Table4bpp.Length; i++)
                {
                    int value = tileMappingTable.Table4bpp[i] - 1;
                    Array.Copy(tileKit.Tiles4bpp, value * 0x20, tileSet, (i + offset) * 0x20, 0x20);
                }
            }
        }

        Palette pal = new(tileKit.Palettes[defaultPalette].Palette);
        TiledTexture2D tex = new(Width, Height, tileSet, TileMap, pal, Is8Bit);
        Screen.Renderer = new TextureScreenRenderer(tex, tileSet, TileMap, pal);
    }
}