namespace OnyxCs.Gba.TgxEngine;

public abstract class GameLayer
{
    protected GameLayer(GameLayerResource resource)
    {
        Width = resource.Width;
        Height = resource.Height;
    }

    public int Width { get; }
    public int Height { get; }
}