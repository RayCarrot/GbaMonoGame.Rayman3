using System;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.TgxEngine;

public abstract class TgxPlayfield
{
    protected TgxPlayfield(TgxCamera camera)
    {
        Camera = camera;
    }

    public TgxCamera Camera { get; }

    public TgxTilePhysicalLayer PhysicalLayer { get; set; }

    public static TgxPlayfield Load(PlayfieldResource playfieldResource, CachedTileKit cachedTileKit = null) => Load<TgxPlayfield>(playfieldResource, cachedTileKit);

    public static T Load<T>(PlayfieldResource playfieldResource, CachedTileKit cachedTileKit = null)
        where T : TgxPlayfield
    {
        TgxPlayfield playfield = playfieldResource.Type switch
        {
            PlayfieldType.Playfield2D => new TgxPlayfield2D(playfieldResource.Playfield2D, cachedTileKit),
            PlayfieldType.PlayfieldMode7 => throw new NotImplementedException("Not implemented loading PlayfieldMode7"),
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

    public virtual void UnInit()
    {
        Camera.UnInit();
    }
}