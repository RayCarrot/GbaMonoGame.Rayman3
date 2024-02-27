using System;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework;

namespace GbaMonoGame;

public class GameViewPort
{
    public GameViewPort(GbaEngineSettings settings)
    {
        OriginalGameResolution = settings.Platform switch
        {
            Platform.GBA => new Vector2(240, 160),
            Platform.NGage => new Vector2(176, 208),
            _ => throw new UnsupportedPlatformException(),
        };
        RequestedGameResolution = OriginalGameResolution;
        GameResolution = OriginalGameResolution;
    }

    public Vector2 OriginalGameResolution { get; }
    public Vector2? MinGameResolution { get; private set; }
    public Vector2? MaxGameResolution { get; private set; }
    public Vector2 RequestedGameResolution { get; private set; }
    public Vector2 GameResolution { get; private set; }
    public float AspectRatio => GameResolution.X / GameResolution.Y;

    public Box ScreenBox { get; private set; }
    public Rectangle ScreenRectangle { get; private set; }
    public Vector2 ScreenSizeVector { get; private set; }
    public Point ScreenSizePoint { get; private set; }

    private void UpdateGameResolution()
    {
        Vector2 originalGameResolution = GameResolution;
        Vector2 newGameResolution = RequestedGameResolution;

        if (MaxGameResolution is { } max)
        {
            if (newGameResolution.X > max.X)
                newGameResolution = new Vector2(max.X, newGameResolution.Y);
            if (newGameResolution.Y > max.Y)
                newGameResolution = new Vector2(newGameResolution.X, max.Y);
        }
        if (MinGameResolution is { } min)
        {
            if (newGameResolution.X < min.X)
                newGameResolution = new Vector2(min.X, newGameResolution.Y);
            if (newGameResolution.Y < min.Y)
                newGameResolution = new Vector2(newGameResolution.X, min.Y);
        }

        GameResolution = newGameResolution;

        if (GameResolution != originalGameResolution)
        {
            OnGameResolutionChanged();
            Resize(ScreenSizeVector);
        }
    }

    protected virtual void OnResized()
    {
        Resized?.Invoke(this, EventArgs.Empty);
    }
    protected virtual void OnGameResolutionChanged()
    {
        GameResolutionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Resize(
        Vector2 newScreenSize,
        bool maintainScreenRatio = false,
        bool centerGame = true,
        Action<Vector2> changeScreenSizeCallback = null)
    {
        float screenRatio = newScreenSize.X / newScreenSize.Y;
        float gameRatio = GameResolution.X / GameResolution.Y;

        float screenScale;
        Vector2 screenPos = Vector2.Zero;
        Vector2 screenSize;

        if (screenRatio > gameRatio)
        {
            screenScale = newScreenSize.Y / GameResolution.Y;

            if (maintainScreenRatio)
                newScreenSize = new Vector2(GameResolution.X * screenScale, GameResolution.Y * screenScale);

            if (centerGame)
                screenPos = new Vector2((newScreenSize.X - GameResolution.X * screenScale) / 2, 0);

            screenSize = new Vector2(GameResolution.X * screenScale, newScreenSize.Y);
        }
        else
        {
            screenScale = newScreenSize.X / GameResolution.X;

            if (maintainScreenRatio)
                newScreenSize = new Vector2(GameResolution.X * screenScale, GameResolution.Y * screenScale);

            if (centerGame)
                screenPos = new Vector2(0, (newScreenSize.Y - GameResolution.Y * screenScale) / 2);

            screenSize = new Vector2(newScreenSize.X, GameResolution.Y * screenScale);
        }

        if (maintainScreenRatio)
            changeScreenSizeCallback?.Invoke(newScreenSize);

        ScreenSizeVector = newScreenSize;
        ScreenSizePoint = newScreenSize.ToRoundedPoint();
        ScreenBox = new Box(screenPos, screenSize);
        ScreenRectangle = new Rectangle(
            (int)MathF.Ceiling(ScreenBox.MinX), 
            (int)MathF.Ceiling(ScreenBox.MinY), 
            (int)MathF.Floor(ScreenBox.MaxX) - (int)MathF.Ceiling(ScreenBox.MinX), 
            (int)MathF.Floor(ScreenBox.MaxY) - (int)MathF.Ceiling(ScreenBox.MinY));

        OnResized();
    }

    public void SetRequestedResolution(Vector2? resolution)
    {
        RequestedGameResolution = resolution ?? OriginalGameResolution;
        UpdateGameResolution();
    }

    public void SetResolutionBoundsToOriginalResolution()
    {
        SetResolutionBounds(Engine.GameViewPort.OriginalGameResolution, Engine.GameViewPort.OriginalGameResolution);
    }

    public void SetResolutionBounds(Vector2? minResolution, Vector2? maxResolution)
    {
        MinGameResolution = minResolution;
        MaxGameResolution = maxResolution;
        UpdateGameResolution();
    }

    public void SetAspectRatio(float aspectRatio, bool crop)
    {
        if ((crop && aspectRatio < 1) || (!crop && aspectRatio > 1))
        {
            float height = OriginalGameResolution.Y;
            float width = height * aspectRatio;

            GameResolution = new Vector2(width, height);
        }
        else
        {
            float width = OriginalGameResolution.X;
            float height = width / aspectRatio;

            GameResolution = new Vector2(width, height);
        }

        OnGameResolutionChanged();
        Resize(ScreenSizeVector);
    }

    public event EventHandler Resized;
    public event EventHandler GameResolutionChanged;
}