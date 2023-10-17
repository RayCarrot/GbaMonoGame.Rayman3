using System.Collections.Generic;
using BinarySerializer.Onyx.Gba;

namespace OnyxCs.Gba.TgxEngine;

public class TgxPlayfield2D : TgxPlayfield
{
    public TgxPlayfield2D(PlayfieldResource playfieldResource)
    {
        List<TgxTileLayer> tileLayers = new();

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
                tileLayers.Add(layer);
                //Screens[layer.LayerId] = layer.Screen;

                layer.LoadTileKit(playfieldResource.TileKit, playfieldResource.TileMappingTable, playfieldResource.DefaultPalette);
                
                // The game does this in the layer constructor, but it's easier here since we have access to the camera
                Camera.AddLayer(layerResource.ClusterIndex, layer);
            }
            else if (layerResource.Type == GameLayerType.TileCollisionLayer)
            {
                CollisionLayer = new TgxTileCollisionLayer(layerResource);
            }
        }

        TileLayers = tileLayers;
    }

    public TgxCamera2D Camera { get; }
    public IReadOnlyList<TgxTileLayer> TileLayers { get; }
}