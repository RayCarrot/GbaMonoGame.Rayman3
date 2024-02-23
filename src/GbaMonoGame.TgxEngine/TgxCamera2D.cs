using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.TgxEngine;

public class TgxCamera2D : TgxCamera
{
    public TgxCamera2D(GameWindow gameWindow) : base(gameWindow) { }

    private bool _fixedResolution;

    private TgxCluster MainCluster { get; set; }
    private List<TgxCluster> Clusters { get; } = new();

    /// <summary>
    /// Indicates if the resolution is fixed to the original screen resolution. This is
    /// needed for certain playfields, such as the menu and intro.
    /// </summary>
    public bool FixedResolution
    {
        get => _fixedResolution;
        set
        {
            _fixedResolution = value;
            UpdateResolution();
        }
    }

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

                //Vector2 scrollFactor = cluster.GetMaxPosition(Engine.GameWindow.OriginalGameResolution) /
                //                       mainCluster.GetMaxPosition(Engine.GameWindow.OriginalGameResolution);
                Vector2 scrollFactor;

                // If it's not scaled to the main playfield camera we have to update the scroll factor
                if (cluster.Camera != this && Engine.ScreenCamera.Resolution != Engine.GameWindow.OriginalGameResolution)
                {
                    // Determine if the cluster wraps horizontally. We assume that none of them wrap vertically.
                    bool wrapX = cluster.GetLayers().Any(x => x.PixelWidth < cluster.Size.X);

                    if (wrapX)
                    {
                        // If the cluster wraps we use the original scroll factor (scaling it by the different camera resolutions)
                        scrollFactor = cluster.ScrollFactor * cluster.Camera.Resolution / Resolution;
                    }
                    else
                    {
                        // If the cluster does not wrap we want it to scroll evenly through the width of the level
                        scrollFactor = new Vector2(
                            cluster.GetMaxPosition(cluster.Camera.Resolution).X / mainCluster.MaxPosition.X,
                            cluster.ScrollFactor.Y);
                    }
                }
                else
                {
                    scrollFactor = cluster.ScrollFactor;
                }

                cluster.Position = mainCluster.Position * scrollFactor;
            }
        }
    }

    protected override Vector2 GetResolution(GameWindow gameWindow)
    {
        // If fixed resolution we always use the original resolution
        if (FixedResolution)
        {
            return Engine.GameWindow.OriginalGameResolution;
        }
        // If not fixed we attempt to confine it to the main cluster size
        else
        {
            Vector2 newGameResolution = gameWindow.GameResolution * Engine.Config.PlayfieldCameraScale;

            if (MainCluster != null)
            {
                Vector2 max = MainCluster.Size;

                if (newGameResolution.X > newGameResolution.Y)
                {
                    if (newGameResolution.Y > max.Y)
                        newGameResolution = new Vector2(max.Y * newGameResolution.X / newGameResolution.Y, max.Y);

                    if (newGameResolution.X > max.X)
                        newGameResolution = new Vector2(max.X, max.X * newGameResolution.Y / newGameResolution.X);
                }
                else
                {
                    if (newGameResolution.X > max.X)
                        newGameResolution = new Vector2(max.X, max.X * newGameResolution.Y / newGameResolution.X);

                    if (newGameResolution.Y > max.Y)
                        newGameResolution = new Vector2(max.Y * newGameResolution.X / newGameResolution.Y, max.Y);
                }
            }

            return newGameResolution;
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
        {
            MainCluster = new TgxCluster(clusterResource, this);

            if (!FixedResolution)
                UpdateResolution();
        }
        else
        {
            Clusters.Add(new TgxCluster(clusterResource, this));
        }
    }

    public void AddLayer(int clusterId, TgxGameLayer layer)
    {
        TgxCluster cluster = GetCluster(clusterId);
        cluster.AddLayer(layer);
    }
}