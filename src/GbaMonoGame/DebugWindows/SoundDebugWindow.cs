using ImGuiNET;

namespace GbaMonoGame;

public class SoundDebugWindow : DebugWindow
{
    public override string Name => "Sound";

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    { 
        ImGui.SeparatorText("Songs");

        if (ImGui.Button("Stop all"))
            SoundEventsManager.StopAll();

        if (ImGui.BeginTable("_songs", 5))
        {
            ImGui.TableSetupColumn("Event", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("State", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Duration", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Next");
            ImGui.TableHeadersRow();

            foreach (SoundEventsManager.PlayingSong playingSong in SoundEventsManager._playingSongs)
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text($"{playingSong.EventId}");

                ImGui.TableNextColumn();
                ImGui.Text($"{playingSong.SoundEffect.Name}");

                ImGui.TableNextColumn();
                ImGui.Text($"{playingSong.SoundInstance.State}");

                ImGui.TableNextColumn();
                ImGui.Text($"{playingSong.SoundEffect.Duration.TotalSeconds:F}");

                ImGui.TableNextColumn();
                ImGui.Text($"{playingSong.NextSoundEventId}");
            }

            ImGui.EndTable();
        }
    }
}