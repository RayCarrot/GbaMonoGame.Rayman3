using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Engine2d;

public class Scene2D
{
    public Scene2D(int id, Func<Scene2D, CameraActor> createCameraFunc, int layersCount)
    {
        LayersCount = layersCount;
        Camera = createCameraFunc(this);
        HudCamera = new HudCamera(Engine.GameViewPort);

        Flag_2 = true;
        AnimationPlayer = new AnimationPlayer(false, SoundEventsManager.ProcessEvent);
        Dialogs = new List<Dialog>(layersCount);
        DialogFlags = new List<bool>(layersCount);

        Scene2DResource scene = Storage.LoadResource<Scene2DResource>(id);

        Playfield = TgxPlayfield.Load(scene.Playfield);

        // TODO: Set bounds if Mode7
        if (Playfield is TgxPlayfield2D playfield2D)
            Engine.GameViewPort.SetResolutionBounds(null, playfield2D.Size);

        KnotManager = new KnotManager(scene);
        KnotManager.LoadGameObjects(this, scene);

        Camera.LinkedObject = MainActor;

        // Game does this ugly hack here for some reason to disable background scrolling in Cave of Bad Dreams 1 TODO: Allow this to be be ignored
        if (id == 11)
            ((TgxPlayfield2D)Playfield).Camera.GetCluster(1).ScrollFactor = Vector2.Zero;

        Camera.SetFirstPosition();
    }

    // Scene2DGameCube
    public Scene2D(GameCubeMap map, Func<Scene2D, CameraActor> createCameraFunc, int layersCount)
    {
        LayersCount = layersCount;
        Camera = createCameraFunc(this);
        HudCamera = new HudCamera(Engine.GameViewPort);

        Flag_2 = true;
        AnimationPlayer = new AnimationPlayer(false, SoundEventsManager.ProcessEvent);
        Dialogs = new List<Dialog>(layersCount);
        DialogFlags = new List<bool>(layersCount);

        Playfield = TgxPlayfield.Load<TgxPlayfield2D>(map.Playfield);

        // TODO: Set bounds if Mode7
        if (Playfield is TgxPlayfield2D playfield2D)
            Engine.GameViewPort.SetResolutionBounds(null, playfield2D.Size);

        KnotManager = new KnotManager(map.Scene);
        KnotManager.LoadGameObjects(this, map.Scene);

        Camera.LinkedObject = MainActor;
        Camera.SetFirstPosition();
    }

    public CameraActor Camera { get; }
    public HudCamera HudCamera { get; }
    public List<Dialog> Dialogs { get; }
    public List<bool> DialogFlags { get; }
    public AnimationPlayer AnimationPlayer { get; }
    public TgxPlayfield Playfield { get; }
    public int LayersCount { get; }
    public KnotManager KnotManager { get; }
    public int DialogIndex { get; set; }
    
    // Flags
    public bool Flag_1 { get; set; }
    public bool Flag_2 { get; set; }
    public bool Flag_3 { get; set; }
    public bool Flag_4 { get; set; }
    public bool Flag_5 { get; set; }
    public bool NGage_Flag_6 { get; set; }

    public Vector2 Resolution => Playfield.Camera.Resolution;

    public MovableActor MainActor => (MovableActor)(RSMultiplayer.IsActive ? GetGameObject(RSMultiplayer.MachineId) : GetGameObject(0));

    // TODO: This causes ResurrectActors to stop working. Perhaps we should still use
    //       knots to handle game logic, but still draw and step all actors in the scene?
    // If we're playing in a different resolution than the original we can't use
    // the knots (object sectors). Instead we keep all objects active at all times.
    public bool KeepAllObjectsActive => Playfield.Camera.Resolution != Engine.GameViewPort.OriginalGameResolution;

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
        FUN_0809cfd4();

