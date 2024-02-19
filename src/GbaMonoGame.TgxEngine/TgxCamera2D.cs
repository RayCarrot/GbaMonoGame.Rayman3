using System;
using System.Collections.Generic;
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

                    // Avoid issues with dividing by 0 if max is 0
                    if (!Single.IsFinite(scrollFactor.X))
                        scrollFactor = new Vector2(0, scrollFactor.Y);
                    if (!Single.IsFinite(scrollFactor.Y))
                        scrollFactor = new Vector2(scrollFactor.X, 0);

                    // TODO: Have some setting for limiting bg scrolling when scaled like so. Do we need to multiply by some scaling value though or is this value good?
                    //if (scrollFactor.Y > 0.7)
                    //    scrollFactor = new Vector2(scrollFactor.X, 0.7f);
                    //if (scrollFactor.X > 0.7) // TODO: This breaks boss machine, but needed for Menhir Hills
                    //    scrollFactor = new Vector2(0.7f, scrollFactor.Y);
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