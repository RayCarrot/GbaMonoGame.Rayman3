﻿using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GbaMonoGame.Editor;

public class EditableScene2D
{
    public EditableScene2D(int id)
    {
        AnimationPlayer = new AnimationPlayer(false, null);

        Scene2DResource sceneResource = Storage.LoadResource<Scene2DResource>(id);
        ClusterResource mainClusterResource = sceneResource.Playfield.Playfield2D.Clusters[0];
        MapSize = new Vector2(mainClusterResource.SizeX * Tile.Size, mainClusterResource.SizeY * Tile.Size);
        Camera = new EditorCamera(Engine.GameViewPort, MapSize);

        Engine.GameViewPort.SetResolutionBounds(null, MapSize);

        if (sceneResource.Playfield.Type == PlayfieldType.Playfield2D)
            Playfield = new EditablePlayfield2D(sceneResource.Playfield.Playfield2D, Camera);
        else
            throw new InvalidOperationException();

        AlwaysActors = new List<EditableActor>(sceneResource.AlwaysActors.Select(x => new EditableActor(this, x)));
        Actors = new List<EditableActor>(sceneResource.Actors.Select(x => new EditableActor(this, x)));
        // TODO: Load captors

        HoverBoxAObject = new DebugBoxAObject
        {
            Camera = Camera,
            Color = new Color(0xFF, 0xEB, 0x3B),
        };
        SelectionBoxAObject = new DebugBoxAObject
        {
            Camera = Camera,
            Color = new Color(0xf4, 0x43, 0x36),
        };
    }

    public Vector2 MapSize { get; }
    public EditorCamera Camera { get; }
    public AnimationPlayer AnimationPlayer { get; }
    public EditablePlayfield2D Playfield { get; }

    // Game objects
    public List<EditableActor> AlwaysActors { get; }
    public List<EditableActor> Actors { get; }

    // Graphics
    public DebugBoxAObject HoverBoxAObject { get; }
    public DebugBoxAObject SelectionBoxAObject { get; }

    // Editor state
    public bool IsDraggingObject { get; set; }
    public EditableGameObject HoverGameObject { get; set; }
    public EditableGameObject SelectedGameObject { get; set; }

    public IEnumerable<EditableGameObject> EnumerateGameObjects() => AlwaysActors.Concat(Actors);
    public IEnumerable<EditableActor> EnumerateActors() => AlwaysActors.Concat(Actors);

    public void Init()
    {
        RunCamera();
        DrawActors();
    }

    public void UnInit()
    {
        Camera.UnInit();
    }

    public void Step()
    {
        StepGameObjects();
        RunCamera();
        DrawActors();
    }

    public void StepGameObjects()
    {
        Vector2 mousePos = InputManager.GetMousePosition(Camera) + Camera.Position;

        // Hover over object
        HoverGameObject = null;
        foreach (EditableGameObject obj in EnumerateGameObjects().Reverse())
        {
            Box selectionBox = obj.GetSelectionBox();
            
            if (selectionBox.Contains(mousePos))
                HoverGameObject = obj;
        }

        // Move object
        if (InputManager.GetMouseState().LeftButton == ButtonState.Pressed)
        {
            if (!IsDraggingObject)
            {
                if (HoverGameObject == null)
                {
                    SelectedGameObject = null;
                }
                else
                {
                    IsDraggingObject = true;
                    SelectedGameObject = HoverGameObject;
                }
            }

            if (IsDraggingObject && SelectedGameObject != null)
                SelectedGameObject.Position += InputManager.GetMousePositionDelta(Camera);
        }
        else
        {
            IsDraggingObject = false;
        }

    }

    public void DrawActors()
    {
        // Draw actors
        foreach (EditableActor actor in EnumerateActors())
            actor.Draw(AnimationPlayer);

        // Draw selected object box
        if (SelectedGameObject != null)
        {
            Box selectionBox = SelectedGameObject.GetSelectionBox();
            SelectionBoxAObject.Position = selectionBox.Position - Camera.Position;
            SelectionBoxAObject.Size = selectionBox.Size;
            AnimationPlayer.PlayFront(SelectionBoxAObject);
        }

        // Draw hovering object box
        if (HoverGameObject != null && !IsDraggingObject && HoverGameObject != SelectedGameObject)
        {
            Box selectionBox = HoverGameObject.GetSelectionBox();
            HoverBoxAObject.Position = selectionBox.Position - Camera.Position;
            HoverBoxAObject.Size = selectionBox.Size;
            AnimationPlayer.PlayFront(HoverBoxAObject);
        }
    }

    public void RunCamera()
    {
        Camera.Step();
    }
}