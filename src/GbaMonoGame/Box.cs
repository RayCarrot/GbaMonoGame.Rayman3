using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework;

namespace GbaMonoGame;

public readonly struct Box
{
    public Box(float minX, float minY, float maxX, float maxY)
    {
        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
    }

    public Box(Vector2 position, Vector2 size)
    {
        MinX = position.X;
        MinY = position.Y;
        MaxX = position.X + size.X;
        MaxY = position.Y + size.Y;
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
    public Box FlipX() => new(MaxX * -1, MinY, MinX * -1, MaxY);
    public Box FlipY() => new(MinX, MaxY * -1, MaxX, MinX * -1);
    public bool Intersects(Box otherBox) => otherBox.MinX < MaxX && MinX < otherBox.MaxX && otherBox.MinY < MaxY && MinY < otherBox.MaxY;
    public bool Contains(Vector2 position) => MinX <= position.X && position.X < MaxX && MinY <= position.Y && position.Y < MaxY;

    public Rectangle ToRectangle() => new((int)MinX, (int)MinY, (int)Width, (int)Height);

    public static Box Intersect(Box box1, Box box2)
    {
        float largestXMin = box2.MinX;
        if (largestXMin < box1.MinX)
            largestXMin = box1.MinX;

        float largestYMin = box2.MinY;
        if (largestYMin < box1.MinY)
            largestYMin = box1.MinY;

        float smallestXMax = box2.MaxX;
        if (box1.MaxX < smallestXMax)
            smallestXMax = box1.MaxX;

        float smallestYMax = box2.MaxY;
        if (box1.MaxY < smallestYMax)
            smallestYMax = box1.MaxY;

        if (largestXMin < smallestXMax && largestYMin < smallestYMax)
            return new Box(largestXMin, largestYMin, smallestXMax, smallestYMax);
        else
            return Empty;
    }

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