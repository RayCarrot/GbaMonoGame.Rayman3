﻿using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Rayman3;

public class FairyGlade_M1 : FrameSideScroller
{
    public FairyGlade_M1(MapId mapId) : base(mapId) { }

    public override void Init()
    {
        base.Init();

        // TODO: Add config option for scrolling on N-Gage
        if (Engine.Settings.Platform == Platform.GBA)
        {
            TgxTileLayer cloudsLayer = ((TgxPlayfield2D)Scene.Playfield).TileLayers[0];
            TextureScreenRenderer renderer = (TextureScreenRenderer)cloudsLayer.Screen.Renderer;
            cloudsLayer.Screen.Renderer = new LevelCloudsRenderer(renderer.Texture, [32, 120, 227])
            {
                PaletteTexture = renderer.PaletteTexture
            };
        }
    }
}