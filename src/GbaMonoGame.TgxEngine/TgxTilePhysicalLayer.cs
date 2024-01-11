using Microsoft.Xna.Framework;

namespace GbaMonoGame.TgxEngine;

public class TgxTilePhysicalLayer : TgxGameLayer
{
    public TgxTilePhysicalLayer(GameLayerResource gameLayerResource, GfxCamera camera) : base(gameLayerResource)
    {
        CollisionMap = gameLayerResource.PhysicalLayer.CollisionMap;

        // TODO: Don't do this unless some debug mode is enabled or it'll impact performance
        // Collision map screen for debugging
        DebugScreen = new GfxScreen(-1)
        {
            IsEnabled = false,
            Offset = Vector2.Zero,
            Priority = 0,
            Wrap = false,
            Is8Bit = null,
            Camera = camera,
            Renderer = new CollisionMapScreenRenderer(camera, Width, Height, CollisionMap)
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