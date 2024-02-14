using ImGuiNET;
using Action = BinarySerializer.Ubisoft.GbaEngine.Action;

namespace GbaMonoGame.Engine2d;

public class GameObjectDebugWindow : DebugWindow
{
    public override string Name => "Game Object";

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        GameObject selectedGameObject = debugLayout.GetWindow<SceneDebugWindow>()?.SelectedGameObject;

        if (selectedGameObject != null)
        {
            selectedGameObject.IsEnabled = ImGuiExt.Checkbox("Enabled", selectedGameObject.IsEnabled);

            System.Numerics.Vector2 pos = new(selectedGameObject.Position.X, selectedGameObject.Position.Y);
            if (ImGui.InputFloat2("Position", ref pos))
                selectedGameObject.Position = new Vector2(pos.X, pos.Y);

            selectedGameObject.DrawDebugLayout(debugLayout, textureManager);

            if (selectedGameObject is ActionActor actionActor)
            {
                ImGui.Spacing();
                ImGui.Spacing();
                ImGui.SeparatorText("Actions");

                if (ImGui.BeginTable("_actions", 5))
                {
                    ImGui.TableSetupColumn("Current", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("Id", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("Animation", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("Flags", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("MechModel");
                    ImGui.TableHeadersRow();

                    for (int actionId = 0; actionId < actionActor.Actions.Length; actionId++)
                    {
                        Action action = actionActor.Actions[actionId];
                        ImGui.TableNextRow();

                        ImGui.TableNextColumn();
                        bool isCurrent = actionActor.ActionId == actionId;
                        if (ImGui.RadioButton($"##{actionId}_enabled", isCurrent))
                            actionActor.ActionId = actionId;

                        ImGui.TableNextColumn();
                        ImGui.Text($"{actionId}");

                        ImGui.TableNextColumn();
                        ImGui.Text($"{action.AnimationIndex}");

                        ImGui.TableNextColumn();
                        ImGui.Text($"{action.Flags}");

                        ImGui.TableNextColumn();
                        ImGui.Text($"{action.MechModelType}");
                    }

                    ImGui.EndTable();
                }
            }
        }
        else
        {
            ImGui.Text("No object has been selected");
        }
    }
}