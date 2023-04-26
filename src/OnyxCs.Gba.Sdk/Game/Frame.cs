#nullable disable // TODO: Would be nice to be able to enable it for frame classes, but doesn't work well here
namespace OnyxCs.Gba.Sdk;

public abstract class Frame
{
    public Engine Engine { get; private set; }

    public virtual void Init(Engine engine) => Engine = engine;
    public abstract void UnInit();
    public abstract void Step();
}