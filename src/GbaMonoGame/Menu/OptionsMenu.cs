namespace GbaMonoGame;

public class OptionsMenu : Menu
{
    public OptionsMenu(GbaGame game)
    {
        Game = game;
    }

    private GbaGame Game { get; }

    public override void Update(MenuManager menu)
    {
        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        menu.Text("Options");
        menu.Spacing();

        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        if (menu.Button("Game options"))
            menu.ChangeMenu(new GameOptionsMenu());

        if (menu.Button("Sound options"))
            menu.ChangeMenu(new SoundsOptionsMenu());

        if (menu.Button("Display options"))
            menu.ChangeMenu(new DisplayOptionsMenu(Game));

        if (menu.Button("Control options"))
            menu.ChangeMenu(new ControlOptionsMenu());

        if (menu.Button("Debug options"))
            menu.ChangeMenu(new DebugOptionsMenu());
    }
}