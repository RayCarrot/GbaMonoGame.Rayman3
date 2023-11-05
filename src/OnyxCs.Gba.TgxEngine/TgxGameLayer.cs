using BinarySerializer.Nintendo.GBA;

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

    public int PixelWidth => Width * Constants.TileSize;
    public int PixelHeight => Height * Constants.TileSize;
}