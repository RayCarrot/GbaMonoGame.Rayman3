using System;
using System.Collections.Generic;
using BinarySerializer.Nintendo.GBA;
using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.TgxEngine;

public class TgxCluster
{
    // TODO: Clean up math code, use more operators
 
    public TgxCluster(ClusterResource cluster)
    {
        ScrollFactor = new Vec2(cluster.ScrollFactor.X, cluster.ScrollFactor.Y);
        // TODO: Support different screen sizes
        MaxPosition = new Vec2Int(
            x: cluster.SizeX * Constants.TileSize - Constants.ScreenWidth, 
            y: cluster.SizeY * Constants.TileSize - Constants.ScreenHeight);
        Layers = new List<TgxTileLayer>();
        CanNotMove = cluster.Stationary;
    }

    private int Width { get; set; }
    private int Height { get; set; }

    private Vec2Int MaxPosition { get; }

    private Vec2Int Position { get; set; }

    private Vec2 ScrollFactor { get; }
    private Vec2 Scrolled { get; set; }

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

    public bool IsOnLimit(TgxClusterLimit limit)
    {
        return limit switch
        {
            TgxClusterLimit.Top => Position.Y == 0,
            TgxClusterLimit.Right => Position.X == MaxPosition.X,
            TgxClusterLimit.Bottom => Position.Y == MaxPosition.Y,
            TgxClusterLimit.Left => Position.X == 0,
            _ => throw new ArgumentOutOfRangeException(nameof(limit), limit, null)
        };
    }

    // TODO: This is confusing. Maybe change?
    public Vec2Int Scroll(Vec2Int deltaPos)
    {
        Scrolled += new Vec2(deltaPos.X * ScrollFactor.X, deltaPos.Y * ScrollFactor.Y);
        Scrolled = new Vec2(
            x: MathHelpers.Clamp(Scrolled.X, 0, MaxPosition.X),
            y: MathHelpers.Clamp(Scrolled.Y, 0, MaxPosition.Y));

        return new Vec2Int((int)Scrolled.X - Position.X, (int)Scrolled.Y - Position.Y);
    }

    public Vec2Int Move(Vec2Int deltaPos)
    {
        Vec2Int newPos = new(
            x: MathHelpers.Clamp(Position.X + deltaPos.X, 0, MaxPosition.X), 
            y: MathHelpers.Clamp(Position.Y + deltaPos.Y, 0, MaxPosition.Y));
        deltaPos = newPos - Position;

        if (deltaPos is { X: 0, Y: 0 })
            return deltaPos;

        Position += deltaPos;

        foreach (TgxTileLayer layer in Layers)
            layer.Screen.SetOffset(Position);

        return deltaPos;
    }
}