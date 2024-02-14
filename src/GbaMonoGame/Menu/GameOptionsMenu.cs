namespace GbaMonoGame;

public class GameOptionsMenu : Menu
{
    public override void Update(MenuManager menu)
    {
        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Center);

        // TODO: Implement

        if (menu.Button("Back"))
            menu.GoBack();
    }
}