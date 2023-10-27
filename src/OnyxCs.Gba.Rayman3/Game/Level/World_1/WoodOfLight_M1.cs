using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public class WoodOfLight_M1 : FrameSideScroller
{
    public WoodOfLight_M1(MapId mapId) : base(mapId) { }

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