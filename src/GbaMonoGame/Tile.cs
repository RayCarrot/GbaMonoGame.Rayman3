using BinarySerializer.Nintendo.GBA;

namespace GbaMonoGame.Engine2d;

public static class Tile
{
    public const int Size = Constants.TileSize;
    public static Vector2 Left { get; } = new(-Constants.TileSize, 0);
    public static Vector2 Right { get; } = new(Constants.TileSize, 0);
    public static Vector2 Up { get; } = new(0, -Constants.TileSize);
    public static Vector2 Down { get; } = new(0, Constants.TileSize);
}