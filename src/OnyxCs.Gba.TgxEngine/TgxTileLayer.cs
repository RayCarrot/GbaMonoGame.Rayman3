using System;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Onyx.Gba;
using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.TgxEngine;

public class TgxTileLayer : GameLayer
{
    public TgxTileLayer(GameLayerResource resource) : base(resource)
    {
        TileMap = resource.TileMap;
        LayerId = resource.LayerId;
        IsDynamic = resource.IsDynamic;

        Screen = new GfxScreen();
        Screen.SetBgDriver(LayerId);
        Screen.SetPriority(3 - LayerId);
        Screen.SetBgOverflowProcess(OverflowProcess.Wrap);
        Screen.SetColorMode(resource.Is8Bit);
        Screen.BackgroundDriver?.SetTileMap(TileMap, Width, Height);

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
        // Load the palette
        Vram.Instance.BackgroundPaletteManager?.Load(tileKit.Palettes[defaultPalette].Palette);

        byte[] tileSet;

        if (IsDynamic)
        {
            if (Screen.Is8Bit)
            {
                tileSet = new byte[0x40 + tileKit.Tiles8bpp.Length];
                Array.Copy(tileKit.Tiles8bpp, 0, tileSet, 0x40, tileKit.Tiles8bpp.Length);
            }
            else
            {
                tileSet = new byte[0x20 + tileKit.Tiles4bpp.Length];
                Array.Copy(tileKit.Tiles4bpp, 0, tileSet, 0x20, tileKit.Tiles4bpp.Length);
            }
        }
        else
        {
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
        }

        Screen.BackgroundDriver?.SetTileSet(tileSet, tileKit.Palettes[defaultPalette].Palette);
    }
}