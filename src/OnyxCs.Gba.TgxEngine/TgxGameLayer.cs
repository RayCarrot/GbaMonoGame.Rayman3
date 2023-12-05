using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.TgxEngine;

public abstract class TgxGameLayer
{
    protected TgxGameLayer(GameLayerResource gameLayerResource)
    {
        Width = gameLayerResource.Width;
        Height = gameLayerResource.Height;
    }

    public int Width { get; }
    public int Height { get; }

    public int PixelWidth => Width * Constants.TileSize;
    public int PixelHeight => Height * Constants.TileSize;

    public abstract void SetOffset(Vector2 offset);
}