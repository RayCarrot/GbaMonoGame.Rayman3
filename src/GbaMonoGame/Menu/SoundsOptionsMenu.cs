using BinarySerializer.Ubisoft.GbaEngine;

namespace GbaMonoGame;

public class SoundsOptionsMenu : Menu
{
    public SoundsOptionsMenu()
    {
        AvailableVolumes = new string[11];
        for (int i = 0; i < AvailableVolumes.Length; i++)
            AvailableVolumes[i] = i.ToString();

        PreviousSfxVolume = Engine.Config.SfxVolume;
    }

    private string[] AvailableVolumes { get; }

    private float PreviousSfxVolume { get; set; }

    public override void OnExit()
    {
        SoundEventsManager.StopSampleSongs();
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

        menu.Text("Sound effects volume");
        Engine.Config.SfxVolume = menu.Selection(AvailableVolumes, (int)(Engine.Config.SfxVolume * 10)) / 10f;
        if (Engine.Config.SfxVolume != PreviousSfxVolume)
        {
            PreviousSfxVolume = Engine.Config.SfxVolume;
            SoundEventsManager.PlaySampleSong(SoundType.Sfx, true, Engine.Config.SfxVolume);
        }

        menu.Text("Music volume");
        Engine.Config.MusicVolume = menu.Selection(AvailableVolumes, (int)(Engine.Config.MusicVolume * 10)) / 10f;
        if (menu.IsElementSelected())
            SoundEventsManager.PlaySampleSong(SoundType.Music, false, Engine.Config.MusicVolume);
        else
            SoundEventsManager.StopSampleSong(SoundType.Music);


        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        if (menu.Button("Back"))
            menu.GoBack();
    }
}