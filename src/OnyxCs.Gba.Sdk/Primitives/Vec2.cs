﻿namespace OnyxCs.Gba.Sdk;

// The game uses fixed-point integers, but we can use floats instead
public readonly struct Vec2
{
    public Vec2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public float X { get; }
    public float Y { get; }

    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);
}