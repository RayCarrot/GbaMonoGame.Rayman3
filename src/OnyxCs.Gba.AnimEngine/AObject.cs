using System;

namespace OnyxCs.Gba.AnimEngine;

public abstract class AObject
{
    public abstract void Execute(AnimationSpriteManager animationSpriteManager, Action<ushort> soundEventCallback);
}