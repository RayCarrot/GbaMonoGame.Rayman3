using BinarySerializer.Nintendo.GBA;

namespace GbaMonoGame;

public static class Tile
{
    public const int Size = Constants.TileSize;
    public static Vector2 Left { get; } = new(-Size, 0);
    public static Vector2 Right { get; } = new(Size, 0);
    public static Vector2 Up { get; } = new(0, -Size);
    public static Vector2 Down { get; } = new(0, Size);
}