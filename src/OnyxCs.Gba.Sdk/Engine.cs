namespace OnyxCs.Gba.Sdk;

public abstract class Engine
{
    protected Engine() => Instance = this;
    public static Engine Instance { get; private set; }

    public abstract FrameMngr FrameMngr { get; }
    public abstract GameTime GameTime { get; }

    public abstract Vram Vram { get; }
    public abstract JoyPad JoyPad { get; }

    public void Step()
    {
        JoyPad.Scan();

        FrameMngr.Step();

        // In the game this call is in the frame manager, but for now this place makes more sense
        GameTime.Update();
    }
}