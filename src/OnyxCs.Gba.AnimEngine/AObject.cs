using System;

namespace OnyxCs.Gba.AnimEngine;

public abstract class AObject
{
    // TODO: Add prio. Includes BG, OBJ and Y prio? Retroactively go through animated objects to see if we forgot to set it!

    public abstract void Execute(AnimationSpriteManager animationSpriteManager, Action<ushort> soundEventCallback);
}