using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.TgxEngine;

public class TgxTilePhysicalLayer : TgxGameLayer
{
    public TgxTilePhysicalLayer(GameLayerResource resource) : base(resource)
    {
        CollisionMap = resource.CollisionMap;

        // TODO: Don't do this unless some debug mode is enabled or it'll impact performance
        // Collision map screen for debugging
        DebugScreen = new GfxScreen(-1)
        {
            IsEnabled = false,
            Offset = Vector2.Zero,
            Priority = 0,
            Wrap = false,
            Is8Bit = null,
            Renderer = new CollisionMapScreenRenderer(Width, Height, CollisionMap)
        };
        Gfx.AddScreen(DebugScreen);
    }

    public GfxScreen DebugScreen { get; }
    public byte[] CollisionMap { get; }

    public override void SetOffset(Vector2 offset)
    {
        DebugScreen.Offset = offset;
    }
}