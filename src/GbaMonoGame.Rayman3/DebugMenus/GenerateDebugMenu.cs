using System;
using System.IO;
using System.Linq;
using System.Text;
using BinarySerializer.Ubisoft.GbaEngine;
using ImGuiNET;

namespace GbaMonoGame.Rayman3;

public class GenerateDebugMenu : DebugMenu
{
    public override string Name => "Generate";

    private void GenerateActorsCsv()
    {
        ActorModel[] actorModels = new ActorModel[256];

        for (int i = 0; i < GameInfo.Levels.Length; i++)
        {
            Scene2DResource scene = Storage.LoadResource<Scene2DResource>(i);

            // NOTE: Some actors types have multiple models, but they all appear the same
            foreach (Actor actor in scene.Actors.Concat(scene.AlwaysActors))
            {
                ActorModel model = actor.Model;
                actorModels[actor.Type] = model;
            }
        }

        StringBuilder sb = new();

        void addValue(string value) => sb.Append($"{value},");

        // Header
        addValue("Type Id");
        addValue("Type Name");
        addValue("Hit Points");
        addValue("Attack Points");
        addValue("Receives damage");
        addValue("Map Collision");
        addValue("Object Collision");
        addValue("Is Solid");
        addValue("Is Against Captor");
        addValue("Actions");
        addValue("Animations");
        sb.AppendLine();

        for (int i = 0; i < actorModels.Length; i++)
        {
            ActorModel model = actorModels[i];

            if (model == null)
                continue;

            addValue($"{i}");
            addValue(Enum.IsDefined(typeof(ActorType), i) ? $"{(ActorType)i}" : "");
            addValue(model.HitPoints != 0 ? model.HitPoints.ToString() : "");
            addValue(model.AttackPoints != 0 ? model.AttackPoints.ToString() : "");
            addValue(model.ReceivesDamage ? "✔️" : "");
            addValue(model.CheckAgainstMapCollision ? $"{model.MapCollisionType}" : "");
            addValue(model.CheckAgainstObjectCollision ? "✔️" : "");
            addValue(model.IsSolid ? "✔️" : "");
            addValue(model.IsAgainstCaptor ? "✔️" : "");
            addValue($"{model.Actions.Length}");
            addValue($"{model.AnimatedObject.Animations.Length}");
            sb.AppendLine();
        }

        File.WriteAllText("actors.csv", sb.ToString());
    }

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        if (ImGui.MenuItem("Actors CSV"))
            GenerateActorsCsv();
    }
}