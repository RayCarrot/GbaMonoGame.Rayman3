using System.Linq;
using System;
using ImGuiNET;

namespace OnyxCs.Gba;

public class GfxDebugWindow : DebugWindow
{
    private bool _cropAspectRatio;

    public override string Name => "Gfx";

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        ImGui.SeparatorText("General");

        float scale = Engine.ScreenCamera.Scale.X;
        if (ImGui.SliderFloat("Scale", ref scale, 0.5f, 2))
            Engine.ScreenCamera.Scale = new Vector2(scale);

        ImGui.SameLine();
        if (ImGui.Button("Reset"))
            Engine.ScreenCamera.Scale = Vector2.One;

        ImGui.Spacing();

        ImGui.Checkbox("Crop", ref _cropAspectRatio);

        if (ImGui.Button("GBA (3:2"))
            Engine.ScreenCamera.SetAspectRatio(3 / 2f, _cropAspectRatio);

        ImGui.SameLine();
        if (ImGui.Button("N-Gage (11:13)"))
            Engine.ScreenCamera.SetAspectRatio(11 / 13f, _cropAspectRatio);

        ImGui.SameLine();
        if (ImGui.Button("Widescreen (16:9)"))
            Engine.ScreenCamera.SetAspectRatio(16 / 9f, _cropAspectRatio);

        float ratio = Engine.ScreenCamera.GameResolution.X / Engine.ScreenCamera.GameResolution.Y;
        if (ImGui.InputFloat("Aspect ratio", ref ratio))
            Engine.ScreenCamera.SetAspectRatio(ratio, _cropAspectRatio);

        ImGui.Spacing();

        ImGui.Text($"Original resolution: {Engine.ScreenCamera.OriginalGameResolution.X} x {Engine.ScreenCamera.OriginalGameResolution.Y}");
        ImGui.Text($"Resolution: {Engine.ScreenCamera.GameResolution.X} x {Engine.ScreenCamera.GameResolution.Y}");
        ImGui.Text($"Scaled resolution: {Engine.ScreenCamera.ScaledGameResolution.X} x {Engine.ScreenCamera.ScaledGameResolution.Y}");

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
            ImGui.TableSetupColumn("Scaled");
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
                ImGui.Text($"{screen.IsScaled}");

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