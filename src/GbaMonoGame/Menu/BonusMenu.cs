namespace GbaMonoGame;

public class BonusMenu : Menu
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