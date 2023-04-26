using System;
using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.AnimEngine;

public abstract class AObject
{
    public abstract void Load();
    public abstract void Execute(Vram vram, Action<int> soundEventCallback);
}