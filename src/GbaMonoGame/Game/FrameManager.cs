namespace GbaMonoGame;

/// <summary>
/// Manages the currently active frame.
/// </summary>
public static class FrameManager
{
    internal static Frame CurrentFrame { get; set; }
    internal static Frame NextFrame { get; set; }

    /// <summary>
    /// Sets the next frame to be made active. This will go into effect at the start of the next game frame.
    /// </summary>
    /// <param name="frame">The new frame</param>
    public static void SetNextFrame(Frame frame)
    {
        NextFrame = frame;
    }

    /// <summary>
    /// Steps the active frame and changes the active frame if scheduled to do so.
    /// </summary>
    public static void Step()
    {
        // The game doesn't clear sprites here, but rather in places such as the animation player.
        // For us this however makes more sense, so we always start each frame fresh.
        Gfx.ClearSprites();

        if (NextFrame != null)
        {
            CurrentFrame?.UnInit();

            // Clear all screens for the new frame. The game doesn't do this, but it makes
            // more sense with how this code is structured.
            Gfx.ClearScreens();

            // Initializing a new frame might take longer than 1/60th of a second, so we mark it as a load
            Engine.BeginLoad();

            CurrentFrame = NextFrame;
            NextFrame.Init();
            NextFrame = null;

            // The game doesn't return here, but it always calls VSync in the init function, so this
            // will basically do the same thing. And this way we limit the loading to a single
            // update cycle and have the next continue on as normal.
            return;
        }

        // Refresh sound events
        SoundEventsManager.RefreshEventSet();

        // Step the currently active frame
        CurrentFrame?.Step();
        
        // Update the game time by one game frame
        GameTime.Update();
    }
}