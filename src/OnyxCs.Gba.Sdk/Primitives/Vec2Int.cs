namespace OnyxCs.Gba.Sdk;

public readonly struct Vec2Int
{
    public Vec2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }

    public static Vec2Int operator +(Vec2Int a, Vec2Int b) => new(a.X + b.X, a.Y + b.Y);
    public static Vec2Int operator -(Vec2Int a, Vec2Int b) => new(a.X - b.X, a.Y - b.Y);
}