using ImGuiNET;

namespace GbaMonoGame;

public static class ImGuiExt
{
    public static bool Checkbox(string label, bool isChecked)
    {
        ImGui.Checkbox(label, ref isChecked);
        return isChecked;
    }
}