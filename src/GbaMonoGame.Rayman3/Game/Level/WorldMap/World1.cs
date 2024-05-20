using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Rayman3;

public class World1 : World
{
    public World1(MapId mapId) : base(mapId) { }

    public override void Init()
    {
        base.Init();

        // TODO: Add config option for scrolling on N-Gage
        if (Engine.Settings.Platform == Platform.GBA)
        {
            TgxTileLayer cloudsLayer = ((TgxPlayfield2D)Scene.Playfield).TileLayers[0];
            cloudsLayer.Screen.Renderer = new LevelCloudsRenderer(((TextureScreenRenderer)cloudsLayer.Screen.Renderer).Texture, new[]
            {
                32, 120, 227
            });
        }
    }
}