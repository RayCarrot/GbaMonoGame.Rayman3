namespace OnyxCs.Gba.Engine2d;

public abstract class Object
{
    public void ProcessMessage(Message message) => ProcessMessageImpl(message, null);
    public void ProcessMessage(Message message, object param) => ProcessMessageImpl(message, param);

    protected abstract bool ProcessMessageImpl(Message message, object param);

    public virtual void DrawDebugLayout(DebugLayout debugLayout, DebugLayoutTextureManager textureManager) { }
}