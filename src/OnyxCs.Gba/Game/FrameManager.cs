﻿namespace OnyxCs.Gba;

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
            NextFrame.Init();
            CurrentFrame = NextFrame;
            NextFrame = null;
        }

        CurrentFrame?.Step();
    }
}