using System;
using BinarySerializer.Onyx.Gba;

namespace OnyxCs.Gba.TgxEngine;

public abstract class TgxPlayfield
{
    public static TgxPlayfield? Instance { get; protected set; }

    public TgxTileCollisionLayer? CollisionLayer { get; set; }

    public static TgxPlayfield Load(PlayfieldResource playfieldResource)
    {
        if (playfieldResource.Type == PlayfieldType.Playfield2d)
            return new TgxPlayfield2D(playfieldResource);
        else if (playfieldResource.Type == PlayfieldType.PlayfieldMode7)
            throw new NotImplementedException("Not implemented loading PlayfieldMode7");
        else if (playfieldResource.Type == PlayfieldType.PlayfieldScope)
            throw new NotImplementedException("Not implemented loading PlayfieldScope");
        else
            throw new NotImplementedException($"Unsupported playfield type {playfieldResource.Type}");
    }
}