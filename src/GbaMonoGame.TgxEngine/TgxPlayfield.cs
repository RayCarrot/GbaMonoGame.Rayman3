﻿using System;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.TgxEngine;

public abstract class TgxPlayfield
{
    protected TgxPlayfield(TgxCamera camera, TileKit tileKit)
    {
        Camera = camera;

        if (tileKit.AnimatedTileKits != null)
            AnimatedTilekitManager = new AnimatedTilekitManager(tileKit.AnimatedTileKits);
    }

    public TgxCamera Camera { get; }
    public AnimatedTilekitManager AnimatedTilekitManager { get; }
    public TgxTilePhysicalLayer PhysicalLayer { get; set; }

    public static TgxPlayfield Load(PlayfieldResource playfieldResource) => Load<TgxPlayfield>(playfieldResource);

    public static T Load<T>(PlayfieldResource playfieldResource)
        where T : TgxPlayfield
    {
        TgxPlayfield playfield = playfieldResource.Type switch
        {
            PlayfieldType.Playfield2D => new TgxPlayfield2D(playfieldResource.Playfield2D),
            PlayfieldType.PlayfieldMode7 => new TgxPlayfieldMode7(playfieldResource.PlayfieldMode7),
            PlayfieldType.PlayfieldScope => throw new NotImplementedException("Not implemented loading PlayfieldScope"),
            _ => throw new NotImplementedException($"Unsupported playfield type {playfieldResource.Type}")
        };

        return playfield as T ?? throw new Exception($"Playfield of type {playfield.GetType()} is not of expected type {typeof(T)}");
    }

    public byte GetPhysicalValue(Point mapPoint)
    {
        // If we're above the map, always return empty type
        if (mapPoint.Y < 0)
            return 0xFF;
        // If we're below the map, always return solid type. Game doesn't do this check, but
        // this is essentially the result since there are usually 0s after the map.
        else if (mapPoint.Y >= PhysicalLayer.Height)
            return 0;

        int index = mapPoint.Y * PhysicalLayer.Width + mapPoint.X;

        // Safety check to avoid out of bounds
        if (index >= PhysicalLayer.CollisionMap.Length || index < 0)
            return 0xFF;

        return PhysicalLayer.CollisionMap[mapPoint.Y * PhysicalLayer.Width + mapPoint.X];
    }

    public void Step()
    {
        AnimatedTilekitManager?.Step();
    }

    public virtual void UnInit()
    {
        Camera.UnInit();
    }
}