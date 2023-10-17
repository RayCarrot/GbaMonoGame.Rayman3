using System;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Onyx.Gba;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace OnyxCs.Gba.TgxEngine;

public class TgxTileLayer : TgxGameLayer
{
    public TgxTileLayer(GameLayerResource resource) : base(resource)
    {
        TileMap = resource.TileMap;
        LayerId = resource.LayerId;
        IsDynamic = resource.IsDynamic;

        Screen = Gfx.Screens[LayerId];
        Screen.IsEnabled = true;
        Screen.Offset = Vector2.Zero;
        Screen.Priority = 3 - LayerId;
        Screen.Wrap = true;
        Screen.Is8Bit = resource.Is8Bit;
        Screen.IsEnabled = true;

        if (resource.HasAlphaBlending)
        {
            // TODO: Set alpha blending
        }

        // TODO: Add layer to camera
    }

    public GfxScreen Screen { get; }
    public MapTile[] TileMap { get; }
    public byte LayerId { get; }
    public bool IsDynamic { get; }

    public void LoadTileKit(TileKit tileKit, TileMappingTable tileMappingTable, int defaultPalette)
    {
        if (tileKit.Idx_AnimatedTileKit != 0xFF)
        {
            // TODO: Load animated tiles
        }

        if (IsDynamic)
        {
            //if (Screen.Is8Bit)
            //{
            //    tileSet = new byte[0x40 + tileKit.Tiles8bpp.Length];
            //    Array.Copy(tileKit.Tiles8bpp, 0, tileSet, 0x40, tileKit.Tiles8bpp.Length);
            //}
            //else
            //{
            //    tileSet = new byte[0x20 + tileKit.Tiles4bpp.Length];
            //    Array.Copy(tileKit.Tiles4bpp, 0, tileSet, 0x20, tileKit.Tiles4bpp.Length);
            //}

            //TiledTexture2D tex = new(Width, Height, tileSet, TileMap, new Palette(tileKit.Palettes[defaultPalette].Palette), Screen.Is8Bit);
            //Screen.Renderer = new TextureScreenRenderer(tex);

            // TODO: Although this makes dealing with animations easier it does however cause lag if fullscreen.
            //       Maybe render to single texture like in commented out code above, and overlay animated tiles?
            byte[] tileSet = Screen.Is8Bit ? tileKit.Tiles8bpp : tileKit.Tiles4bpp;
            Screen.Renderer = new TileMapScreenRenderer(Width, Height, TileMap, tileSet, new Palette(tileKit.Palettes[defaultPalette].Palette), Screen.Is8Bit);
        }
        else
        {
            byte[] tileSet;

            if (Screen.Is8Bit)
            {
                tileSet = new byte[1024 * 0x40];
                for (int i = 0; i < tileMappingTable.Table8bpp.Length; i++)
                {
                    int value = tileMappingTable.Table8bpp[i] - 1;
                    Array.Copy(tileKit.Tiles8bpp, value * 0x40, tileSet, (i + 2) * 0x40, 0x40);
                }
            }
            else
            {
                tileSet = new byte[1024 * 0x20];
                for (int i = 0; i < tileMappingTable.Table4bpp.Length; i++)
                {
                    int value = tileMappingTable.Table4bpp[i] - 1;
                    Array.Copy(tileKit.Tiles4bpp, value * 0x20, tileSet, (i + 2) * 0x20, 0x20);
                }
            }

            TiledTexture2D tex = new(Width, Height, tileSet, TileMap, new Palette(tileKit.Palettes[defaultPalette].Palette), Screen.Is8Bit);
            Screen.Renderer = new TextureScreenRenderer(tex);
        }
    }
}