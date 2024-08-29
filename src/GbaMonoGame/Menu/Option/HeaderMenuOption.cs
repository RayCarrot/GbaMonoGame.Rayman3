namespace GbaMonoGame;

public class HeaderMenuOption : MenuOption
{
    public HeaderMenuOption(string name) : base(name) { }

    public override void Update(MenuManager menu)
    {
        menu.SetColumns(1);
        menu.SetHorizontalAlignment(MenuManager.HorizontalAlignment.Left);

        menu.SmallSpacing();
        menu.Text(Name);
    }
}