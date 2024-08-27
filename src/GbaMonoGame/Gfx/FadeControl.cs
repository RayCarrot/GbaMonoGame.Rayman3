namespace GbaMonoGame;

// BLDCNT on GBA
public readonly struct FadeControl
{
    public FadeControl(FadeMode mode)
    {
        Mode = mode;
        Flags = FadeFlags.Default;
    }

    public FadeControl(FadeMode mode, FadeFlags flags)
    {
        Mode = mode;
        Flags = flags;
    }

    public static FadeControl None => new();

    public FadeMode Mode { get; init; }
    public FadeFlags Flags { get; init; } // Simplified way of setting a target
}