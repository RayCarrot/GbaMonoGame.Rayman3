using System;
using Microsoft.Xna.Framework;

namespace GbaMonoGame;

// The follow base class is custom and not in the original GBA engine. We use this for cameras responsible for rendering graphics.
public abstract class GfxCamera
{
    protected GfxCamera(GameWindow gameWindow)
    {
        GameWindow = gameWindow;
        UpdateResolution();

        gameWindow.GameResolutionChanged += GameWindow_GameResolutionChanged;
        gameWindow.Resized += GameWindow_Resized;
    }

    private Vector2 _resolution;
    private Matrix _matrix;

    private GameWindow GameWindow { get; }

    public Vector2 Resolution
    {
        get => _resolution;
        private set
        {
            _resolution = value;
            Matrix = CreateRenderMatrix(Resolution);
        }
    }

    public Matrix Matrix
    {
        get => _matrix;
        private set
        {
            _matrix = value;
            VisibleArea = GetVisibleArea(Matrix);
        }
    }

    public Box VisibleArea { get; private set; }

    private void GameWindow_GameResolutionChanged(object sender, EventArgs e) => 
        UpdateResolution();
    private void GameWindow_Resized(object sender, EventArgs e) => 
        Matrix = CreateRenderMatrix(Resolution);

    private Matrix CreateRenderMatrix(Vector2 resolution)
    {
        float screenRatio = GameWindow.ScreenSize.X / (float)GameWindow.ScreenSize.Y;
        float gameRatio = GameWindow.GameResolution.X / GameWindow.GameResolution.Y;

        float worldScale;

        if (screenRatio > gameRatio)
            worldScale = GameWindow.ScreenSize.Y / resolution.Y;
        else
            worldScale = GameWindow.ScreenSize.X / resolution.X;

        return Matrix.CreateScale(worldScale) *
               Matrix.CreateTranslation(GameWindow.ScreenRectangle.X, GameWindow.ScreenRectangle.Y, 0);
    }

    private Box GetVisibleArea(Matrix matrix)
    {
        Matrix inverseViewMatrix = Matrix.Invert(matrix);
        Vector2 tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
        Vector2 tr = Vector2.Transform(new Vector2(GameWindow.ScreenRectangle.Width, 0), inverseViewMatrix);
        Vector2 bl = Vector2.Transform(new Vector2(0, GameWindow.ScreenRectangle.Height), inverseViewMatrix);
        Vector2 br = Vector2.Transform(GameWindow.ScreenRectangle.Size.ToVector2(), inverseViewMatrix);
        Vector2 min = new(
            MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
            MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
        Vector2 max = new(
            MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
            MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
        return new Box(0, 0, max.X - min.X, max.Y - min.Y);
    }

    protected abstract Vector2 GetResolution(GameWindow gameWindow);

    protected void UpdateResolution()
    {
        Resolution = GetResolution(GameWindow);
    }

    public Vector2 ToWorldPosition(Vector2 pos) => Vector2.Transform(pos, Matrix.Invert(Matrix));
    public Vector2 ToScreenPosition(Vector2 pos) => Vector2.Transform(pos, Matrix);

    public bool IsVisible(Box rect) => VisibleArea.Intersects(rect);
    public bool IsVisible(Vector2 position, Point size) => VisibleArea.Intersects(new Box(position.X, position.Y, position.X + size.X, position.Y + size.Y));
    public bool IsVisible(Vector2 position) => VisibleArea.Contains(position);

    public virtual void UnInit()
    {
        GameWindow.GameResolutionChanged -= GameWindow_GameResolutionChanged;
        GameWindow.Resized -= GameWindow_Resized;
    }
}