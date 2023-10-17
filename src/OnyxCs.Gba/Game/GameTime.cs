namespace OnyxCs.Gba;

public static class GameTime
{
    public static bool IsPaused { get; private set; }
    public static long ElapsedFrames { get; private set; }

    public static void Pause() => IsPaused = true;
    public static void Resume() => IsPaused = false;

    public static void Update()
    {
        if (!IsPaused)
            ElapsedFrames++;
    }
}