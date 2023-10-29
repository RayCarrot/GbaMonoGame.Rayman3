namespace OnyxCs.Gba.TgxEngine;

public class TgxTilePhysicalLayer : TgxGameLayer
{
    public TgxTilePhysicalLayer(GameLayerResource resource) : base(resource)
    {
        CollisionMap = resource.CollisionMap;
    }

    public byte[] CollisionMap { get; }
}