using System;
using System.Collections.Generic;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.TgxEngine;

public class TgxCluster
{
    public TgxCluster(ClusterResource cluster)
    {
        ScrollFactor = new Vector2(cluster.ScrollFactor.X, cluster.ScrollFactor.Y);
        Layers = new List<TgxTileLayer>();
        Size = new Vector2(cluster.SizeX * Constants.TileSize, cluster.SizeY * Constants.TileSize);
        Stationary = cluster.Stationary;
    }

    private Vector2 _position;

    private List<TgxTileLayer> Layers { get; }

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

            foreach (TgxTileLayer layer in Layers)
                layer.Screen.Offset = value;
        }
    }
    public Vector2 MaxPosition => new(
        x: Math.Max(0, Size.X - Gfx.GfxCamera.GameResolution.X),
        y: Math.Max(0, Size.Y - Gfx.GfxCamera.GameResolution.Y));

    public Vector2 ScrollFactor { get; }
    public bool Stationary { get; }

    public void AddLayer(TgxTileLayer layer)
    {
        Layers.Add(layer);
    }

    public IReadOnlyList<TgxTileLayer> GetLayers()
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

    // TODO: Is there a point of this?
    public Vector2 Move(Vector2 deltaPos)
    {
        Vector2 maxPos = MaxPosition;

        Vector2 newPos = new(
            x: Math.Clamp(Position.X + deltaPos.X, 0, maxPos.X), 
            y: Math.Clamp(Position.Y + deltaPos.Y, 0, maxPos.Y));
        deltaPos = newPos - Position;

        if (deltaPos is { X: 0, Y: 0 })
            return deltaPos;

        Position += deltaPos;

        foreach (TgxTileLayer layer in Layers)
            layer.Screen.Offset = Position;

        return deltaPos;
    }
}