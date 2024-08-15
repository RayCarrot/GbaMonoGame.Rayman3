using System.Collections.Generic;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework.Audio;

namespace GbaMonoGame;

public class SoundsOptionsMenu : Menu
{
    public SoundsOptionsMenu(Dictionary<SoundType, string> sampleSongs)
    {
        SampleSongs = sampleSongs;

        AvailableVolumes = new string[11];
        for (int i = 0; i < AvailableVolumes.Length; i++)
            AvailableVolumes[i] = i.ToString();

        PreviousSfxVolume = Engine.Config.SfxVolume;
    }

    private Dictionary<SoundType, string> SampleSongs { get; }
    private SoundEffectInstance ActiveSampleSong { get; set; }
    private SoundType? ActiveSampleSongType { get; set; }

    private string[] AvailableVolumes { get; }

    private float PreviousSfxVolume { get; set; }

    private void PlaySampleSong(SoundType type, bool restart, float volume)
    {
        if (!restart && ActiveSampleSongType == type)
        {
            ActiveSampleSong.Volume = volume;
        }
        else
        {
            StopSampleSongs();
            ActiveSampleSong = SoundEventsManager.GetSoundByName(SampleSongs[type]).CreateInstance();
            ActiveSampleSong.Volume = volume;
            ActiveSampleSong.Play();
            ActiveSampleSongType = type;
        }
    }

    private void StopSampleSongs()
    {
        if (ActiveSampleSong != null)
        {
            ActiveSampleSong.Dispose();
            ActiveSampleSong = null;
            ActiveSampleSongType = null;
        }
    }

    private void StopSampleSong(SoundType type)
    {
        if (ActiveSampleSong != null && ActiveSampleSongType == type)
        {
            ActiveSampleSong.Dispose();
            ActiveSampleSong = null;
            ActiveSampleSongType = null;
        }
    }

    public override void OnExit()
    {
        StopSampleSongs();
    }

    public override void Update(MenuManager menu)
    {
        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        menu.Text("Sound options");
        menu.Spacing();

        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        menu.SetColumns(1, 0.9f);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Left);

        menu.Text("Music volume");
        Engine.Config.MusicVolume = menu.Selection(AvailableVolumes, (int)(Engine.Config.MusicVolume * 10)) / 10f;
        if (menu.IsElementSelected())
            PlaySampleSong(SoundType.Music, false, Engine.Config.MusicVolume);
        else
            StopSampleSong(SoundType.Music);

        menu.Text("Sound effects volume");
        Engine.Config.SfxVolume = menu.Selection(AvailableVolumes, (int)(Engine.Config.SfxVolume * 10)) / 10f;
        if (Engine.Config.SfxVolume != PreviousSfxVolume)
        {
            PreviousSfxVolume = Engine.Config.SfxVolume;
            PlaySampleSong(SoundType.Sfx, true, Engine.Config.SfxVolume);
        }

        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        if (menu.Button("Back"))
            menu.GoBack();
    }
}