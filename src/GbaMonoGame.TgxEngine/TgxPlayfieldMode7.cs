namespace GbaMonoGame.TgxEngine;

// TODO: Implement
public class TgxPlayfieldMode7 : TgxPlayfield
{
    public TgxPlayfieldMode7(PlayfieldMode7Resource playfieldResource, CachedTileKit cachedTileKit)
        : base(new TgxCamera2D(Engine.GameViewPort), playfieldResource.TileKit)
    {
    }
}