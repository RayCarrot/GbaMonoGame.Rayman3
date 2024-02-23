using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BinarySerializer.Ubisoft.GbaEngine;
using ImGuiNET;

namespace GbaMonoGame.Rayman3;

public class GenerateDebugMenu : DebugMenu
{
    public override string Name => "Generate";

    private void WriteJson<T>(T obj, string filePath)
    {
        string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        });
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllText(filePath, json);
    }

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

    private void GenerateGameData()
    {
        string outputDir = "GameData";

        List<ActorModel>[] actorModels = new List<ActorModel>[256];

        for (int i = 0; i < GameInfo.Levels.Length; i++)
        {
            Scene2DResource scene = Storage.LoadResource<Scene2DResource>(i);

            foreach (Actor actor in scene.Actors.Concat(scene.AlwaysActors))
            {
                ActorModel model = actor.Model;

                actorModels[actor.Type] ??= new List<ActorModel>();

                if (actorModels[actor.Type].Contains(model))
                    continue;

                actorModels[actor.Type].Add(model);
                string outputFile = Path.Combine(outputDir, "Actors", $"{(ActorType)actor.Type}_{actorModels[actor.Type].Count}.json");
                WriteJson(new
                {
                    ViewBox = new Box(model.ViewBox),
                    DetectionBox = new Box(model.DetectionBox),
                    AnimatedObject = model.AnimatedObject.Offset.ToString(), // TODO: Replace with file name once that's set up
                    model.MapCollisionType,
                    model.CheckAgainstMapCollision,
                    model.CheckAgainstObjectCollision,
                    model.IsSolid,
                    model.IsAgainstCaptor,
                    model.ReceivesDamage,
                    model.HitPoints,
                    model.AttackPoints,
                    Actions = model.Actions.Select(x => new
                    {
                        Box = new Box(x.Box),
                        x.AnimationIndex,
                        x.Flags,
                        x.MechModelType,
                        MechModelParams = x.MechModel?.Params.Select(p => p.AsFloat)
                    })
                }, outputFile);
            }
        }

        // TODO: Generate remaining data (playfields, localization, story acts, animated objects etc.). Data with properties should
        //       be exported to .json and raw data, like graphics and maps, should be .dat files (with a .json for the header).
        //       Once done we can use this to load this as the game data, allowing easier edits. We can also compare data between
        //       prototypes, or even import data from prototypes into final version (like the snail actor).
    }

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        if (ImGui.MenuItem("Actors CSV"))
            GenerateActorsCsv();

        if (ImGui.MenuItem("Game data"))
            GenerateGameData();
    }
}