using ImGuiNET;

namespace GbaMonoGame;

public class WindowsDebugMenu : DebugMenu
{
    public override string Name => "Windows";

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        foreach (DebugWindow window in debugLayout.GetWindows())
        {
            if (window.CanClose)
            {
                if (ImGui.MenuItem(window.Name))
                    window.IsOpen = true;
            }
        }
    }
}