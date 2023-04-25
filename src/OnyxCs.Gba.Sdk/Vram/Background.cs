using BinarySerializer.Nintendo.GBA;

namespace OnyxCs.Gba.Sdk;

public class Background
{
    public int Priority { get; set; }
    public OverflowProcess OverflowProcess { get; set; }
    public bool Is8Bit { get; set; }
    public Vec2Int Offset { get; set; }
    public bool IsEnabled { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }

    public MapTile[]? Map { get; set; }
    public byte[]? TileSet { get; set; }
    public Palette? Palette { get; set; }
}