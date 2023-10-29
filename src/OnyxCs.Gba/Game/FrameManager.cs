namespace OnyxCs.Gba;

public static class FrameManager
{
    internal static Frame CurrentFrame { get; set; }
    internal static Frame NextFrame { get; set; }

    public static void SetNextFrame(Frame frame)
    {
        NextFrame = frame;
    }

    public static void Step()
    {
        if (NextFrame != null)
        {
            CurrentFrame?.UnInit();

            // Reset all screens for the new frame
            foreach (GfxScreen screen in Gfx.Screens)
            {
                screen.IsEnabled = false;
                screen.Renderer = null;
            }

            CurrentFrame = NextFrame;
            NextFrame.Init();
            NextFrame = null;
        }

        CurrentFrame?.Step();
    }
}