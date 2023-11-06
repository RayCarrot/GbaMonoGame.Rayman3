using System.Diagnostics;

namespace OnyxCs.Gba;

public static class SoundManager
{
    public static void Play(int id, int unknown)
    {
        // TODO: Implement
        Debug.WriteLine($"Play sound {id}");
    }

    public static bool IsPlaying(int id)
    {
        // TODO: Implement
        return false;
    }

    public static void FUN_080ac468(int id, float unknown)
    {
        // TODO: Implement. Sound speed?
    }
}