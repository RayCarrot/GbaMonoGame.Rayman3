namespace OnyxCs.Gba;

public abstract class Frame
{
    public bool EndOfFrame { get; set; }

    public virtual void Init() { }
    public virtual void UnInit() { }
    public abstract void Step();
}