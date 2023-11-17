using System;

namespace OnyxCs.Gba.AnimEngine;

[Flags]
public enum ScreenAnchor
{
    None = 0,
    Right = 1 << 0,
    Bottom = 1 << 1,
}