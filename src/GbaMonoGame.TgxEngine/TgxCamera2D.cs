using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.TgxEngine;

public class TgxCamera2D : TgxCamera
{
    public TgxCamera2D(GameViewPort gameViewPort) : base(gameViewPort) { }

    private bool _fixedResolution;
    private Vector2? _maxResolution;

    private TgxCluster _mainCluster;
    private readonly List<TgxCluster> _clusters = [];

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

    /// <summary>
    /// Defines a custom maximum resolution. This is needed for the worldmap which has
    /// 8 pixels of blank space on the bottom.
    /// </summary>
    public Vector2? MaxResolution
    {
        get => _maxResolution;
        set
        {
            _maxResolution = value;
            UpdateResolution();
        }
    }

    public override Vector2 Position
    {
        get => _mainCluster.Position;
        set
        {
            TgxCluster mainCluster = GetMainCluster();
            mainCluster.Position = value;

            foreach (TgxCluster cluster in _clusters)
            {
                if (cluster.Stationary)
                    continue;

                //Vector2 scrollFactor = cluster.GetMaxPosition(Engine.GameViewPort.OriginalGameResolution) /
                //                       mainCluster.GetMaxPosition(Engine.GameViewPort.OriginalGameResolution);
                Vector2 scrollFactor;

                // If it's not scaled to the main playfield camera we have to update the scroll factor
                if (cluster.Camera != this && Engine.GameViewPort.GameResolution != Engine.GameViewPort.OriginalGameResolution)
                {
                    // Determine if the cluster wraps horizontally. We assume that none of them wrap vertically.
                    bool wrapX = cluster.Layers.Any(x => x.PixelWidth < cluster.Size.X);

                    if (wrapX)
                    {
                        // If the cluster wraps we use the original scroll factor (scaling it by the different camera resolutions)
                        scrollFactor = cluster.ScrollFactor * cluster.Camera.Resolution / Resolution;
                    }
                    else if (mainCluster.MaxPosition.X != 0)
                    {
                        // If the cluster does not wrap we want it to scroll evenly through the width of the level
                        scrollFactor = new Vector2(
                            cluster.GetMaxPosition(cluster.Camera.Resolution).X / mainCluster.MaxPosition.X,
                            cluster.ScrollFactor.Y);
                    }
                    else
                    {
                        scrollFactor = new Vector2(0, cluster.ScrollFactor.Y);
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

    protected override Vector2 GetResolution(GameViewPort gameViewPort)
    {
        // If fixed resolution we always use the original resolution
        if (FixedResolution)
        {
            return Engine.GameViewPort.OriginalGameResolution;
        }
        // If not fixed we attempt to confine it to the main cluster size
        else
        {
            Vector2 newGameResolution = gameViewPort.GameResolution * Engine.Config.PlayfieldCameraScale;

            Vector2? maxRes = MaxResolution ?? _mainCluster?.Size;
            if (maxRes is { } max)
            {
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
            return _mainCluster ?? throw new Exception("The main cluster hasn't been added yet");
        else
            return _clusters[clusterId - 1];
    }

    public IEnumerable<TgxCluster> GetClusters(bool includeMain)
    {
        if (includeMain)
            yield return GetMainCluster();

        foreach (TgxCluster cluster in _clusters)
            yield return cluster;
    }

    public void AddCluster(ClusterResource clusterResource)
    {
        if (_mainCluster == null)
        {
            _mainCluster = new TgxCluster(clusterResource, this);

            if (!FixedResolution)
                UpdateResolution();
        }
        else
        {
            _clusters.Add(new TgxCluster(clusterResource, this));
        }
    }

    public void AddLayer(int clusterId, TgxGameLayer layer)
    {
        TgxCluster cluster = GetCluster(clusterId);
        cluster.AddLayer(layer);
    }

    public override void UnInit()
    {
        base.UnInit();

        // Make sure to uninit the parallax cameras
        foreach (TgxCluster cluster in GetClusters(true))
        {
            if (cluster.Camera != this)
                cluster.Camera.UnInit();
        }
    }
}