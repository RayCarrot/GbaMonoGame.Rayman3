namespace OnyxCs.Gba;

/// <summary>
/// Manages tracking elapsed game time.
/// </summary>
public static class GameTime
{
    /// <summary>
    /// Indicates if tracking the game time is currently paused.
    /// </summary>
    public static bool IsPaused { get; set; }

    /// <summary>
    /// The number of elapsed game frames since the game started.
    /// </summary>
    public static long ElapsedFrames { get; private set; }

    /// <summary>
    /// Updates the tracked game time by one frame if not paused.
    /// </summary>
    internal static void Update()
    {
        if (!IsPaused)
            ElapsedFrames++;
    }
}