using BinarySerializer.Nintendo.GBA;

namespace OnyxCs.Gba.Sdk;

public class Sprite
{
    public Sprite(
        Vec2Int position, 
        byte spriteShape, 
        byte spriteSize, 
        OBJ_ATTR_ObjectMode mode,
        Palette palette,
        byte[] tileSet, 
        int tileIndex)
    {
        Position = position;
        SpriteShape = spriteShape;
        SpriteSize = spriteSize;
        Mode = mode;
        Palette = palette;
        TileSet = tileSet;
        TileIndex = tileIndex;
    }

    public Vec2Int Position { get; }
    
    public byte SpriteShape { get; }
    public byte SpriteSize { get; }

    public bool FlipX { get; init; }
    public bool FlipY { get; init; }

    public bool Is8Bit { get; init; }
    public int Priority { get; init; }

    public OBJ_ATTR_ObjectMode Mode { get; }

    public AffineMatrix AffineMatrix { get; init; }

    public Palette Palette { get; }
    
    public byte[] TileSet { get; }
    public int TileIndex { get; }
}