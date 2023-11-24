using System.Linq;
using System;
using ImGuiNET;

namespace OnyxCs.Gba;

public class GfxDebugWindow : DebugWindow
{
    public override string Name => "Gfx";

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        ImGui.SeparatorText("Screens");

        if (ImGui.BeginTable("_screens", 7))
        {
            ImGui.TableSetupColumn("Enabled", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Wrap", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Id", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Priority", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Offset");
            ImGui.TableSetupColumn("Size");
            ImGui.TableSetupColumn("Color mode");
            ImGui.TableHeadersRow();

            foreach (GfxScreen screen in Gfx.GetScreens().OrderBy(x => x.Id))
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                bool enabled = screen.IsEnabled;
                ImGui.Checkbox($"##{screen.Id}_enabled", ref enabled);
                screen.IsEnabled = enabled;

                ImGui.TableNextColumn();
                bool wrap = screen.Wrap;
                ImGui.Checkbox($"##{screen.Id}_wrap", ref wrap);
                screen.Wrap = wrap;

                ImGui.TableNextColumn();
                ImGui.Text($"{screen.Id}");

                ImGui.TableNextColumn();
                ImGui.Text($"{screen.Priority}");

                ImGui.TableNextColumn();
                ImGui.Text($"{screen.Offset.X:0.00} x {screen.Offset.Y:0.00}");

                ImGui.TableNextColumn();
                ImGui.Text($"{screen.Renderer?.Size.X:0.00} x {screen.Renderer?.Size.Y:0.00}");

                ImGui.TableNextColumn();
                ImGui.Text(screen.Is8Bit switch
                {
                    null => String.Empty,
                    true => "8-bit",
                    false => "4-bit",
                });
            }
            ImGui.EndTable();
        }

        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.SeparatorText("Sprites");

        ImGui.Text("TODO: Implement");
    }
}