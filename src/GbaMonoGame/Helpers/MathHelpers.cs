using System;

namespace GbaMonoGame;

public static class MathHelpers
{
    public static float Mod(float x, float m)
    {
        float r = x % m;
        return r < 0 ? r + m : r;
    }

    public static float Sin256(float x)
    {
        return MathF.Sin(2 * MathF.PI * x / 256f);
    }

    public static float Cos256(float x)
    {
        return MathF.Cos(2 * MathF.PI * x / 256f);
    }

    public static float FromFixedPoint(int x)
    {
        return (float)x / 0x10000;
    }
}