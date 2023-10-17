using System;
using System.Collections.Generic;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.TgxEngine;

public class TgxCluster
{
    // TODO: Clean up math code, use more operators
 
    public TgxCluster(ClusterResource cluster)
    {
        ScrollFactor = new Vector2(cluster.ScrollFactor.X, cluster.ScrollFactor.Y);
        // TODO: Support different screen sizes
        MaxPosition = new Vector2(
            x: cluster.SizeX * Constants.TileSize - Constants.ScreenWidth, 
            y: cluster.SizeY * Constants.TileSize - Constants.ScreenHeight);
        Layers = new List<TgxTileLayer>();
        CanNotMove = cluster.Stationary;
    }

    private int Width { get; set; }
    private int Height { get; set; }

    private Vector2 MaxPosition { get; }

    private Vector2 Position { get; set; }

    private Vector2 ScrollFactor { get; }
    private Vector2 Scrolled { get; set; }

    private List<TgxTileLayer> Layers { get; }

    public bool CanNotMove { get; }

    public void AddLayer(TgxTileLayer layer)
    {
        if (Layers.Count == 0)
        {
            Width = layer.Width;
            Height = layer.Height;
        }

        Layers.Add(layer);
    }

    public bool IsOnLimit(Edge limit)
    {
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

    // TODO: This is confusing. Maybe change?
    public Vector2 Scroll(Vector2 deltaPos)
    {
        Scrolled += new Vector2(deltaPos.X * ScrollFactor.X, deltaPos.Y * ScrollFactor.Y);
        Scrolled = new Vector2(
            x: Math.Clamp(Scrolled.X, 0, MaxPosition.X),
            y: Math.Clamp(Scrolled.Y, 0, MaxPosition.Y));

        return new Vector2((int)Scrolled.X - Position.X, (int)Scrolled.Y - Position.Y);
    }

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
            layer.Screen.Offset = new Vector2(Position.X, Position.Y);

        return deltaPos;
    }
}