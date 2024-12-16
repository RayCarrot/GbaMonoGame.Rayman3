using BinarySerializer.Ubisoft.GbaEngine;

namespace GbaMonoGame.TgxEngine;

public class TileKitAnimation
{
    public TileKitAnimation(AnimatedTileKit tileKit)
    {
        TileKit = tileKit;
    }

    public AnimatedTileKit TileKit { get; }
    public byte Frame { get; set; }
    public byte Timer { get; set; }
}