using System.Diagnostics;
using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.MonoGame;

public class MonoGameSoundManager : SoundManager
{
    public override void Play(int id)
    {
        // TODO: Implement
        Debug.WriteLine($"Play sound {id}");
    }
}