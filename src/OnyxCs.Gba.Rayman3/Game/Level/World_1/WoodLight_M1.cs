using BinarySerializer.Onyx.Gba;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public class WoodLight_M1 : FrameSideScroller
{
    public WoodLight_M1(MapId mapId) : base(mapId) { }

    public override void Init()
    {
        base.Init();

        // TODO: Allow scrolling on N-Gage too?
        if (Engine.Settings.Platform == Platform.GBA)
        {
            TgxTileLayer cloudsLayer = Scene.Playfield.TileLayers[0];
            cloudsLayer.Screen.Renderer = new LevelCloudsRenderer(((TextureScreenRenderer)cloudsLayer.Screen.Renderer).Texture);
        }
    }

    public override void Step()
    {
        base.Step();
        // TODO: Implement
    }
}