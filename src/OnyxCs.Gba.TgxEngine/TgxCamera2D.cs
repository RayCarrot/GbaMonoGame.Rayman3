using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.TgxEngine;

public class TgxCamera2D : TgxCamera
{
    private TgxCluster MainCluster { get; set; }
    private List<TgxCluster> Clusters { get; } = new();

    public override Vector2 Position
    {
        get => MainCluster.Position;
        set
        {
            TgxCluster mainCluster = GetMainCluster();
            mainCluster.Position = value;

            foreach (TgxCluster cluster in Clusters)
            {
                if (cluster.Stationary)
                    continue;

                cluster.Position = value * cluster.ScrollFactor;
            }
        }
    }

    public TgxCluster GetMainCluster() => GetCluster(0);

    public TgxCluster GetCluster(int clusterId)
    {
        if (clusterId == 0)
            return MainCluster ?? throw new Exception("The main cluster hasn't been added yet");
        else
            return Clusters[clusterId - 1];
    }

    public IEnumerable<TgxCluster> GetClusters(bool includeMain)
    {
        if (includeMain)
            yield return GetMainCluster();

        foreach (TgxCluster cluster in Clusters)
            yield return cluster;
    }

    public void AddCluster(ClusterResource clusterResource)
    {
        if (MainCluster == null)
            MainCluster = new TgxCluster(clusterResource);
        else
            Clusters.Add(new TgxCluster(clusterResource));
    }

    public void AddLayer(int clusterId, TgxGameLayer layer)
    {
        TgxCluster cluster = GetCluster(clusterId);
        cluster.AddLayer(layer);
    }
}