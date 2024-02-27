using System;
using Microsoft.Xna.Framework;

namespace GbaMonoGame;

// The follow base class is custom and not in the original GBA engine. We use this for cameras responsible for rendering graphics.
public abstract class GfxCamera
{
    protected GfxCamera(GameViewPort gameViewPort)
    {
        GameViewPort = gameViewPort;

        gameViewPort.GameResolutionChanged += GameViewPort_GameResolutionChanged;
        gameViewPort.Resized += GameViewPort_Resized;
    }

    private bool _hasSetResolution;
    private Vector2 _resolution;
    private Matrix _matrix;
    private Box _visibleArea;

    private GameViewPort GameViewPort { get; }

    public Vector2 Resolution
    {
        get
        {
            if (!_hasSetResolution)
                UpdateResolution();

            return _resolution;
        }
        private set
        {
            _resolution = value;
            _hasSetResolution = true;
            Matrix = CreateRenderMatrix(Resolution);
        }
    }

    public Matrix Matrix
    {
        get
        {
            if (!_hasSetResolution)
                UpdateResolution();

            return _matrix;
        }
        private set
        {
            _matrix = value;
            VisibleArea = GetVisibleArea(Matrix);
        }
    }

    public Box VisibleArea
    {
        get
        {
            if (!_hasSetResolution)
                UpdateResolution();

            return _visibleArea;
        }
        private set => _visibleArea = value;
    }

    private void GameViewPort_GameResolutionChanged(object sender, EventArgs e) => 
        UpdateResolution();
    private void GameViewPort_Resized(object sender, EventArgs e) => 
        Matrix = CreateRenderMatrix(Resolution);

    private Matrix CreateRenderMatrix(Vector2 resolution)
    {
        float screenRatio = GameViewPort.ScreenSizeVector.X / GameViewPort.ScreenSizeVector.Y;
        float gameRatio = GameViewPort.GameResolution.X / GameViewPort.GameResolution.Y;

        float worldScale;

        if (screenRatio > gameRatio)
            worldScale = GameViewPort.ScreenSizeVector.Y / resolution.Y;
        else
            worldScale = GameViewPort.ScreenSizeVector.X / resolution.X;

        return Matrix.CreateScale(worldScale) *
               Matrix.CreateTranslation(GameViewPort.ScreenBox.MinX, GameViewPort.ScreenBox.MinY, 0);
    }

    private Box GetVisibleArea(Matrix matrix)
    {
        Matrix inverseViewMatrix = Matrix.Invert(matrix);
        Vector2 tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
        Vector2 tr = Vector2.Transform(new Vector2(GameViewPort.ScreenBox.Width, 0), inverseViewMatrix);
        Vector2 bl = Vector2.Transform(new Vector2(0, GameViewPort.ScreenBox.Height), inverseViewMatrix);
        Vector2 br = Vector2.Transform(GameViewPort.ScreenBox.Size, inverseViewMatrix);
        Vector2 min = new(
            MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
            MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
        Vector2 max = new(
            MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
            MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
        return new Box(0, 0, max.X - min.X, max.Y - min.Y);
    }

    protected abstract Vector2 GetResolution(GameViewPort gameViewPort);

    protected void UpdateResolution()
    {
        Resolution = GetResolution(GameViewPort);
    }

    public Vector2 ToWorldPosition(Vector2 pos) => Vector2.Transform(pos, Matrix.Invert(Matrix));
    public Vector2 ToScreenPosition(Vector2 pos) => Vector2.Transform(pos, Matrix);

    public bool IsVisible(Box rect) => VisibleArea.Intersects(rect);
    public bool IsVisible(Vector2 position, Point size) => VisibleArea.Intersects(new Box(position.X, position.Y, position.X + size.X, position.Y + size.Y));
    public bool IsVisible(Vector2 position) => VisibleArea.Contains(position);

    public virtual void UnInit()
    {
        GameViewPort.GameResolutionChanged -= GameViewPort_GameResolutionChanged;
        GameViewPort.Resized -= GameViewPort_Resized;
    }
}