using ImGuiNET;

namespace GbaMonoGame;

/// <summary>
/// Debug window which shows the logger output.
/// </summary>
public class LoggerDebugWindow : DebugWindow
{
    public override string Name => "Log Output";

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        ImGui.Text("TODO: Implement");

        // TODO: Implement. Different log providers and levels to toggle?
    }
}