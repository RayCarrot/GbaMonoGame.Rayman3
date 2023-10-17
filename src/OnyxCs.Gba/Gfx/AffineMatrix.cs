namespace OnyxCs.Gba;

public readonly struct AffineMatrix
{
    public AffineMatrix(int pa, int pb, int pc, int pd)
    {
        Pa = pa;
        Pb = pb;
        Pc = pc;
        Pd = pd;
    }

    public int Pa { get; }
    public int Pb { get; }
    public int Pc { get; }
    public int Pd { get; }
}