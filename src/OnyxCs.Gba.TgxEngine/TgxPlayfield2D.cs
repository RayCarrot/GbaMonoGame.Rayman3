using System.Collections.Generic;
using BinarySerializer.Onyx.Gba;
using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.TgxEngine;

public class TgxPlayfield2D : TgxPlayfield
{
    public TgxPlayfield2D(PlayfieldResource playfieldResource)
    {
        Screens = new GfxScreen[4];
        TileLayers = new List<TgxTileLayer>();

        Camera = new TgxCamera2D();

        // Add clusters to the camera
        foreach (ClusterResource clusterResource in playfieldResource.Clusters)
            Camera.AddCluster(clusterResource);

        // Load the layers
        foreach (GameLayerResource layerResource in playfieldResource.Layers)
        {
            if (layerResource.Type == GameLayerType.TileLayer)
            {
                TgxTileLayer layer = new(layerResource);
                TileLayers.Add(layer);
                Screens[layer.LayerId] = layer.Screen;

                layer.LoadTileKit(playfieldResource.TileKit, playfieldResource.TileMappingTable, playfieldResource.DefaultPalette);
                
                // The game does this in the layer constructor, but it's easier here since we have access to the camera
                Camera.AddLayer(layerResource.ClusterIndex, layer);
            }
            else if (layerResource.Type == GameLayerType.TileCollisionLayer)
            {
                CollisionLayer = new TgxTileCollisionLayer(layerResource);
            }
        }
    }

    public TgxCamera2D Camera { get; }
    public GfxScreen[] Screens { get; }
    public List<TgxTileLayer> TileLayers { get; }
}