using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba;

public readonly struct Box
{
    public Box(float minX, float minY, float maxX, float maxY)
    {
        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
    }

    public Box(EngineBox engineBox)
    {
        MinX = engineBox.MinX;
        MinY = engineBox.MinY;
        MaxX = engineBox.MaxX;
        MaxY = engineBox.MaxY;
    }

    public Box(ChannelBox channelBox)
    {
        MinX = channelBox.MinX;
        MinY = channelBox.MinY;
        MaxX = channelBox.MaxX;
        MaxY = channelBox.MaxY;
    }

    public static Box Empty { get; } = new(0, 0, 0, 0);

    public float MinX { get; }
    public float MinY { get; }
    public float MaxX { get; }
    public float MaxY { get; }

    public float Width => MaxX - MinX;
    public float Height => MaxY - MinY;

    public Vector2 Center => new(Width / 2 + MinX, Height / 2 + MinY);

    public Box Offset(Vector2 offset) => new(MinX + offset.X, MinY + offset.Y, MaxX + offset.X, MaxY + offset.Y);
    public bool Intersects(Box otherBox) => otherBox.MinX < MaxX && MinX < otherBox.MaxX && otherBox.MinY < MaxY && MinY < otherBox.MaxY;

    public Rectangle ToRectangle() => new((int)MinX, (int)MinY, (int)Width, (int)Height);

    public static bool operator ==(Box a, Box b)
    {
        return a.MinX == b.MinX && a.MinY == b.MinY && a.MaxX == b.MaxX && a.MaxY == b.MaxY;
    }

    public static bool operator !=(Box a, Box b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        if (obj is Box box)
            return this == box;

        return false;
    }

    public bool Equals(Box other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return (((17 * 23 + MinX.GetHashCode()) * 23 + MinY.GetHashCode()) * 23 + MaxX.GetHashCode()) * 23 + MaxY.GetHashCode();
    }

}