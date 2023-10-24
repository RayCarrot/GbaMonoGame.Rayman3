using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public class WoodLight : FrameSideScroller
{
    public WoodLight(MapId mapId) : base(mapId) { }

    public override void Init()
    {
        base.Init();

        TgxTileLayer cloudsLayer = Scene.Playfield.TileLayers[0];
        cloudsLayer.Screen.Renderer = new LevelCloudsRenderer(((TextureScreenRenderer)cloudsLayer.Screen.Renderer).Texture);
    }

    public override void Step()
    {
        base.Step();
        // TODO: Implement
    }
}