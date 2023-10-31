using System.Collections.Generic;
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Engine2d;

public class Scene2D
{
    public Scene2D(int id, CameraActor camera, int layersCount)
    {
        Frame.RegisterComponent(this);

        Camera = camera;
        LayersCount = layersCount;

        AnimationPlayer = new AnimationPlayer(false);
        Dialogs = new List<Dialog>(layersCount);

        Scene2DResource scene = Storage.LoadResource<Scene2DResource>(id);
        Playfield = TgxPlayfield.Load<TgxPlayfield2D>(scene.Playfield);

        Objects = new GameObjects(scene);

        Camera.LinkedObject = GetMainActor();
        Camera.SetFirstPosition();
    }

    public CameraActor Camera { get; }
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
            dialog.Fsm.Step();
            dialog.Draw(AnimationPlayer);
        }
    }

    public void RunActors()
    {
        foreach (BaseActor actor in Objects.EnumerateEnabledAlwaysActors())
        {
            actor.DoBehavior();
        }

        foreach (BaseActor actor in Objects.EnumerateEnabledActors())
        {
            actor.DoBehavior();
        }
    }

    public void StepActors()
    {
        foreach (BaseActor actor in Objects.EnumerateEnabledAlwaysActors())
        {
            actor.Step();
        }

        foreach (BaseActor actor in Objects.EnumerateEnabledActors())
        {
            actor.Step();
        }
    }

    public void MoveActors()
    {
        foreach (BaseActor actor in Objects.EnumerateEnabledAlwaysActors())
        {
            if (actor is MovableActor movableActor)
                movableActor.Move();
        }

        foreach (BaseActor actor in Objects.EnumerateEnabledActors())
        {
            if (actor is MovableActor movableActor)
                movableActor.Move();
        }
    }

    public void DrawActors()
    {
        foreach (BaseActor actor in Objects.EnumerateEnabledAlwaysActors())
        {
            actor.Draw(AnimationPlayer, false);
        }

        foreach (BaseActor actor in Objects.EnumerateEnabledActors())
        {
            actor.Draw(AnimationPlayer, false);
        }
    }

    public void RunCamera()
    {
        Camera.Fsm.Step();
    }

    public MovableActor GetMainActor()
    {
        return (MovableActor)Objects.Objects[0];
    }

    public bool IsHitMainActor(InteractableActor actor)
    {
        // TODO: Implement

        return false;
    }

    public void DamageMainActor(int damage)
    {
        MovableActor mainActor = GetMainActor();

        if (damage < mainActor.HitPoints)
            mainActor.HitPoints -= damage;
        else
            mainActor.HitPoints = 0;
    }

    public byte GetPhysicalType(Vector2 position)
    {
        return Playfield.GetPhysicalValue((position / 8).ToPoint());
    }
}