using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.TgxEngine;

// TODO: Add debug mode to the game where we can freely move the camera with arrow keys and speed up by holding space
public class TgxCamera2D : TgxCamera
{
    private TgxCluster MainCluster { get; set; }
    private List<TgxCluster> Clusters { get; } = new();
    private Vector2 Position { get; set; }

    public TgxCluster GetMainCluster() => GetCluster(0);

    public TgxCluster GetCluster(int clusterId)
    {
        if (clusterId == 0)
            return MainCluster ?? throw new Exception("The main cluster hasn't been added yet");
        else
            return Clusters[clusterId - 1];
    }

    public void AddCluster(ClusterResource clusterResource)
    {
        if (MainCluster == null)
            MainCluster = new TgxCluster(clusterResource);
        else
            Clusters.Add(new TgxCluster(clusterResource));
    }

    public void AddLayer(int clusterId, TgxTileLayer layer)
    {
        TgxCluster cluster = GetCluster(clusterId);
        cluster.AddLayer(layer);
    }

    public override void Move(Vector2 deltaPos)
    {
        Position += deltaPos;
        Vector2 moved = GetMainCluster().Move(deltaPos);

        if (moved is { X: 0, Y: 0 }) 
            return;
        
        foreach (TgxCluster cluster in Clusters)
        {
            if (cluster.CanNotMove)   
                continue;

            cluster.Move(cluster.Scroll(moved));
        }
    }
}