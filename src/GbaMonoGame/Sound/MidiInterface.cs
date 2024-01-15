using System;

namespace GbaMonoGame;

// NOTE: This is only implemented on GBA
public static class MidiInterface
{
    static MidiInterface()
    {
        _midiParams = new MidiParams
        {
            ReverbValue = 0,
            NbOfVoices = 7,
            MasterVolume = MaxVolume,
            Uint_04 = 0x70000,
        };
        SetMidiParams();
    }

    private static readonly MidiParams _midiParams;

    public const int MaxVolume = 128;

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
        public byte NbOfVoices { get; set; }
        public byte MasterVolume { get; set; }
        public uint Uint_04 { get; set; }
    }
}