namespace GbaMonoGame;

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

    public AffineMatrix(float rotation256, float scaleX, float scaleY)
    {
        Pa = scaleX * MathHelpers.Cos256(rotation256);
        Pb = scaleX * MathHelpers.Sin256(rotation256);
        Pc = scaleY * -MathHelpers.Sin256(rotation256);
        Pd = scaleY * MathHelpers.Cos256(rotation256);
    }

    public float Pa { get; }
    public float Pb { get; }
    public float Pc { get; }
    public float Pd { get; }

    public static AffineMatrix Identity => new(1, 0, 0, 1);
}