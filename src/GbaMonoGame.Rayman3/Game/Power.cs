using System;

namespace GbaMonoGame.Rayman3;

[Flags]
public enum Power : byte
{
    None = 0,
    DoubleFist = 1 << 0,
    Grab = 1 << 1,
    Climb = 1 << 2,
    SuperHelico = 1 << 3,
    BodyShot = 1 << 4,
    SuperFist = 1 << 5,
    All = 0xFF,
}