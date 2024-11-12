﻿using System;
using ImGuiNET;
using Action = BinarySerializer.Ubisoft.GbaEngine.Action;

namespace GbaMonoGame.Engine2d;

public class GameObjectDebugWindow : DebugWindow
{
    private readonly MechModel _dummyMechModel = new();

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

                if (ImGui.BeginTable("_actions", 7))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("Id", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("Anim", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("Flags", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("Speed");
                    ImGui.TableSetupColumn("Accel");
                    ImGui.TableSetupColumn("Target");
                    ImGui.TableHeadersRow();

                    // Attempt to get enum for actions
                    Type actionType = selectedGameObject.GetType().GetNestedType("Action");
                    if (actionType is { IsEnum: false })
                        actionType = null;

                    for (int actionId = 0; actionId < actionActor.Actions.Length; actionId++)
                    {
                        Action action = actionActor.Actions[actionId];
                        ImGui.TableNextRow();

                        ImGui.TableNextColumn();
                        bool isCurrent = actionActor.ActionId == actionId;
                        if (ImGui.RadioButton($"##{actionId}_enabled", isCurrent))
                            actionActor.ActionId = actionId;

                        ImGui.TableNextColumn();
                        if (actionType != null)
                            ImGui.Text($"{actionId} - {Enum.GetName(actionType, actionId)}");
                        else
                            ImGui.Text($"{actionId}");

                        ImGui.TableNextColumn();
                        ImGui.Text($"{action.AnimationIndex}");

                        ImGui.TableNextColumn();
                        ImGui.Text($"{action.Flags}");

                        if (action.MechModelType != null)
                        {
                            // Default to NaN so we can tell if a value was changed or not
                            _dummyMechModel.Speed = new Vector2(Single.NaN);
                            _dummyMechModel.Acceleration = new Vector2(Single.NaN);
                            _dummyMechModel.TargetSpeed = new Vector2(Single.NaN);

                            // Init from the action
                            _dummyMechModel.Init(action.MechModelType.Value, action.MechModel?.Params);

                            ImGui.TableNextColumn();
                            ImGui.Text($"{formatValue(_dummyMechModel.Speed.X)} x {formatValue(_dummyMechModel.Speed.Y)}");

                            ImGui.TableNextColumn();
                            ImGui.Text($"{formatValue(_dummyMechModel.Acceleration.X)} x {formatValue(_dummyMechModel.Acceleration.Y)}");

                            ImGui.TableNextColumn();
                            ImGui.Text($"{formatValue(_dummyMechModel.TargetSpeed.X)} x {formatValue(_dummyMechModel.TargetSpeed.Y)}");

                            string formatValue(float value)
                            {
                                // Unchanged
                                if (Single.IsNaN(value))
                                    return "_";
                                // 0
                                else if (value == 0)
                                    return "0";
                                // Value, limit to two decimals
                                else
                                    return $"{value:##}";
                            }
                        }
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