using System;
using System.Linq;
using Microsoft.Xna.Framework.Audio;

namespace GbaMonoGame;

public class VolumeSelectionMenuOption : MultiSelectionMenuOption<float>
{
    public VolumeSelectionMenuOption(string name, string sampleSongName, bool restart, Func<float> getVolume, Action<float> setVolume) 
        : base(name, CreateItems(), CreateGetData(getVolume), CreateSetData(setVolume), CreateGetCustomName())
    {
        SampleSongName = sampleSongName;
        Restart = restart;
        GetVolume = getVolume;
    }

    private const int MaxStep = 10;

    private string SampleSongName { get; }
    private bool Restart { get; }
    private Func<float> GetVolume { get; }

    private SoundEffectInstance ActiveSampleSong { get; set; }

    private static Item[] CreateItems() => Enumerable.Range(0, MaxStep + 1).Select(x => new Item(x.ToString(), x)).ToArray();
    private static Func<Item[], float> CreateGetData(Func<float> getVolume) => _ => getVolume() * MaxStep;
    private static Action<float> CreateSetData(Action<float> setVolume) => data => setVolume(data / MaxStep);
    private static Func<float, string> CreateGetCustomName() => data => $"{data:0.00}";

    private void PlaySampleSong()
    {
        if (!Restart && ActiveSampleSong != null)
        {
            ActiveSampleSong.Volume = GetVolume();
        }
        else
        {
            StopSampleSong();
            ActiveSampleSong = SoundEventsManager.GetSoundByName(SampleSongName).CreateInstance();
            ActiveSampleSong.Volume = GetVolume();
            ActiveSampleSong.Play();
        }
    }

    private void StopSampleSong()
    {
        ActiveSampleSong?.Dispose();
        ActiveSampleSong = null;
    }

    public override void OnExit()
    {
        base.OnExit();
        StopSampleSong();
    }

    public override void Update(MenuManager menu)
    {
        int prevSelectedIndex = SelectedIndex;

        base.Update(menu);

        // Stop if not selected
        if (!menu.IsElementSelected())
            StopSampleSong();
        // Start playing if modified
        else if (prevSelectedIndex != SelectedIndex)
            PlaySampleSong();
    }
}