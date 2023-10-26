using System;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba;

public class GfxCamera
{
    public GfxCamera(GraphicsDeviceManager graphics, Point screenSize)
    {
        Graphics = graphics;
        OriginalGameResolution = new Point(Constants.ScreenWidth, Constants.ScreenHeight);
        GameResolution = OriginalGameResolution;
        Resize(screenSize, false);
    }

    private GraphicsDeviceManager Graphics { get; }

    public Point GameResolution { get; private set; }
    public Point OriginalGameResolution { get; private set; }
    public Rectangle ScreenRectangle { get; private set; }
    public Rectangle VisibleArea { get; private set; }
    public Matrix TransformMatrix { get; private set; }

    public bool IsVisible(Rectangle rect) => VisibleArea.Intersects(rect);

    public (Rectangle Source, Rectangle Destination) GetVisibleRectangle(Rectangle source, Rectangle destination)
    {
        Rectangle visibleDestinationRectangle = Rectangle.Intersect(destination, VisibleArea);
        Point ratio = destination.Size / source.Size;

        visibleDestinationRectangle.Size *= ratio;

        source.Offset(visibleDestinationRectangle.Location - destination.Location);
        source.Size = visibleDestinationRectangle.Size;

        return (source, visibleDestinationRectangle);
    }

    public void Resize(Point newScreenSize, bool maintainAspectRatio, bool centerGame = true)
    {
        float screenRatio = newScreenSize.X / (float)newScreenSize.Y;
        float gameRatio = GameResolution.X / (float)GameResolution.Y;

        float scale;
        Point screenPos = Point.Zero;
        Point screenSize;

        if (screenRatio > gameRatio)
        {
            scale = newScreenSize.Y / (float)GameResolution.Y;

            if (maintainAspectRatio)
                newScreenSize = new Point((int)Math.Round(GameResolution.X * scale), (int)Math.Round(GameResolution.Y * scale));

            if (centerGame)
                screenPos = new Point((int)Math.Round((newScreenSize.X - GameResolution.X * scale) / 2), 0);
            
            screenSize = new Point((int)Math.Round(GameResolution.X * scale), newScreenSize.Y);
        }
        else
        {
            scale = newScreenSize.X / (float)GameResolution.X;

            if (maintainAspectRatio)
                newScreenSize = new Point((int)Math.Round(GameResolution.X * scale), (int)Math.Round(GameResolution.Y * scale));

            if (centerGame)
                screenPos = new Point(0, (int)Math.Round((newScreenSize.Y - GameResolution.Y * scale) / 2));
            
            screenSize = new Point(newScreenSize.X, (int)Math.Round(GameResolution.Y * scale));
        }

        if (maintainAspectRatio)
        {
            Graphics.PreferredBackBufferWidth = newScreenSize.X;
            Graphics.PreferredBackBufferHeight = newScreenSize.Y;
            Graphics.ApplyChanges();
        }

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
        VisibleArea = new Rectangle(0, 0, (int)Math.Round(max.X - min.X), (int)Math.Round(max.Y - min.Y));
    }
}