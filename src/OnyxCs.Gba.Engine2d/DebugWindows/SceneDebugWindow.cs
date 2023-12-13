using System;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Engine2d;

public class SceneDebugWindow : DebugWindow
{
    private bool _showViewBoxes;
    private bool _showActionBoxes;
    private bool _showDetectionBoxes;
    private bool _showAttackBoxes;
    private bool _showVulnerabilityBoxes;
    private bool _showCaptorBoxes;

    private bool _fillBoxes;

    public override string Name => "Scene";
    public GameObject HighlightedGameObject { get; set; }
    public GameObject SelectedGameObject { get; set; }

    private void DrawBox(GfxRenderer renderer, TgxPlayfield2D playfield, Box box, Color color)
    {
        if (box == Box.Empty)
            return;

        box = box.Offset(-playfield.Camera.Position);

        if (_fillBoxes)
            renderer.DrawFilledRectangle(box.ToRectangle(), new Color(color, 0.1f));
        else
            renderer.DrawRectangle(box.ToRectangle(), color);
    }

    private Box GetObjBox(GameObject obj)
    {
        if (obj is BaseActor actor)
            return actor.GetViewBox();
        else if (obj is Captor captor)
            return captor.GetCaptorBox();
        else
            throw new Exception("Unsupported object type");
    }

    private void UpdateMouseDetection(Scene2D scene)
    {
        Vector2 mousePos = JoyPad.GetMousePosition();

        if (!JoyPad.IsMouseOnScreen())
            return;
        
        HighlightedGameObject = null;

        foreach (GameObject obj in scene.GameObjects.EnumerateAllGameObjects(true))
        {
            Box box = GetObjBox(obj).Offset(-scene.Playfield.Camera.Position);

            if (box.Contains(mousePos))
            {
                HighlightedGameObject = obj;
                break;
            }
        }

        if (JoyPad.GetMouseState().LeftButton == ButtonState.Pressed)
        {
            SelectedGameObject = HighlightedGameObject;
        }
    }

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        if (Frame.Current is not IHasScene { Scene: { } scene2D }) 
            return;

        UpdateMouseDetection(scene2D);

        if (ImGui.Button("Deselect object"))
            SelectedGameObject = null;

        ImGui.SeparatorText("Always actors");

        ImGui.Text($"Count: {scene2D.GameObjects.AlwaysActorsCount}");

        if (ImGui.BeginListBox("##_alwaysActors"))
        {
            foreach (BaseActor actor in scene2D.GameObjects.Objects.
                         Take(scene2D.GameObjects.AlwaysActorsCount).
                         Cast<BaseActor>())
            {
                bool isSelected = SelectedGameObject == actor;
                if (ImGui.Selectable($"{actor.Id}. {ObjectFactory.GetActorTypeName(actor.Type)}", isSelected))
                    SelectedGameObject = actor;
            }

            ImGui.EndListBox();
        }

        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.SeparatorText("Actors");

        ImGui.Text($"Count: {scene2D.GameObjects.ActorsCount}");

        if (ImGui.BeginListBox("##_actors"))
        {
            foreach (BaseActor actor in scene2D.GameObjects.Objects.
                         Skip(scene2D.GameObjects.AlwaysActorsCount).
                         Take(scene2D.GameObjects.ActorsCount).
                         Cast<BaseActor>())
            {
                bool isSelected = SelectedGameObject == actor;
                if (ImGui.Selectable($"{actor.Id}. {ObjectFactory.GetActorTypeName(actor.Type)}", isSelected))
                    SelectedGameObject = actor;
            }

            ImGui.EndListBox();
        }

        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.SeparatorText("Captors");

        ImGui.Text($"Count: {scene2D.GameObjects.CaptorsCount}");

        if (scene2D.GameObjects.CaptorsCount > 0 && ImGui.BeginListBox("##_captors"))
        {
            foreach (Captor captor in scene2D.GameObjects.Objects.
                         Skip(scene2D.GameObjects.AlwaysActorsCount + scene2D.GameObjects.ActorsCount).
                         Take(scene2D.GameObjects.CaptorsCount).
                         Cast<Captor>())
            {
                bool isSelected = SelectedGameObject == captor;
                if (ImGui.Selectable($"{captor.Id}. Captor", isSelected))
                    SelectedGameObject = captor;
            }

            ImGui.EndListBox();
        }

        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.SeparatorText("Knots");

        ImGui.Text("Count: 0");
        ImGui.Text("TODO: Implement");

        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.SeparatorText("Boxes");

        ImGui.Checkbox("Fill", ref _fillBoxes);
        ImGui.Spacing();

        ImGui.Checkbox("Show view boxes", ref _showViewBoxes);
        ImGui.Checkbox("Show action boxes", ref _showActionBoxes);
        ImGui.Checkbox("Show detection boxes", ref _showDetectionBoxes);
        ImGui.Checkbox("Show attack boxes", ref _showAttackBoxes);
        ImGui.Checkbox("Show vulnerability boxes", ref _showVulnerabilityBoxes);
        ImGui.Checkbox("Show captor boxes", ref _showCaptorBoxes);

        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.SeparatorText("Camera");

        scene2D.Camera.DrawDebugLayout(debugLayout, textureManager);
    }

    public override void DrawGame(GfxRenderer renderer)
    {
        if (Frame.Current is not IHasScene { Scene: { } scene2D }) 
            return;

        foreach (BaseActor actor in scene2D.GameObjects.EnumerateAllActors(isEnabled: true))
        {
            if (_showViewBoxes)
                DrawBox(renderer, scene2D.Playfield, actor.GetViewBox(), Color.Lime);

            if (actor is ActionActor actionActor)
            {
                if (_showActionBoxes)
                    DrawBox(renderer, scene2D.Playfield, actionActor.GetActionBox(), Color.Blue);

                if (_showDetectionBoxes)
                    DrawBox(renderer, scene2D.Playfield, actionActor.GetDetectionBox(), Color.DeepPink);
            }

            if (actor is InteractableActor interactableActor)
            {
                if (_showAttackBoxes)
                    DrawBox(renderer, scene2D.Playfield, interactableActor.GetAttackBox(), Color.OrangeRed);

                if (_showVulnerabilityBoxes)
                    DrawBox(renderer, scene2D.Playfield, interactableActor.GetVulnerabilityBox(), Color.Green);
            }
        }

        if (_showCaptorBoxes)
        {
            foreach (Captor captor in scene2D.GameObjects.EnumerateCaptors(isEnabled: true))
            {
                DrawBox(renderer, scene2D.Playfield, captor.GetCaptorBox(), Color.DeepPink);
            }
        }

        if (HighlightedGameObject != null)
            DrawBox(renderer, scene2D.Playfield, GetObjBox(HighlightedGameObject), Color.Orange);

        if (SelectedGameObject != null)
            DrawBox(renderer, scene2D.Playfield, GetObjBox(SelectedGameObject), Color.Red);
    }
}