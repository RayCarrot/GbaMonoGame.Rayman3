using BinarySerializer.Ubisoft.GbaEngine;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public class FairyGlade_M2 : FrameSideScroller
{
    public FairyGlade_M2(MapId mapId) : base(mapId) { }

    private TextBoxDialog TextBox { get; set; }

    public override void Init()
    {
        base.Init();

        TextBox = new TextBoxDialog();
        Scene.AddDialog(TextBox, false, false);

        // TODO: Add config option for scrolling on N-Gage
        if (Engine.Settings.Platform == Platform.GBA)
        {
            TgxTileLayer cloudsLayer = Scene.Playfield.TileLayers[0];
            cloudsLayer.Screen.Renderer = new LevelCloudsRenderer(((TextureScreenRenderer)cloudsLayer.Screen.Renderer).Texture, new[]
            {
                32, 120, 227
            });
        }
    }
}