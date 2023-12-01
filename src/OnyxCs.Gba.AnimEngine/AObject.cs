using System;

namespace OnyxCs.Gba.AnimEngine;

public abstract class AObject
{
    public byte Priority { get; set; }

    public int SpritePriority
    {
        get => Priority >> 6;
        set
        {
            Priority &= 0x3F;
            Priority |= (byte)((value & 0x3) << 6);
        }
    }

    public int YPriority
    {
        get => Priority & 0x3F;
        set
        {
            Priority &= 0xC0;
            Priority |= (byte)(value & 0x3F);
        }
    }

    public abstract void Execute(AnimationSpriteManager animationSpriteManager, Action<ushort> soundEventCallback);
}