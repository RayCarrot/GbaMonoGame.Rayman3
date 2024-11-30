using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Engine2d;

public class Scene2D
{
    public Scene2D(int id, Func<Scene2D, CameraActor> createCameraFunc, int layersCount, int actorDrawPriority)
    {
        LayersCount = layersCount;
        ActorDrawPriority = actorDrawPriority;
        Camera = createCameraFunc(this);
        HudCamera = new HudCamera(Engine.GameViewPort);

        AllowModalDialogs = true;
        AnimationPlayer = new AnimationPlayer(false, SoundEventsManager.ProcessEvent);
        Dialogs = new List<Dialog>(layersCount);
        DialogModalFlags = new List<bool>(layersCount);

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
    public Scene2D(GameCubeMap map, Func<Scene2D, CameraActor> createCameraFunc, int layersCount, int actorDrawPriority)
    {
        LayersCount = layersCount;
        Camera = createCameraFunc(this);
        HudCamera = new HudCamera(Engine.GameViewPort);

        AllowModalDialogs = true;
        AnimationPlayer = new AnimationPlayer(false, SoundEventsManager.ProcessEvent);
        Dialogs = new List<Dialog>(layersCount);
        DialogModalFlags = new List<bool>(layersCount);

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
    public List<bool> DialogModalFlags { get; }
    public AnimationPlayer AnimationPlayer { get; }
    public TgxPlayfield Playfield { get; }
    public int LayersCount { get; }
    public int ActorDrawPriority { get; }
    public KnotManager KnotManager { get; }
    public int FirstActiveDialogIndex { get; set; }

    public bool ShowDebugBoxes { get; set; }
    
    // Flags
    public bool InDialogModalMode { get; set; }
    public bool AllowModalDialogs { get; set; }
    public bool PendingDialogRefresh { get; set; }
    public bool InitializeNewModalDialog { get; set; }
    public bool ReloadPlayfield { get; set; }
    public bool NGage_Flag_6 { get; set; }

    public Vector2 Resolution => Playfield.Camera.Resolution;

    public MovableActor MainActor => (MovableActor)(RSMultiplayer.IsActive ? GetGameObject(RSMultiplayer.MachineId) : GetGameObject(0));

    // TODO: This causes ResurrectActors to stop working, which is an issue for actors which respawn, such as FallingPlatform.
    //       Perhaps we should still use knots to handle game logic, but still draw and step all actors in the scene?
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
        RefreshDialogs();

        if (Engine.Settings.Platform == Platform.GBA)
        {
            if (InDialogModalMode)
            {
                ProcessDialogs();
            }
            else
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
        }
        else if (Engine.Settings.Platform == Platform.NGage)
        {
            if (InDialogModalMode || NGage_Flag_6)
            {
                ProcessDialogs();
            }
            else
            {
                RunActors();
                ResurrectActors();
                StepActors();
                MoveActors();
                RunCaptors();
                RunCamera();
                DrawActors();
                ProcessDialogs();
            }
        }
        else
        {
            throw new UnsupportedPlatformException();
        }

        // Toggle showing debug boxes
        if (InputManager.IsButtonJustPressed(Input.Debug_ToggleBoxes))
            ShowDebugBoxes = !ShowDebugBoxes;

        // Draw debug boxes
        if (!InDialogModalMode && ShowDebugBoxes)
            DrawDebugBoxes();
    }

    public bool AddDialog(Dialog dialog, bool isModal, bool reloadPlayfield)
    {
        // Can't add new dialogs if a refresh is pending
        if (PendingDialogRefresh)
            return false;

        // Modal (for example the pause dialog)
        if (isModal)
        {
            if (!AllowModalDialogs)
                return false;

            DialogModalFlags.Add(true);
            Dialogs.Add(dialog);
            FirstActiveDialogIndex = Dialogs.Count - 1;

            InDialogModalMode = true;
            PendingDialogRefresh = true;
            InitializeNewModalDialog = true;

            if (reloadPlayfield)
                ReloadPlayfield = true;
        }
        // Normal
        else
        {
            DialogModalFlags.Add(false);
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
        // Can't process if a refresh is pending
        if (PendingDialogRefresh) 
            return;
        
        for (int i = FirstActiveDialogIndex; i < Dialogs.Count; i++)
        {
            Dialogs[i].Step();
            Dialogs[i].Draw(AnimationPlayer);
        }
    }

    public void RemoveLastDialog()
    {
        // If the last dialog is a modal...
        if (DialogModalFlags.Last())
        {
            // Remove the modal dialog
            Dialogs.RemoveAt(Dialogs.Count - 1);
            DialogModalFlags.RemoveAt(DialogModalFlags.Count - 1);

            // Check if there is another modal dialog and if so have that one be active
            bool endModalMode = true;
            for (int i = Dialogs.Count - 1; i >= 0; i--)
            {
                // Found a modal dialog
                if (DialogModalFlags[i])
                {
                    InDialogModalMode = true;
                    FirstActiveDialogIndex = i;
                    endModalMode = false;
                    break;
                }
            }

            // End modal mode if there were no other modal dialogs found
            if (endModalMode)
            {
                FirstActiveDialogIndex = 0;
                InDialogModalMode = false;
            }

            InitializeNewModalDialog = false;
            PendingDialogRefresh = true;
        }
        // If the last dialog is a normal one we just remove it
        else
        {
            Dialogs.RemoveAt(Dialogs.Count - 1);
            DialogModalFlags.RemoveAt(DialogModalFlags.Count - 1);
        }
    }

    public void RefreshDialogs()
    {
        if (!PendingDialogRefresh) 
            return;
        
        // Game resets the animation palette and sprite managers here

        if (ReloadPlayfield)
            throw new NotImplementedException();

        // If we're exiting modal mode we want to reload animation data
        if (!InDialogModalMode)
        {
            if (ReloadPlayfield)
                throw new NotImplementedException();

            KnotManager.ReloadAnimations();

            for (int i = FirstActiveDialogIndex; i < Dialogs.Count; i++)
                Dialogs[i].Load();
        }
        // A new model dialog has been added which we want to load and initialize
        else if (InitializeNewModalDialog)
        {
            Dialogs[FirstActiveDialogIndex].Load();
            Dialogs[FirstActiveDialogIndex].Init();
        }
        // No new modal dialog has been added, so just reload the animation data for what's there from before
        else
        {
            for (int i = FirstActiveDialogIndex; i < Dialogs.Count; i++)
                Dialogs[i].Load();
        }

        PendingDialogRefresh = false;
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

    public void DrawDebugBoxes()
    {
        foreach (GameObject obj in KnotManager.EnumerateAllGameObjects(isEnabled: true))
        {
            obj.DrawDebugBoxes(AnimationPlayer);
        }
    }

    public bool IsDetectedMainActor(ActionActor actor)
    {
        Box mainActorDetectionBox = MainActor.GetDetectionBox();
        Box actionBox = actor.GetActionBox();

        return mainActorDetectionBox.Intersects(actionBox);
    }

    public bool IsDetectedMainActor(ActionActor actor, float addMaxY, float addMinY, float addMinX, float addMaxX)
    {
        Box mainActorDetectionBox = MainActor.GetDetectionBox();
        Box actionBox = actor.GetActionBox();

        actionBox = new Box(
            minX: actionBox.MinX + addMinX,
            minY: actionBox.MinY + addMinY,
            maxX: actionBox.MaxX + addMaxX,
            maxY: actionBox.MaxY + addMaxY);

        return mainActorDetectionBox.Intersects(actionBox);
    }

    public bool IsHitMainActor(InteractableActor actor)
    {
        Box mainActorVulnerabilityBox = MainActor.GetVulnerabilityBox();
        Box attackBox = actor.GetAttackBox();

        return mainActorVulnerabilityBox.Intersects(attackBox);
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

    public InteractableActor IsHitActorOfType(InteractableActor actor, int type)
    {
        Box attackBox = actor.GetAttackBox();

        foreach (BaseActor actorToCheck in KnotManager.EnumerateAllActors(isEnabled: true))
        {
            // Ignore main actor if not in multiplayer
            if (!RSMultiplayer.IsActive && actorToCheck.InstanceId == 0)
                continue;

            // Check for collision
            if (actorToCheck != actor && 
                actorToCheck.Type == type &&
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
        return new PhysicalType(Playfield.GetPhysicalValue((position / Tile.Size).ToPoint()));
    }
}