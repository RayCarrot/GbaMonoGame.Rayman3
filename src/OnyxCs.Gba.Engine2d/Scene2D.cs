using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Nintendo.GBA;
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

        GameObjects = new GameObjects(scene);

        Camera.LinkedObject = GetMainActor();
        Camera.SetFirstPosition();
    }

    public CameraActor Camera { get; }
    public List<Dialog> Dialogs { get; }
    public AnimationPlayer AnimationPlayer { get; }
    public TgxPlayfield2D Playfield { get; }
    public int LayersCount { get; }
    public GameObjects GameObjects { get; }

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
        foreach (BaseActor actor in GameObjects.EnumerateAllActors(isEnabled: true))
        {
            actor.DoBehavior();
        }
    }

    public void ResurrectActors()
    {
        Vector2 camPos = Playfield.Camera.Position;
        bool newKnot = GameObjects.UpdateCurrentKnot(Playfield, camPos);

        foreach (GameObject obj in GameObjects.EnumerateAllGameObjects(isEnabled: false))
        {
            if (obj.ResurrectsImmediately)
                obj.SendMessage(Message.Resurrect);
        }

        if (newKnot && GameObjects.PreviousKnot != null)
        {
            foreach (GameObject obj in GameObjects.EnumerateActorsAndCaptors(isEnabled: false, knot: GameObjects.PreviousKnot))
            {
                if (obj.ResurrectsLater && GameObjects.CurrentKnot.ActorIds.Concat(GameObjects.CurrentKnot.CaptorIds).All(x => x != obj.Id))
                {
                    obj.SendMessage(Message.Resurrect);
                }
            }
        }
    }

    public void StepActors()
    {
        foreach (BaseActor actor in GameObjects.EnumerateAllActors(isEnabled: true))
        {
            actor.Step();
        }
    }

    public void MoveActors()
    {
        foreach (BaseActor actor in GameObjects.EnumerateAllActors(isEnabled: true))
        {
            if (actor is MovableActor movableActor)
                movableActor.Move();
        }
    }

    public void RunCaptors()
    {
        foreach (Captor captor in GameObjects.EnumerateCaptors(isEnabled: true))
        {
            if (captor.TriggerOnMainActorDetection)
            {
                MovableActor mainActor = GetMainActor();
                captor.IsTriggering = captor.GetAbsoluteBox(captor.CaptorBox).Intersects(mainActor.GetAbsoluteBox(mainActor.DetectionBox));
            }
            else
            {
                if (!captor.IsTriggering)
                {
                    foreach (BaseActor actor in GameObjects.EnumerateAllActors(isEnabled: true))
                    {
                        if (actor.IsAgainstCaptor && actor is ActionActor actionActor)
                        {
                            captor.IsTriggering = captor.GetAbsoluteBox(captor.CaptorBox).Intersects(actionActor.GetAbsoluteBox(actionActor.DetectionBox));
                        }
                    }
                }
            }

            if (captor.IsTriggering)
                captor.TriggerEvent();
        }
    }

    public void DrawActors()
    {
        foreach (BaseActor actor in GameObjects.EnumerateAllActors(isEnabled: true))
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
        // TODO: In multiplayer we get the actor from the machine id
        return (MovableActor)GameObjects.Objects[0];
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
        return Playfield.GetPhysicalValue((position / Constants.TileSize).ToPoint());
    }
}