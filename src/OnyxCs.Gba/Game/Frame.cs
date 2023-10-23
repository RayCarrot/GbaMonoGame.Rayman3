namespace OnyxCs.Gba;

public abstract class Frame
{
    public virtual void Init()
    {
        Gfx.Clear();
    }
    public virtual void UnInit() { }
    public abstract void Step();
}