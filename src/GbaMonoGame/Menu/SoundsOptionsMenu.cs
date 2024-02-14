using BinarySerializer.Ubisoft.GbaEngine;

namespace GbaMonoGame;

public class SoundsOptionsMenu : Menu
{
    public SoundsOptionsMenu()
    {
        AvailableVolumes = new string[11];
        for (int i = 0; i < AvailableVolumes.Length; i++)
            AvailableVolumes[i] = i.ToString();
    }

    private string[] AvailableVolumes { get; }

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

        menu.Text("Music volume");
        Engine.Config.MusicVolume = menu.Selection(AvailableVolumes, (int)(Engine.Config.MusicVolume * 10)) / 10f;

        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        if (menu.Button("Back"))
            menu.GoBack();
    }
}