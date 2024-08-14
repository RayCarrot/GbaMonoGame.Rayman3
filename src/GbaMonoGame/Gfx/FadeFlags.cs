using System;

namespace GbaMonoGame;

// Sometimes we need to fade as certain points in the draw process, so we use this. This is
// roughly an equivalent to BLDCNT on GBA, but much simpler.
[Flags]
public enum FadeFlags
{
    Default = 0,

    Screen0 = 1 << 0,
    Screen1 = 1 << 1,
    Screen2 = 1 << 2,
    Screen3 = 1 << 3,

    Sprites0 = 1 << 4,
    Sprites1 = 1 << 5,
    Sprites2 = 1 << 6,
    Sprites3 = 1 << 7,
}