using System;
using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba;

/// <summary>
/// Custom camera class for allow variable game and screen sizes.
/// </summary>
public class ScreenCamera
{
    public ScreenCamera()
    {
        GameResolution = Engine.Settings.Platform switch
        {
            Platform.GBA => new Vector2(240, 160),
            Platform.NGage => new Vector2(176, 208),
            _ => throw new UnsupportedPlatformException(),
        };
        ScaledGameResolution = GameResolution;
        _scale = Vector2.One;
    }

    private Vector2 _scale;

    public Vector2 ScaledGameResolution { get; private set; }
    public Vector2 GameResolution { get; }
    public Vector2 Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            ScaledGameResolution = GameResolution * value;

            // Refresh
            ResizeScreen(ScreenSize);
        }
    }
    public bool IsScaled => Scale != Vector2.One;

    public Rectangle ScreenRectangle { get; private set; }
    public Point ScreenSize { get; private set; }
    public Box ScaledVisibleArea { get; private set; }
    public Box VisibleArea { get; private set; }
    public Matrix ScaledTransformMatrix { get; private set; }
    public Matrix TransformMatrix { get; private set; }

    private Box GetVisibleArea(Matrix matrix)
    {
        Matrix inverseViewMatrix = Matrix.Invert(matrix);
        Vector2 tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
        Vector2 tr = Vector2.Transform(new Vector2(ScreenRectangle.Width, 0), inverseViewMatrix);
        Vector2 bl = Vector2.Transform(new Vector2(0, ScreenRectangle.Height), inverseViewMatrix);
        Vector2 br = Vector2.Transform(ScreenRectangle.Size.ToVector2(), inverseViewMatrix);
        Vector2 min = new(
            MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
            MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
        Vector2 max = new(
            MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
            MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
        return new Box(0, 0, max.X - min.X, max.Y - min.Y);
    }

    public Vector2 ToGamePosition(Vector2 pos) => Vector2.Transform(pos, Matrix.Invert(ScaledTransformMatrix));
    public Vector2 ToScreenPosition(Vector2 pos) => Vector2.Transform(pos, ScaledTransformMatrix);

    public bool IsVisible(Box rect, bool isScaled)
    {
        if (isScaled)
            return ScaledVisibleArea.Intersects(rect);
        else
            return VisibleArea.Intersects(rect);
    }
    public bool IsVisible(Vector2 position, Point size, bool isScaled)
    {
        if (isScaled)
            return ScaledVisibleArea.Intersects(new Box(position.X, position.Y, position.X + size.X, position.Y + size.Y));
        else
            return VisibleArea.Intersects(new Box(position.X, position.Y, position.X + size.X, position.Y + size.Y));
    }

    public void ResizeScreen(
        Point newScreenSize, 
        bool maintainScreenRatio = false, 
        bool centerGame = true, 
        Action<Point> changeScreenSizeCallback = null)
    {
        float screenRatio = newScreenSize.X / (float)newScreenSize.Y;
        float gameRatio = ScaledGameResolution.X / ScaledGameResolution.Y;

        float worldScale;
        float screenScale;
        Point screenPos = Point.Zero;
        Point screenSize;

        if (screenRatio > gameRatio)
        {
            worldScale = newScreenSize.Y / ScaledGameResolution.Y;
            screenScale = newScreenSize.Y / GameResolution.Y;

            if (maintainScreenRatio)
                newScreenSize = new Point((int)Math.Round(ScaledGameResolution.X * worldScale), (int)Math.Round(ScaledGameResolution.Y * worldScale));

            if (centerGame)
                screenPos = new Point((int)Math.Round((newScreenSize.X - ScaledGameResolution.X * worldScale) / 2), 0);
            
            screenSize = new Point((int)Math.Round(ScaledGameResolution.X * worldScale), newScreenSize.Y);
        }
        else
        {
            worldScale = newScreenSize.X / ScaledGameResolution.X;
            screenScale = newScreenSize.X / GameResolution.X;

            if (maintainScreenRatio)
                newScreenSize = new Point((int)Math.Round(ScaledGameResolution.X * worldScale), (int)Math.Round(ScaledGameResolution.Y * worldScale));

            if (centerGame)
                screenPos = new Point(0, (int)Math.Round((newScreenSize.Y - ScaledGameResolution.Y * worldScale) / 2));
            
            screenSize = new Point(newScreenSize.X, (int)Math.Round(ScaledGameResolution.Y * worldScale));
        }

        if (maintainScreenRatio)
            changeScreenSizeCallback?.Invoke(newScreenSize);

        ScreenSize = newScreenSize;

        ScreenRectangle = new Rectangle(screenPos, screenSize);

        TransformMatrix = Matrix.CreateScale(screenScale) *
                                Matrix.CreateTranslation(ScreenRectangle.X, ScreenRectangle.Y, 0);
        ScaledTransformMatrix = Matrix.CreateScale(worldScale) *
                               Matrix.CreateTranslation(ScreenRectangle.X, ScreenRectangle.Y, 0);

        VisibleArea = GetVisibleArea(TransformMatrix);
        ScaledVisibleArea = GetVisibleArea(ScaledTransformMatrix);
    }
}