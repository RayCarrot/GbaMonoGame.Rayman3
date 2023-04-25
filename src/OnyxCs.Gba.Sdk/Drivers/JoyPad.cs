namespace OnyxCs.Gba.Sdk;

public abstract class JoyPad
{
    public abstract bool Check(Input input);

    public abstract void Scan();
}