namespace GbaMonoGame;

public class MainMenu : Menu
{
    public MainMenu(GbaGame game)
    {
        Game = game;
    }

    private GbaGame Game { get; }

    public override void Update(MenuManager menu)
    {
        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        menu.Text("Paused");
        menu.Spacing();

        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        if (menu.Button("Resume"))
            menu.Close();

        if (Game.CanSkipCutscene)
        {
            if (menu.Button("Skip cutscene"))
            {
                Game.SkipCutscene();
                menu.Close();
            }
        }

        if (menu.Button("Options"))
            menu.ChangeMenu(new OptionsMenu(Game));

        if (menu.Button("Bonus"))
            menu.ChangeMenu(new BonusMenu());

        if (menu.Button("Quit game"))
            menu.ChangeMenu(new QuitGameMenu(Game));
    }
}