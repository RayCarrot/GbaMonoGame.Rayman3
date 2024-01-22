namespace GbaMonoGame;

public class MainMenu : Menu
{
    public override void Update(MenuManager menu)
    {
        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        if (menu.Button("Resume"))
            menu.Close();

        if (menu.Button("General options"))
            menu.ChangeMenu(new GeneralOptionsMenu());

        if (menu.Button("Display options"))
            menu.ChangeMenu(new DisplayOptionsMenu());

        if (menu.Button("Control options"))
            menu.ChangeMenu(new ControlOptionsMenu());

        if (menu.Button("Debug"))
            menu.ChangeMenu(new DebugOptionsMenu());

        if (menu.Button("Quit game"))
            menu.ChangeMenu(new QuitGameMenu());
    }
}