using System;

namespace GbaMonoGame.Rayman3;

[Flags]
public enum ActorSoundFlags
{
    None = 0,
    FlyingBomb = 1 << 1,
    Urchin = 1 << 3,
}