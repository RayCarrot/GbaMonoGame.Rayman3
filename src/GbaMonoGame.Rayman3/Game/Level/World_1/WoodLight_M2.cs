using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Rayman3;

public class WoodLight_M2 : FrameSideScroller
{
    public WoodLight_M2(MapId mapId) : base(mapId) { }

    public override void Init()
    {
        base.Init();

        Scene.AddDialog(new TextBoxDialog(), false, false);

        // TODO: Add config option for scrolling on N-Gage
        if (Engine.Settings.Platform == Platform.GBA)
        {
            TgxTileLayer cloudsLayer = Scene.Playfield.TileLayers[0];
            cloudsLayer.Screen.Renderer = new LevelCloudsRenderer(((TextureScreenRenderer)cloudsLayer.Screen.Renderer).Texture, new[]
            {
                15, 71, 227
            });
        }
    }
}