namespace OnyxCs.Gba.TgxEngine;

public abstract class TgxGameLayer
{
    protected TgxGameLayer(GameLayerResource resource)
    {
        Width = resource.Width;
        Height = resource.Height;
    }

    public int Width { get; }
    public int Height { get; }
}