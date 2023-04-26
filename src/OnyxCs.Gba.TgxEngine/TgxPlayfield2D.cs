﻿using System.Collections.Generic;
using BinarySerializer.Onyx.Gba;
using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.TgxEngine;

public class TgxPlayfield2D : TgxPlayfield
{
    public TgxPlayfield2D(PlayfieldResource playfieldResource)
    {
        Screens = new GfxScreen[4];
        TileLayers = new List<TgxTileLayer>();

        // Load the layers
        foreach (GameLayerResource layerResource in playfieldResource.Layers)
        {
            if (layerResource.Type == GameLayerType.TileLayer)
            {
                TgxTileLayer layer = new(layerResource);
                TileLayers.Add(layer);
                Screens[layer.LayerId] = layer.Screen;

                layer.LoadTileKit(playfieldResource.TileKit, playfieldResource.TileMappingTable, playfieldResource.DefaultPalette);
            }
            else if (layerResource.Type == GameLayerType.TileCollisionLayer)
            {
                CollisionLayer = new TgxTileCollisionLayer(layerResource);
            }
        }
    }

    public GfxScreen[] Screens { get; }
    public List<TgxTileLayer> TileLayers { get; }
}