using System;
using ImGuiNET;

namespace GbaMonoGame;

public class MultiplayerDebugWindow : DebugWindow
{
    public override string Name => "Multiplayer";

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        ImGui.SeparatorText("General");

        RSMultiplayer.IsActive = ImGuiExt.Checkbox("IsActive", RSMultiplayer.IsActive);

        ImGui.BeginDisabled(!RSMultiplayer.IsActive);

        if (ImGui.BeginCombo("State", RSMultiplayer.MubState.ToString()))
        {
            foreach (MubState state in Enum.GetValues<MubState>())
            {
                if (ImGui.Selectable(state.ToString(), RSMultiplayer.MubState == state))
                    RSMultiplayer.MubState = state;
            }

            ImGui.EndCombo();
        }

        int playersCount = RSMultiplayer.PlayersCount;
        if (ImGui.SliderInt("PlayersCount", ref playersCount, 0, RSMultiplayer.MaxPlayersCount))
        {
            RSMultiplayer.PlayersCount = playersCount;
            MultiplayerManager.PlayersCount = playersCount;
        }

        int machineId = RSMultiplayer.MachineId;
        if (ImGui.SliderInt("MachineId", ref machineId, 0, Math.Max(RSMultiplayer.PlayersCount - 1, 0)))
        {
            RSMultiplayer.MachineId = machineId;
            MultiplayerManager.MachineId = machineId;
        }

        ImGui.EndDisabled();
    }
}