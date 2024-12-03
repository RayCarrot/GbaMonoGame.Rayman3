﻿using System;
using System.Runtime.CompilerServices;

namespace GbaMonoGame;

public static class MathHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Mod(float x, float m)
    {
        float r = x % m;
        return r < 0 ? r + m : r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sin256(float x)
    {
        return MathF.Sin(2 * MathF.PI * x / 256f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cos256(float x)
    {
        return MathF.Cos(2 * MathF.PI * x / 256f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Rotate256(Vector2 vector, float angle)
    {
        float cos = Cos256(angle);
        float sin = Sin256(angle);

        return new Vector2(
            x: vector.X * cos - vector.Y * sin,
            y: vector.X * sin + vector.Y * cos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Atan2_256(float x, float y)
    {
        return MathF.Atan2(y, x) / (2 * MathF.PI) * 256;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FromFixedPoint(int x)
    {
        return (float)x / 0x10000;
    }
}