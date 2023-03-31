namespace OnyxCs.Gba.Sdk;

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
}