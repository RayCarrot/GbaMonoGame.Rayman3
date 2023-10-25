﻿using System.Collections.Generic;
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Engine2d;

public class Scene2D
{
    public Scene2D(int id, int layersCount)
    {
        LayersCount = layersCount;

        AnimationPlayer = new AnimationPlayer(false);
        Dialogs = new List<Dialog>(layersCount);

        Scene2DResource scene = Storage.LoadResource<Scene2DResource>(id);
        Playfield = TgxPlayfield.Load<TgxPlayfield2D>(scene.Playfield);

        Objects = new GameObjects(scene);
    }

    public List<Dialog> Dialogs { get; }
    public AnimationPlayer AnimationPlayer { get; }
    public TgxPlayfield2D Playfield { get; }
    public int LayersCount { get; }
    public GameObjects Objects { get; }

    public void AddDialog(Dialog dialog)
    {
        Dialogs.Add(dialog);
        dialog.Load();
        dialog.Init();
    }

    public void ProcessDialogs()
    {
        foreach (Dialog dialog in Dialogs)
        {
            dialog.Fsm();
            dialog.Draw(AnimationPlayer);
        }
    }

    public void RunActors()
    {
        foreach (BaseActor actor in Objects.EnumerateAlwaysActors())
        {
            if (actor.IsEnabled)
                actor.DoBehavior();
        }

        foreach (BaseActor actor in Objects.EnumerateActors())
        {
            if (actor.IsEnabled)
                actor.DoBehavior();
        }
    }

    public void StepActors()
    {
        foreach (BaseActor actor in Objects.EnumerateAlwaysActors())
        {
            if (actor.IsEnabled)
                actor.Step();
        }

        foreach (BaseActor actor in Objects.EnumerateActors())
        {
            if (actor.IsEnabled)
                actor.Step();
        }
    }

    public void DrawActors()
    {
        foreach (BaseActor actor in Objects.EnumerateAlwaysActors())
        {
            if (actor.IsEnabled)
                actor.Draw(AnimationPlayer, false);
        }

        foreach (BaseActor actor in Objects.EnumerateActors())
        {
            if (actor.IsEnabled)
                actor.Draw(AnimationPlayer, false);
        }
    }
}