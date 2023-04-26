using System;

namespace OnyxCs.Gba.Sdk;

public class FrameManager
{
    public FrameManager(Frame initialFrame)
    {
        NextFrame = initialFrame;
    }

    public Frame? CurrentFrame { get; set; }
    public Frame? NextFrame { get; set; }

    public void SetNextFrame(Frame frame) => NextFrame = frame;

    public void Step(Engine engine)
    {
        if (NextFrame != null)
        {
            CurrentFrame?.UnInit();
            NextFrame.Init(engine);
            CurrentFrame = NextFrame;
            NextFrame = null;
        }

        if (CurrentFrame == null)
            throw new Exception("A frame has to be set before running");

        CurrentFrame.Step();
    }
}