using System;

namespace GbaMonoGame.Engine2d;

public abstract class Object
{
    public void ProcessMessage(Message message) => ProcessMessage(message, null);
    public void ProcessMessage(Message message, object param)
    {
        if (!Enum.IsDefined(message))
            Logger.NotImplemented("Attempting to process undefined message {0}", message);

        ProcessMessageImpl(message, param);
    }

    protected abstract bool ProcessMessageImpl(Message message, object param);

    public virtual void DrawDebugLayout(DebugLayout debugLayout, DebugLayoutTextureManager textureManager) { }
}