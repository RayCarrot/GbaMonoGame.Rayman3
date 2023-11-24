using System;
using ImGuiNET;

namespace OnyxCs.Gba.Rayman3;

public class FramesDebugMenu : DebugMenu
{
    private FrameFactory[] FrameFactories { get; } =
    {
        new("Intro", () => new Intro()),
        new("Menu", () => new MenuAll(MenuAll.Page.SelectLanguage)),
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