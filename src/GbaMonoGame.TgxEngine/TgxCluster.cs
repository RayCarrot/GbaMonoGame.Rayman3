using System;
using System.Collections.Generic;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.TgxEngine;

public class TgxCluster
{
    public TgxCluster(ClusterResource cluster, TgxCamera2D tgxCamera)
    {
        ScrollFactor = new Vector2(cluster.ScrollFactor.X, cluster.ScrollFactor.Y);
        Layers = new List<TgxGameLayer>();
        Size = new Vector2(cluster.SizeX * Constants.TileSize, cluster.SizeY * Constants.TileSize);
        Stationary = cluster.Stationary;

        // Render backgrounds to the screen camera
        Camera = !Stationary && ScrollFactor == Vector2.One ? tgxCamera : Engine.ScreenCamera;
    }

    private Vector2 _position;

    private List<TgxGameLayer> Layers { get; }

    public Vector2 Size { get; }

    public Vector2 Position
    {
        get => _position;
        set
        {
            Vector2 maxPos = MaxPosition;

            if (value.X < 0)
                value.X = 0;
            if (value.Y < 0)
                value.Y = 0;
            if (value.X > maxPos.X)
                value.X = maxPos.X;
            if (value.Y > maxPos.Y)
                value.Y = maxPos.Y;

            _position = value;

            foreach (TgxGameLayer layer in Layers)
                layer.SetOffset(value);
        }
    }

    public Vector2 MaxPosition => GetMaxPosition(Camera);

    public Vector2 ScrollFactor { get; set; }

    public bool Stationary { get; }
    public GfxCamera Camera { get; }

    public Vector2 GetMaxPosition(GfxCamera camera) => new(Math.Max(0, Size.X - camera.Resolution.X), Math.Max(0, Size.Y - camera.Resolution.Y));

    public void AddLayer(TgxGameLayer layer)
    {
        Layers.Add(layer);
    }

    public IReadOnlyList<TgxGameLayer> GetLayers()
    {
        return Layers;
    }

    public bool IsOnLimit(Edge limit)
    {
        Vector2 maxPos = MaxPosition;

        // TODO: Doesn't work very well with floats, because it can get stuck on .999. Fix by using our own scrollfactor?
        return limit switch
        {
            // In the game these are == checks, but since we're dealing with floats here they're <= and >=
            Edge.Top => Position.Y <= 0,
            Edge.Right => Position.X >= maxPos.X,
            Edge.Bottom => Position.Y >= maxPos.Y,
            Edge.Left => Position.X <= 0,
            _ => throw new ArgumentOutOfRangeException(nameof(limit), limit, null)
        };
    }
}