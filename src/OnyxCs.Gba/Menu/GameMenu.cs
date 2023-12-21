using System;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba;

public class GameMenu
{
    #region Constructor

    public GameMenu()
    {
        MenuRenderer = new MenuRenderer();
        MenuRenderer.ChangeMenu(Menu_Main);
    }

    #endregion

    #region Private Properties

    private MenuRenderer MenuRenderer { get; }

    #endregion

    #region Menus

    private static void Menu_Main(MenuRenderer menu)
    {
        if (menu.Button("General options"))
            menu.ChangeMenu(Menu_GeneralOptions);

        if (menu.Button("Display options"))
            menu.ChangeMenu(Menu_DisplayOptions);

        if (menu.Button("Control options"))
            menu.ChangeMenu(Menu_ControlOptions);

        if (menu.Button("Debug"))
            menu.ChangeMenu(Menu_Debug);

        if (menu.Button("Exit"))
            menu.ChangeMenu(Menu_Exit);
    }

    private static void Menu_GeneralOptions(MenuRenderer menu)
    {
        // TODO: Implement

        if (menu.Button("Back"))
            menu.ChangeMenu(Menu_Main);
    }

    private static void Menu_DisplayOptions(MenuRenderer menu)
    {
        // TODO: Implement

        if (menu.Button("Back"))
            menu.ChangeMenu(Menu_Main);
    }

    private static void Menu_ControlOptions(MenuRenderer menu)
    {
        // TODO: Implement

        if (menu.Button("Back"))
            menu.ChangeMenu(Menu_Main);
    }

    private static void Menu_Debug(MenuRenderer menu)
    {
        // TODO: Implement

        if (menu.Button("Back"))
            menu.ChangeMenu(Menu_Main);
    }

    private static void Menu_Exit(MenuRenderer menu)
    {
        menu.Text("Are you sure you want to exit the game? Any unsaved progress will be lost!");

        if (menu.Button("No"))
            menu.ChangeMenu(Menu_Main);

        if (menu.Button("Yes"))
            Environment.Exit(0);
    }

    #endregion

    #region Public Methods

    public void Update()
    {
        MenuRenderer.Update();
    }

    public void Draw(GfxRenderer renderer)
    {
        // Fade out the game
        renderer.DrawFilledRectangle(Vector2.Zero, Engine.GameWindow.GameResolution, Color.Black * 0.7f);

        // Draw menu
        MenuRenderer.Draw(renderer);
    }

    #endregion
}