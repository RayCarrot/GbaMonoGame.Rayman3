namespace OnyxCs.Gba.Sdk;

public struct Color
{
    public Color(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }

    public float R { get; }
    public float G { get; }
    public float B { get; }
}