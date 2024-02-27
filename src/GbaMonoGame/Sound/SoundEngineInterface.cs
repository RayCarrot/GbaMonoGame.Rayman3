using System;

namespace GbaMonoGame;

// Useful links for reverse engineering the GBA MP2k sound engine:
// https://loveemu.github.io/vgmdocs/Summary_of_GBA_Standard_Sound_Driver_MusicPlayer2000.html
// https://github.dev/pret/pokeruby/blob/master/include/gba/m4a_internal.h

// NOTE: This is only implemented on GBA
public static class SoundEngineInterface
{
    static SoundEngineInterface()
    {
        _midiParams = new MidiParams
        {
            ReverbValue = 0,
            NbOfVoices = 7,
            MasterVolume = MaxVolume,
            Freq = 0x70000,
        };
        SetMidiParams();
    }

    private static readonly MidiParams _midiParams;

    public const float MaxVolume = 128;

    public static void SetNbVoices(byte newNbOfVoices)
    {
        if (newNbOfVoices is < 1 or > 12)
            throw new Exception("Invalid number of voices. Must be a value between 1 and 12.");

        _midiParams.NbOfVoices = newNbOfVoices;

        SetMidiParams();
    }

    private static void SetMidiParams()
    {
        // This is only implemented on GBA and has it push the params into the sound engine
    }

    private class MidiParams
    {
        public byte ReverbValue { get; set; }
        public byte NbOfVoices { get; set; } // maxChans in MP2k
        public float MasterVolume { get; set; }
        public uint Freq { get; set; }
    }
}