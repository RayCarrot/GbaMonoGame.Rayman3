namespace OnyxCs.Gba.Sdk;

public abstract class Engine
{
#nullable disable
    // TODO: Would be nice to eventually remove this
    protected Engine() => Instance = this;
    public static Engine Instance { get; private set; }
#nullable restore

    public abstract FrameManager FrameManager { get; }
    public abstract GameTime GameTime { get; }
    public abstract SoundManager SoundManager { get; }

    public abstract Vram Vram { get; }
    public abstract JoyPad JoyPad { get; }

    public void Step()
    {
        JoyPad.Scan();

        FrameManager.Step(this);

        // In the game this call is in the frame manager, but for now this place makes more sense
        GameTime.Update();
    }
}