﻿using System.Collections.Generic;
using BinarySerializer.Ubisoft.GbaEngine;

namespace GbaMonoGame.TgxEngine;

public class TgxPlayfieldMode7 : TgxPlayfield
{
    public TgxPlayfieldMode7(PlayfieldMode7Resource playfieldResource)
        : base(new TgxCameraMode7(Engine.GameViewPort), playfieldResource.TileKit)
    {
        List<TgxRotscaleLayerMode7> rotScaleLayers = new();

        // Load vram
        Vram = GbaVram.AllocateStatic(playfieldResource.TileKit, playfieldResource.TileMappingTable, 0x100, true, playfieldResource.DefaultPalette);

        // Load the layers
        foreach (GameLayerResource gameLayerResource in playfieldResource.Layers)
        {
            if (gameLayerResource.Type == GameLayerType.RotscaleLayerMode7)
            {
                TgxRotscaleLayerMode7 layer = new(gameLayerResource);
                rotScaleLayers.Add(layer);

                layer.LoadRenderer(playfieldResource.TileKit, Vram);

                layer.Screen.Camera = Camera;

                // Add the renderer to the animated tile kit manager
                if (layer.Screen.Renderer is TileMapScreenRenderer renderer)
                    AnimatedTilekitManager?.AddRenderer(renderer);
            }
            else if (gameLayerResource.Type == GameLayerType.TextLayerMode7)
            {
                // TODO: Implement
            }
            else if (gameLayerResource.Type == GameLayerType.PhysicalLayer)
            {
                PhysicalLayer = new TgxTilePhysicalLayer(gameLayerResource, Camera);

                // TODO: Somehow add layer to camera so we can display it in debug mode
            }
        }

        RotScaleLayers = rotScaleLayers;
    }

    public IReadOnlyList<TgxRotscaleLayerMode7> RotScaleLayers { get; }
    public GbaVram Vram { get; }
}