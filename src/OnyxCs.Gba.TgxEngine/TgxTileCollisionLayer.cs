namespace OnyxCs.Gba.TgxEngine;

public class TgxTileCollisionLayer : GameLayer
{
    public TgxTileCollisionLayer(GameLayerResource resource) : base(resource)
    {
        CollisionMap = resource.CollisionMap;
    }

    public byte[] CollisionMap { get; }
}