﻿using System;

namespace GbaMonoGame;

// TODO: Replace this with a more game-specific input system where we have input actions for things like jump, confirm in menu etc.
[Flags]
public enum GbaInput
{
    None = 0,
    A = 1 << 0,
    B = 1 << 1,
    Select = 1 << 2,
    Start = 1 << 3,
    Right = 1 << 4,
    Left = 1 << 5,
    Up = 1 << 6,
    Down = 1 << 7,
    R = 1 << 8,
    L = 1 << 9,
}