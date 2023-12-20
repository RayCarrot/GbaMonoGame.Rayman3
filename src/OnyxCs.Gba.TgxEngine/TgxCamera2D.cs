using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.TgxEngine;

public class TgxCamera2D : TgxCamera
{
    public TgxCamera2D(GameWindow gameWindow) : base(gameWindow) { }

    private TgxCluster MainCluster { get; set; }
    private List<TgxCluster> Clusters { get; } = new();

    public override Vector2 Position
    {
        get => MainCluster.Position;
        set
        {
            TgxCluster mainCluster = GetMainCluster();
            mainCluster.Position = value;

            Vector2 originalMax = mainCluster.GetMaxPosition(Engine.ScreenCamera);
            Vector2 scaledMax = mainCluster.GetMaxPosition(this);

            foreach (TgxCluster cluster in Clusters)
            {
                if (cluster.Stationary)
                    continue;

                Vector2 scrollFactor;

                // If it's not scaled we have to update the scroll factor so it scrolls the same range
                if (cluster.Camera == Engine.ScreenCamera)
                {
                    scrollFactor = originalMax * cluster.ScrollFactor / scaledMax;

                    // Avoid issues with dividing by 0. Should never happen, but will if we scale out of bounds.
                    if (Single.IsInfinity(scrollFactor.X))
                        scrollFactor = new Vector2(0, scrollFactor.Y);
                    if (Single.IsInfinity(scrollFactor.Y))
                        scrollFactor = new Vector2(scrollFactor.X, 0);
                }
                else
                {
                    scrollFactor = cluster.ScrollFactor;
                }

                cluster.Position = mainCluster.Position * scrollFactor;
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
            MainCluster = new TgxCluster(clusterResource, this);
        else
            Clusters.Add(new TgxCluster(clusterResource, this));
    }

    public void AddLayer(int clusterId, TgxGameLayer layer)
    {
        TgxCluster cluster = GetCluster(clusterId);
        cluster.AddLayer(layer);
    }
}