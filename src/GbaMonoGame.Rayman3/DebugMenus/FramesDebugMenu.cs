using System;
using System.Linq;
using ImGuiNET;

namespace GbaMonoGame.Rayman3;

public class FramesDebugMenu : DebugMenu
{
    private FrameMenuItem[] Menu { get; } =
    {
        new("Intro", () => new Intro()),
        new("Menu", () => new MenuAll(MenuAll.Page.SelectLanguage)),
        new("Game Over", () => new GameOver()),
        new("Story", null, new FrameMenuItem[]
        {
            new("NGage Splash Screens", () => new NGageSplashScreensAct()),
            new("Act #1", () => new Act1()),
            new("Act #2", () => new Act2()),
            new("Act #3", () => new Act3()),
            new("Act #4", () => new Act4()),
            new("Act #5", () => new Act5()),
            new("Act #6", () => new Act6()),
        }),
        new("Levels", null, 
            GameInfo.Levels.
            Select((_, i) => new FrameMenuItem(((MapId)i).ToString(), () => LevelFactory.Create((MapId)i))).
            ToArray()),
    };

    public override string Name => "Frames";

    private void DrawMenu(FrameMenuItem[] items)
    {
        foreach (FrameMenuItem menuItem in items)
        {
            if (menuItem.SubMenu != null)
            {
                if (ImGui.BeginMenu(menuItem.Name))
                {
                    DrawMenu(menuItem.SubMenu);
                    ImGui.EndMenu();
                }
            }
            else if (ImGui.MenuItem(menuItem.Name))
            {
                FrameManager.SetNextFrame(menuItem.CreateFrame());
            }
        }
    }

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        DrawMenu(Menu);
    }

    private record FrameMenuItem(string Name, Func<Frame> CreateFrame, FrameMenuItem[] SubMenu = null);
}