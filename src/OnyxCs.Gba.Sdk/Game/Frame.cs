namespace OnyxCs.Gba.Sdk;

public abstract class Frame
{
    public abstract void Init(FrameMngr frameMngr);
    public abstract void UnInit();
    public abstract void Step();
}