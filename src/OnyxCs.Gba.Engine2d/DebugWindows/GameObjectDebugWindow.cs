using ImGuiNET;

namespace OnyxCs.Gba.Engine2d;

public class GameObjectDebugWindow : DebugWindow
{
    public override string Name => "Game Object";

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        GameObject selectedGameObject = debugLayout.GetWindow<SceneDebugWindow>()?.SelectedGameObject;

        if (selectedGameObject != null)
        {
            bool enabled = selectedGameObject.IsEnabled;
            ImGui.Checkbox("Enabled", ref enabled);
            selectedGameObject.IsEnabled = enabled;

            System.Numerics.Vector2 pos = new(selectedGameObject.Position.X, selectedGameObject.Position.Y);
            if (ImGui.InputFloat2("Position", ref pos))
                selectedGameObject.Position = new Vector2(pos.X, pos.Y);

            selectedGameObject.DrawDebugLayout(debugLayout, textureManager);
        }
        else
        {
            ImGui.Text("No object has been selected");
        }
    }
}