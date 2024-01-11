using System;

namespace GbaMonoGame.Rayman3;

[Flags]
public enum CheatFlags
{
    None = 0,
    Invulnerable = 1 << 0,
    AllPowers = 1 << 1,
    InfiniteLives = 1 << 2,
}