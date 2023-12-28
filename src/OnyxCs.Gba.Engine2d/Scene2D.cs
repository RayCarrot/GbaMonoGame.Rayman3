using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Nintendo.GBA;
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Engine2d;

public class Scene2D
{
    public Scene2D(int id, Func<Scene2D, CameraActor> createCameraFunc, int layersCount)
    {
        LayersCount = layersCount;
        Camera = createCameraFunc(this);

        AnimationPlayer = new AnimationPlayer(false);
        Dialogs = new List<Dialog>(layersCount);

        Scene2DResource scene = Storage.LoadResource<Scene2DResource>(id);
        Playfield = TgxPlayfield.Load<TgxPlayfield2D>(scene.Playfield);

        GameObjects = new GameObjects(scene);
        GameObjects.Load(this, scene);

        Camera.LinkedObject = MainActor;
        Camera.SetFirstPosition();
    }

    public CameraActor Camera { get; }
    public List<Dialog> Dialogs { get; }
    public AnimationPlayer AnimationPlayer { get; }
    public TgxPlayfield2D Playfield { get; }
    public int LayersCount { get; }
    public GameObjects GameObjects { get; }

    // TODO: In multiplayer we get the actor from the machine id
    public MovableActor MainActor => (MovableActor)GameObjects.Objects[0];

    public void Init()
    {
        ResurrectActors();
        RunCamera();
        ProcessDialogs();
        DrawActors();
    }

    public void UnInit()
    {
        Playfield.UnInit();
    }

    public void Step()
    {
        RunActors();
        ResurrectActors();
        StepActors();
        MoveActors();
        RunCaptors();
        RunCamera();
        ProcessDialogs();
        DrawActors();
    }

    public void AddDialog(Dialog dialog, bool param1, bool param2) // TODO: USe the bool params like the game does
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
                obj.ProcessMessage(Message.Resurrect);
        }

        if (newKnot && GameObjects.PreviousKnot != null)
        {
            foreach (GameObject obj in GameObjects.EnumerateActorsAndCaptors(isEnabled: false, knot: GameObjects.PreviousKnot))
            {
                if (obj.ResurrectsLater && GameObjects.CurrentKnot.ActorIds.Concat(GameObjects.CurrentKnot.CaptorIds).All(x => x != obj.Id))
                {
                    obj.ProcessMessage(Message.Resurrect);
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
                captor.IsTriggering = captor.GetCaptorBox().Intersects(MainActor.GetDetectionBox());
            }
            else
            {
                if (!captor.IsTriggering)
                {
                    foreach (BaseActor actor in GameObjects.EnumerateAllActors(isEnabled: true))
                    {
                        if (actor.IsAgainstCaptor && actor is ActionActor actionActor)
                        {
                            captor.IsTriggering = captor.GetCaptorBox().Intersects(actionActor.GetDetectionBox());
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
        Camera.Step();
    }

    public bool IsDetectedMainActor(ActionActor actor)
    {
        return IsDetectedMainActor(actor.GetActionBox());
    }

    public bool IsDetectedMainActor(Box box)
    {
        return box.Intersects(MainActor.GetDetectionBox());
    }

    public bool IsHitMainActor(InteractableActor actor)
    {
        // TODO: Implement

        return false;
    }

    public PhysicalType GetPhysicalType(Vector2 position)
    {
        return new PhysicalType(Playfield.GetPhysicalValue((position / Constants.TileSize).ToPoint()));
    }
}