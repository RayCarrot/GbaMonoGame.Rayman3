namespace OnyxCs.Gba.TgxEngine;

public class TgxTileCollisionLayer : TgxGameLayer
{
    public TgxTileCollisionLayer(GameLayerResource resource) : base(resource)
    {
        CollisionMap = resource.CollisionMap;
    }

    public byte[] CollisionMap { get; }
}