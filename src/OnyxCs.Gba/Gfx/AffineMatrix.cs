namespace OnyxCs.Gba;

/// <summary>
/// An affine matrix for rotation/scaling.
/// </summary>
public readonly struct AffineMatrix
{
    public AffineMatrix(float pa, float pb, float pc, float pd)
    {
        Pa = pa;
        Pb = pb;
        Pc = pc;
        Pd = pd;
    }

    public float Pa { get; }
    public float Pb { get; }
    public float Pc { get; }
    public float Pd { get; }

    public static AffineMatrix Identity => new(1, 0, 0, 1);
}