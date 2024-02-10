using ImGuiNET;

namespace GbaMonoGame;

public class SoundDebugWindow : DebugWindow
{
    public override string Name => "Sound";

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    { 
        ImGui.SeparatorText("Songs");

        if (ImGui.Button("Stop all"))
            SoundEventsManager.StopAllSongs();

        SoundEventsManager.DrawDebugLayout();
    }
}