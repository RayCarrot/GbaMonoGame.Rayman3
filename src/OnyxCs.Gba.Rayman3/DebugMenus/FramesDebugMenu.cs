using System;
using ImGuiNET;

namespace OnyxCs.Gba.Rayman3;

public class FramesDebugMenu : DebugMenu
{
    private FrameFactory[] FrameFactories { get; } =
    {
        new("Intro", () => new Intro()),
        new("Menu", () => new MenuAll(MenuAll.Page.SelectLanguage)),
        new("Act #1", () => new Act1()),
        new("Act #2", () => new Act2()),
        new("Act #3", () => new Act3()),
        new("Act #4", () => new Act4()),
        new("Act #5", () => new Act5()),
        new("Act #6", () => new Act6()),
    };

    public override string Name => "Frames";

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        foreach (FrameFactory frameFactory in FrameFactories)
        {
            if (ImGui.MenuItem(frameFactory.Name))
            {
                FrameManager.SetNextFrame(frameFactory.CreateFrame());
            }
        }

        if (ImGui.BeginMenu("Levels"))
        {
            for (int i = 0; i < GameInfo.Levels.Length; i++)
            {
                if (ImGui.MenuItem(((MapId)i).ToString()))
                    FrameManager.SetNextFrame(LevelFactory.Create((MapId)i));
            }

            ImGui.EndMenu();
        }
    }

    private record FrameFactory(string Name, Func<Frame> CreateFrame);
}