using System;
using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.TgxEngine;

public abstract class TgxPlayfield
{
    public TgxTilePhysicalLayer PhysicalLayer { get; set; }

    public static TgxPlayfield Load(PlayfieldResource playfieldResource) => Load<TgxPlayfield>(playfieldResource);

    public static T Load<T>(PlayfieldResource playfieldResource)
        where T : TgxPlayfield
    {
        TgxPlayfield playfield = playfieldResource.Type switch
        {
            PlayfieldType.Playfield2d => new TgxPlayfield2D(playfieldResource),
            PlayfieldType.PlayfieldMode7 => throw new NotImplementedException("Not implemented loading PlayfieldMode7"),
            PlayfieldType.PlayfieldScope => throw new NotImplementedException("Not implemented loading PlayfieldScope"),
            _ => throw new NotImplementedException($"Unsupported playfield type {playfieldResource.Type}")
        };

        return playfield as T ?? throw new Exception($"Playfield of type {playfield.GetType()} is not of expected type {typeof(T)}");
    }

    public byte GetPhysicalValue(Point mapPoint)
    {
        if (mapPoint.X < 0 || mapPoint.Y < 0 || mapPoint.X >= PhysicalLayer.Width || mapPoint.Y >= PhysicalLayer.Height)
            return 0xFF;
        else
            return PhysicalLayer.CollisionMap[mapPoint.Y * PhysicalLayer.Width + mapPoint.X];
    }
}