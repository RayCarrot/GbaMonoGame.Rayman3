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
        // TODO: Support different screen sizes
        MaxPosition = new Vector2(
            x: cluster.SizeX * Constants.TileSize - Constants.ScreenWidth, 
            y: cluster.SizeY * Constants.TileSize - Constants.ScreenHeight);
        Layers = new List<TgxTileLayer>();
        Stationary = cluster.Stationary;
    }

    private Vector2 _position;

    private List<TgxTileLayer> Layers { get; }

    public Vector2 MaxPosition { get; }
    public Vector2 ScrollFactor { get; }
    public bool Stationary { get; }

    public Vector2 Position
    {
        get => _position;
        set
        {
            // TODO: Also check for less than 0?
            if (value.X > MaxPosition.X)
                value.X = MaxPosition.X;
            if (value.Y > MaxPosition.Y)
                value.Y = MaxPosition.Y;

            _position = value;

            foreach (TgxTileLayer layer in Layers)
                layer.Screen.Offset = value;
        }
    }

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
        // TODO: Doesn't work very well with floats, because it can get stuck on .999. Fix by using our own scrollfactor?
        return limit switch
        {
            // In the game these are == checks, but since we're dealing with floats here they're <= and >=
            Edge.Top => Position.Y <= 0,
            Edge.Right => Position.X >= MaxPosition.X,
            Edge.Bottom => Position.Y >= MaxPosition.Y,
            Edge.Left => Position.X <= 0,
            _ => throw new ArgumentOutOfRangeException(nameof(limit), limit, null)
        };
    }

    // TODO: Is there a point of this?
    public Vector2 Move(Vector2 deltaPos)
    {
        Vector2 newPos = new(
            x: Math.Clamp(Position.X + deltaPos.X, 0, MaxPosition.X), 
            y: Math.Clamp(Position.Y + deltaPos.Y, 0, MaxPosition.Y));
        deltaPos = newPos - Position;

        if (deltaPos is { X: 0, Y: 0 })
            return deltaPos;

        Position += deltaPos;

        foreach (TgxTileLayer layer in Layers)
            layer.Screen.Offset = Position;

        return deltaPos;
    }
}