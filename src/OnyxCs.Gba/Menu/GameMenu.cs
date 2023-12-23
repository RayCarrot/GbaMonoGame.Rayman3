using System;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba;

public class GameMenu
{
    #region Constructor

    public GameMenu()
    {
        MenuRenderer = new MenuRenderer();
    }

    #endregion

    #region Private Properties

    private MenuRenderer MenuRenderer { get; }
    private float TransitionValue { get; set; }

    #endregion

    #region Public Properties

    public bool IsTransitioningIn { get; private set; }
    public bool IsTransitioningOut { get; private set; }

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

    public void Open()
    {
        if (IsTransitioningIn || IsTransitioningOut)
            return;

        MenuRenderer.Init(Menu_Main);
        IsTransitioningOut = false;
        IsTransitioningIn = true;
        TransitionValue = 0;
    }

    public void Close()
    {
        if (IsTransitioningIn || IsTransitioningOut)
            return;

        MenuRenderer.UnInit();
        IsTransitioningOut = true;
        IsTransitioningIn = false;
        TransitionValue = 1;
    }

    public void Update()
    {
        MenuRenderer.Update();
    }

    public void Draw(GfxRenderer renderer)
    {
        if (IsTransitioningIn)
        {
            TransitionValue += 1 / 10f;

            if (TransitionValue >= 1)
            {
                TransitionValue = 1;
                IsTransitioningIn = false;
            }
        }
        else if (IsTransitioningOut)
        {
            TransitionValue -= 1 / 10f;

            if (TransitionValue <= 0)
            {
                TransitionValue = 0;
                IsTransitioningOut = false;
            }
        }

        renderer.BeginRender(new RenderOptions(false, Engine.ScreenCamera));

        // Fade out the game
        renderer.DrawFilledRectangle(Vector2.Zero, Engine.GameWindow.GameResolution, Color.Black * MathHelper.Lerp(0.0f, 0.7f, TransitionValue));

        // Draw menu
        MenuRenderer.Draw(renderer);
    }

    #endregion
}