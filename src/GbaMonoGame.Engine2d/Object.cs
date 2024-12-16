using System;

namespace GbaMonoGame.Engine2d;

public abstract class Object
{
    protected abstract bool ProcessMessageImpl(object sender, Message message, object param);

    public void ProcessMessage(object sender, Message message) => ProcessMessage(sender, message, null);
    public void ProcessMessage(object sender, Message message, object param)
    {
        if (!Enum.IsDefined(message))
            Logger.NotImplemented("Attempting to process undefined message {0}", message);

        ProcessMessageImpl(sender, message, param);
    }

    public virtual void DrawDebugLayout(DebugLayout debugLayout, DebugLayoutTextureManager textureManager) { }
}