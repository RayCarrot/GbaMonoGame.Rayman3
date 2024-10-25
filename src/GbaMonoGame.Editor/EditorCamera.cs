using System;
using System.Collections.Generic;
using GbaMonoGame.TgxEngine;
using Microsoft.Xna.Framework.Input;

namespace GbaMonoGame.Editor;

public class EditorCamera : GfxCamera
{
    public EditorCamera(GameViewPort gameViewPort, Vector2 mapSize) : base(gameViewPort)
    {
        ScrollBounds = new Box(
            minX: 0 - ScrollMargin,
            minY: 0 - ScrollMargin,
            maxX: mapSize.X + ScrollMargin,
            maxY: mapSize.Y + ScrollMargin);

        GameLayers = new List<TgxGameLayer>();
    }

    private const int ScrollMargin = Tile.Size * 10;
    private const float DefaultScale = 1.5f;
    private const float MouseWheelZoomSpeed = 0.05f;

    private Vector2 _position;

    public Box ScrollBounds { get; }
    public Vector2 Position
    {
        get => _position;
        set
        {
            Vector2 minPos = ScrollBounds.Position;
            Vector2 maxPos = new(Math.Max(minPos.X, ScrollBounds.MaxX - Resolution.X), Math.Max(minPos.Y, ScrollBounds.MaxY - Resolution.Y));

            _position = Vector2.Clamp(value, minPos, maxPos);

            foreach (TgxGameLayer gameLayer in GameLayers)
                gameLayer.SetOffset(_position);
        }
    }
    public List<TgxGameLayer> GameLayers { get; }

    public float Scale { get; set; } = DefaultScale;

    protected override Vector2 GetResolution(GameViewPort gameViewPort)
    {
        Vector2 newGameResolution = gameViewPort.GameResolution * Scale;

        Vector2 max = ScrollBounds.Size;

        if (newGameResolution.X > newGameResolution.Y)
        {
            if (newGameResolution.Y > max.Y)
                newGameResolution = new Vector2(max.Y * newGameResolution.X / newGameResolution.Y, max.Y);

            if (newGameResolution.X > max.X)
                newGameResolution = new Vector2(max.X, max.X * newGameResolution.Y / newGameResolution.X);
        }
        else
        {
            if (newGameResolution.X > max.X)
                newGameResolution = new Vector2(max.X, max.X * newGameResolution.Y / newGameResolution.X);

            if (newGameResolution.Y > max.Y)
                newGameResolution = new Vector2(max.Y * newGameResolution.X / newGameResolution.Y, max.Y);
        }

        return newGameResolution;
    }

    public bool IsActorFramed(EditableActor actor)
    {
        Box viewBox = actor.GetViewBox();
        Box camBox = new(Position, Resolution);

        bool isFramed = viewBox.Intersects(camBox);

        if (isFramed)
            actor.AnimatedObject.ScreenPos = actor.Position - Position;

        return isFramed;
    }

    public void AddGameLayer(TgxGameLayer gameLayer)
    {
        GameLayers.Add(gameLayer);
    }

    public void Step()
    {
        // Zoom
        int wheelDelta = InputManager.GetMouseWheelDelta();
        if (wheelDelta < 0)
        {
            Scale += MouseWheelZoomSpeed;
            UpdateResolution();
        }
        else if (wheelDelta > 0)
        {
            Scale -= MouseWheelZoomSpeed;
            UpdateResolution();
        }

        // Scroll
        if (InputManager.GetMouseState().RightButton == ButtonState.Pressed)
            Position += InputManager.GetMousePositionDelta(this) * -1;
    }
}