        if (Engine.Settings.Platform == Platform.GBA)
        {
            if (!Flag_1)
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
            else
            {
                ProcessDialogs();
            }
        }
        else if (Engine.Settings.Platform == Platform.NGage)
        {
            if (!Flag_1 && !NGage_Flag_6)
            {
                RunActors();
                ResurrectActors();
                StepActors();
                MoveActors();
                RunCaptors();
                RunCamera();
                DrawActors();
            }
            
            ProcessDialogs();
        }
        else
        {
            throw new UnsupportedPlatformException();
        }
    }

    public bool AddDialog(Dialog dialog, bool param1, bool param2)
    {
        if (Flag_3)
            return false;

        if (param1)
        {
            if (!Flag_2)
                return false;

            DialogFlags.Add(true);
            Dialogs.Add(dialog);
            DialogIndex = Dialogs.Count - 1;

            Flag_1 = true;
            Flag_3 = true;
            Flag_4 = true;

            if (param2)
                Flag_5 = true;
        }
        else
        {
            DialogFlags.Add(false);
            Dialogs.Add(dialog);

            dialog.Load();
            dialog.Init();
        }

        return true;
    }

    public T GetDialog<T>()
        where T : Dialog
    {
        foreach (Dialog dialog in Dialogs)
        {
            if (dialog is T dlg)
                return dlg;
        }

        return null;
    }

    public T GetRequiredDialog<T>()
        where T : Dialog
    {
        foreach (Dialog dialog in Dialogs)
        {
            if (dialog is T dlg)
                return dlg;
        }

        throw new Exception($"Dialog of type {typeof(T)} has not been added to the scene");
    }

    public void ProcessDialogs()
    {
        if (!Flag_3)
        {
            for (int i = DialogIndex; i < Dialogs.Count; i++)
            {
                Dialogs[i].Step();
                Dialogs[i].Draw(AnimationPlayer);
            }
        }
    }

    public void FUN_0809e1cc()
    {
        if (!DialogFlags.Last())
        {
            Dialogs.RemoveAt(Dialogs.Count - 1);
            DialogFlags.RemoveAt(DialogFlags.Count - 1);
        }
        else
        {
            Dialogs.RemoveAt(Dialogs.Count - 1);
            DialogFlags.RemoveAt(DialogFlags.Count - 1);

            bool flag = true;

            for (int i = Dialogs.Count - 1; i >= 0; i--)
            {
                if (DialogFlags[i])
                {
                    Flag_1 = true;
                    DialogIndex = i;
                    flag = false;
                    break;
                }
            }

            if (flag)
            {
                DialogIndex = 0;
                Flag_1 = false;
            }

            Flag_4 = false;
            Flag_3 = true;
        }
    }

    public void FUN_0809cfd4()
    {
        if (Flag_3)
        {
            // TODO: A lot of weird unloading stuff

            Flag_3 = false;
        }
    }

    public void RunActors()
    {
        foreach (BaseActor actor in KnotManager.EnumerateAllActors(isEnabled: true))
        {
            actor.DoBehavior();
        }
    }

    public void ResurrectActors()
    {
        Vector2 camPos = Playfield.Camera.Position;
        bool newKnot = KnotManager.UpdateCurrentKnot(Playfield, camPos, KeepAllObjectsActive);

        foreach (GameObject obj in KnotManager.EnumerateAllGameObjects(isEnabled: false))
        {
            if (obj.ResurrectsImmediately)
                obj.ProcessMessage(null, Message.Resurrect);
        }

        if (newKnot && KnotManager.PreviousKnot != null)
        {
            foreach (BaseActor obj in KnotManager.EnumerateActors(isEnabled: false, knot: KnotManager.PreviousKnot))
            {
                if (obj.ResurrectsLater && !KnotManager.IsInCurrentKnot(obj))
                {
                    obj.ProcessMessage(null, Message.Resurrect);
                }
            }

            foreach (Captor obj in KnotManager.EnumerateCaptors(isEnabled: false, knot: KnotManager.PreviousKnot))
            {
                if (obj.ResurrectsLater && !KnotManager.IsInCurrentKnot(obj))
                {
                    obj.ProcessMessage(null, Message.Resurrect);
                }
            }
        }
    }

    public void StepActors()
    {
        foreach (BaseActor actor in KnotManager.EnumerateAllActors(isEnabled: true))
        {
            actor.Step();
        }
    }

    public void MoveActors()
    {
        foreach (BaseActor actor in KnotManager.EnumerateAllActors(isEnabled: true))
        {
            if (actor is MovableActor movableActor)
                movableActor.Move();
        }
    }

    public void RunCaptors()
    {
        foreach (Captor captor in KnotManager.EnumerateCaptors(isEnabled: true))
        {
            if (captor.TriggerOnMainActorDetection)
            {
                captor.IsTriggering = captor.GetCaptorBox().Intersects(MainActor.GetDetectionBox());
            }
            else
            {
                if (!captor.IsTriggering)
                {
                    foreach (BaseActor actor in KnotManager.EnumerateAllActors(isEnabled: true))
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
        foreach (BaseActor actor in KnotManager.EnumerateAllActors(isEnabled: true))
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
        return actor.GetAttackBox().Intersects(MainActor.GetVulnerabilityBox());
    }

    public InteractableActor IsHitActor(InteractableActor actor)
    {
        Box attackBox = actor.GetAttackBox();

        foreach (BaseActor actorToCheck in KnotManager.EnumerateAllActors(isEnabled: true))
        {
            // Ignore main actor if not in multiplayer
            if (!RSMultiplayer.IsActive && actorToCheck.InstanceId == 0)
                continue;

            // Check for collision
            if (actorToCheck != actor && 
                actorToCheck.ReceivesDamage && 
                actorToCheck is InteractableActor interactableActor && 
                interactableActor.GetVulnerabilityBox().Intersects(attackBox))
                return interactableActor;
        }

        return null;
    }

    public GameObject GetGameObject(int instanceId) => KnotManager.GetGameObject(instanceId);
    public T GetGameObject<T>(int instanceId) where T : GameObject => (T)KnotManager.GetGameObject(instanceId);

    public T CreateProjectile<T>(Enum actorType)
        where T : BaseActor
    {
        return (T)KnotManager.CreateProjectile((int)(object)actorType);
    }

    public BaseActor CreateProjectile(int actorType)
    {
        return KnotManager.CreateProjectile(actorType);
    }

    public PhysicalType GetPhysicalType(Vector2 position)
    {
        return new PhysicalType(Playfield.GetPhysicalValue((position / Constants.TileSize).ToPoint()));
    }
}