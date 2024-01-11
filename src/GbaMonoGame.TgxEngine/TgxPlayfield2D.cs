using System.Collections.Generic;
using BinarySerializer.Ubisoft.GbaEngine;

namespace GbaMonoGame.TgxEngine;

public class TgxPlayfield2D : TgxPlayfield
{
    public TgxPlayfield2D(Playfield2DResource playfieldResource) : base(new TgxCamera2D(Engine.GameWindow))
    {
        List<TgxTileLayer> tileLayers = new();

        // Add clusters to the camera
        foreach (ClusterResource clusterResource in playfieldResource.Clusters)
            Camera.AddCluster(clusterResource);

        // Load the layers
        foreach (GameLayerResource gameLayerResource in playfieldResource.Layers)
        {
            if (gameLayerResource.Type == GameLayerType.TileLayer)
            {
                TgxTileLayer layer = new(gameLayerResource);
                tileLayers.Add(layer);

                layer.LoadTileKit(playfieldResource.TileKit, playfieldResource.TileMappingTable, playfieldResource.DefaultPalette);
                
                // The game does this in the layer constructor, but it's easier here since we have access to the camera
                Camera.AddLayer(gameLayerResource.TileLayer.ClusterIndex, layer);

                // Set if the layer is scaled. The game doesn't do this as it has no concept of scaling.
                TgxCluster cluster = Camera.GetCluster(gameLayerResource.TileLayer.ClusterIndex);
                layer.Screen.Camera = cluster.Camera;
            }
            else if (gameLayerResource.Type == GameLayerType.PhysicalLayer)
            {
                PhysicalLayer = new TgxTilePhysicalLayer(gameLayerResource, Camera);

                // We want the debug collision map to scroll with the main cluster
                Camera.AddLayer(0, PhysicalLayer);
            }
        }

        TileLayers = tileLayers;
    }

    public new TgxCamera2D Camera => (TgxCamera2D)base.Camera;
    public IReadOnlyList<TgxTileLayer> TileLayers { get; }
}