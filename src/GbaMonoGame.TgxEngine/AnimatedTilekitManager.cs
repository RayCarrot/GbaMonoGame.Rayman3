using System.Collections.Generic;
using BinarySerializer.Ubisoft.GbaEngine;

namespace GbaMonoGame.TgxEngine;

public class AnimatedTilekitManager
{
    public AnimatedTilekitManager(AnimatedTileKit[] animatedTileKits)
    {
        Animations = new TileKitAnimation[animatedTileKits.Length];
        for (int i = 0; i < Animations.Length; i++)
            Animations[i] = new TileKitAnimation(animatedTileKits[i]);

        TileRenderers = new List<TileMapScreenRenderer>();
    }

    private TileKitAnimation[] Animations { get; }
    private List<TileMapScreenRenderer> TileRenderers { get; }

    public void AddRenderer(TileMapScreenRenderer renderer)
    {
        TileRenderers.Add(renderer);
    }

    public void Step()
    {
        foreach (TileKitAnimation anim in Animations)
        {
            anim.Timer++;

            if (anim.Timer >= anim.TileKit.Speed)
            {
                anim.Frame++;
                anim.Timer = 0;

                if (anim.Frame >= anim.TileKit.FramesCount)
                    anim.Frame = 0;

                for (int i = 0; i < anim.TileKit.TilesCount; i++)
                {
                    foreach (TileMapScreenRenderer renderer in TileRenderers)
                    {
                        if (renderer.Is8Bit == anim.TileKit.Is8Bit)
                            renderer.ReplaceTile(anim.TileKit.Tiles[i], anim.TileKit.Tiles[i] + anim.Frame * anim.TileKit.TilesStep);
                    }
                }
            }
        }
    }

    private class TileKitAnimation
    {
        public TileKitAnimation(AnimatedTileKit tileKit)
        {
            TileKit = tileKit;
        }

        public AnimatedTileKit TileKit { get; }
        public byte Frame { get; set; }
        public byte Timer { get; set; }
    }
}