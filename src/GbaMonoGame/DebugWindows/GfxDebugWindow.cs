using System.Linq;
using System;
using ImGuiNET;

namespace GbaMonoGame;

public class GfxDebugWindow : DebugWindow
{
    private bool _cropAspectRatio;

    public override string Name => "Gfx";

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        ImGui.SeparatorText("General");

        float scale = Engine.Config.Scale;
        if (ImGui.SliderFloat("Scale", ref scale, 0.5f, 2))
        {
            Engine.Config.Scale = scale;
            Engine.Config.OnConfigChanged();
        }

        ImGui.SameLine();
        if (ImGui.Button("Reset"))
        {
            Engine.Config.Scale = 1;
            Engine.Config.OnConfigChanged();
        }

        ImGui.Spacing();

        ImGui.Checkbox("Crop", ref _cropAspectRatio);

        if (ImGui.Button("GBA (3:2"))
            Engine.GameWindow.SetAspectRatio(3 / 2f, _cropAspectRatio);

        ImGui.SameLine();
        if (ImGui.Button("N-Gage (11:13)"))
            Engine.GameWindow.SetAspectRatio(11 / 13f, _cropAspectRatio);

        ImGui.SameLine();
        if (ImGui.Button("Widescreen (16:9)"))
            Engine.GameWindow.SetAspectRatio(16 / 9f, _cropAspectRatio);

        float ratio = Engine.GameWindow.AspectRatio;
        if (ImGui.InputFloat("Aspect ratio", ref ratio) && ratio > 0)
            Engine.GameWindow.SetAspectRatio(ratio, _cropAspectRatio);

        ImGui.Spacing();

        ImGui.Text($"Original resolution: {Engine.GameWindow.OriginalGameResolution.X} x {Engine.GameWindow.OriginalGameResolution.Y}");
        ImGui.Text($"Resolution: {Engine.GameWindow.GameResolution.X} x {Engine.GameWindow.GameResolution.Y}");

        ImGui.Spacing();

        float fade = Gfx.Fade;
        ImGui.SliderFloat("Fade", ref fade, 0, 1);
        Gfx.Fade = fade;

        ImGui.SeparatorText("Screens");

        if (ImGui.BeginTable("_screens", 8))
        {
            ImGui.TableSetupColumn("Enabled", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Wrap", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Id", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Priority", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Offset");
            ImGui.TableSetupColumn("Size");
            ImGui.TableSetupColumn("Camera");
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
                ImGui.Text($"{screen.Renderer?.GetSize(screen).X:0.00} x {screen.Renderer?.GetSize(screen).Y:0.00}");

                ImGui.TableNextColumn();
                ImGui.Text($"{screen.Camera.GetType().Name}");

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