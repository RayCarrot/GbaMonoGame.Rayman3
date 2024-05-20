using System;

namespace GbaMonoGame.AnimEngine;

// On GBA setting the priority is inlined. Sprite is set using the mask 0x3fff and Y is set using the mask 0xc0ff.
public abstract class AObject
{
    public byte Priority { get; private set; }

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

    public abstract void Execute(Action<short> soundEventCallback);
}