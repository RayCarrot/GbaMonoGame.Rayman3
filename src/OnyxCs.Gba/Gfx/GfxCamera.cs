using System;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba;

/// <summary>
/// Custom camera class for allow variable game and screen sizes.
/// </summary>
public class GfxCamera
{
    public GfxCamera(Point screenSize)
    {
        OriginalGameResolution = Gfx.Platform switch
        {
            Platform.GBA => new Point(240, 160),
            Platform.NGage => new Point(176, 208),
            _ => throw new UnsupportedPlatformException(),
        };
        GameResolution = OriginalGameResolution;
        ResizeScreen(screenSize);
    }

    public Point GameResolution { get; private set; }
    public Point OriginalGameResolution { get; }
    public Rectangle ScreenRectangle { get; private set; }
    public Point ScreenSize { get; private set; }
    public Box VisibleArea { get; private set; }
    public Matrix TransformMatrix { get; private set; }

    public Vector2 ToGamePosition(Vector2 pos) => Vector2.Transform(pos, Matrix.Invert(TransformMatrix));
    public Vector2 ToScreenPosition(Vector2 pos) => Vector2.Transform(pos, TransformMatrix);

    public bool IsVisible(Box rect) => VisibleArea.Intersects(rect);
    public bool IsVisible(Vector2 position, Point size) => VisibleArea.Intersects(new Box(position.X, position.Y, position.X + size.X, position.Y + size.Y));

    public void ResizeGame(Point newGameSize)
    {
        GameResolution = newGameSize;

        // Refresh
        ResizeScreen(ScreenSize);
    }

    public void ResizeScreen(
        Point newScreenSize, 
        bool maintainScreenRatio = false, 
        bool centerGame = true, 
        Action<Point> changeScreenSizeCallback = null)
    {
        float screenRatio = newScreenSize.X / (float)newScreenSize.Y;
        float gameRatio = GameResolution.X / (float)GameResolution.Y;

        float scale;
        Point screenPos = Point.Zero;
        Point screenSize;

        if (screenRatio > gameRatio)
        {
            scale = newScreenSize.Y / (float)GameResolution.Y;

            if (maintainScreenRatio)
                newScreenSize = new Point((int)Math.Round(GameResolution.X * scale), (int)Math.Round(GameResolution.Y * scale));

            if (centerGame)
                screenPos = new Point((int)Math.Round((newScreenSize.X - GameResolution.X * scale) / 2), 0);
            
            screenSize = new Point((int)Math.Round(GameResolution.X * scale), newScreenSize.Y);
        }
        else
        {
            scale = newScreenSize.X / (float)GameResolution.X;

            if (maintainScreenRatio)
                newScreenSize = new Point((int)Math.Round(GameResolution.X * scale), (int)Math.Round(GameResolution.Y * scale));

            if (centerGame)
                screenPos = new Point(0, (int)Math.Round((newScreenSize.Y - GameResolution.Y * scale) / 2));
            
            screenSize = new Point(newScreenSize.X, (int)Math.Round(GameResolution.Y * scale));
        }

        if (maintainScreenRatio)
            changeScreenSizeCallback?.Invoke(newScreenSize);

        ScreenSize = newScreenSize;

        ScreenRectangle = new Rectangle(screenPos, screenSize);
        TransformMatrix = Matrix.CreateScale(scale) *
                          Matrix.CreateTranslation(ScreenRectangle.X, ScreenRectangle.Y, 0);

        Matrix inverseViewMatrix = Matrix.Invert(TransformMatrix);
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
        VisibleArea = new Box(0, 0, max.X - min.X, max.Y - min.Y);
    }
}