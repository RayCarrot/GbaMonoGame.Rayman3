namespace GbaMonoGame;

public class MidiParams
{
    public byte ReverbValue { get; set; }
    public byte NbOfVoices { get; set; } // maxChans in MP2k
    public float MasterVolume { get; set; }
    public uint Freq { get; set; }
}