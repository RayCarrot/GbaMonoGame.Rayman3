using System;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework;

namespace GbaMonoGame;

public class GameWindow
{
    public GameWindow(GbaEngineSettings settings)
    {
        OriginalGameResolution = settings.Platform switch
        {
            Platform.GBA => new Vector2(240, 160),
            Platform.NGage => new Vector2(176, 208),
            _ => throw new UnsupportedPlatformException(),
        };
        GameResolution = OriginalGameResolution;
    }

    public Vector2 OriginalGameResolution { get; }
    public Vector2 GameResolution { get; private set; }
    public float AspectRatio => GameResolution.X / GameResolution.Y;

    public Rectangle ScreenRectangle { get; private set; }
    public Point ScreenSize { get; private set; }

    protected virtual void OnResized()
    {
        Resized?.Invoke(this, EventArgs.Empty);
    }
    protected virtual void OnGameResolutionChanged()
    {
        GameResolutionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Resize(
        Point newScreenSize,
        bool maintainScreenRatio = false,
        bool centerGame = true,
        Action<Point> changeScreenSizeCallback = null)
    {
        float screenRatio = newScreenSize.X / (float)newScreenSize.Y;
        float gameRatio = GameResolution.X / GameResolution.Y;

        float screenScale;
        Point screenPos = Point.Zero;
        Point screenSize;

        if (screenRatio > gameRatio)
        {
            screenScale = newScreenSize.Y / GameResolution.Y;

            if (maintainScreenRatio)
                newScreenSize = new Point((int)Math.Round(GameResolution.X * screenScale), (int)Math.Round(GameResolution.Y * screenScale));

            if (centerGame)
                screenPos = new Point((int)Math.Round((newScreenSize.X - GameResolution.X * screenScale) / 2), 0);

            screenSize = new Point((int)Math.Round(GameResolution.X * screenScale), newScreenSize.Y);
        }
        else
        {
            screenScale = newScreenSize.X / GameResolution.X;

            if (maintainScreenRatio)
                newScreenSize = new Point((int)Math.Round(GameResolution.X * screenScale), (int)Math.Round(GameResolution.Y * screenScale));

            if (centerGame)
                screenPos = new Point(0, (int)Math.Round((newScreenSize.Y - GameResolution.Y * screenScale) / 2));

            screenSize = new Point(newScreenSize.X, (int)Math.Round(GameResolution.Y * screenScale));
        }

        if (maintainScreenRatio)
            changeScreenSizeCallback?.Invoke(newScreenSize);

        ScreenSize = newScreenSize;
        ScreenRectangle = new Rectangle(screenPos, screenSize);

        OnResized();
    }

    public void SetResolution(Vector2? resolution)
    {
        GameResolution = resolution ?? OriginalGameResolution;
        OnGameResolutionChanged();
        Resize(ScreenSize);
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
        Resize(ScreenSize);
    }

    public event EventHandler Resized;
    public event EventHandler GameResolutionChanged;
}