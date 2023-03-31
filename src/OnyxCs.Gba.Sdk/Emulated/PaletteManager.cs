using BinarySerializer.Nintendo.GBA;

namespace OnyxCs.Gba.Sdk;

public abstract class PaletteManager
{
    public abstract void Load(Palette palette);
    public abstract void UnloadAll();
}