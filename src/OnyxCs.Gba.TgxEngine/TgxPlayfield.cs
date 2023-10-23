using System;
using BinarySerializer.Onyx.Gba;

namespace OnyxCs.Gba.TgxEngine;

public abstract class TgxPlayfield
{
    protected TgxPlayfield()
    {
        CurrentPlayfield = this;
    }

    // TODO: Currently this is just for the debug window. Might be nice to find a better solution, like a property in
    //       the current frame. Or maybe current frame stores collection of "frame components" that you can access generic?
    public static TgxPlayfield CurrentPlayfield { get; private set; }

    public TgxTileCollisionLayer CollisionLayer { get; set; }

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
}