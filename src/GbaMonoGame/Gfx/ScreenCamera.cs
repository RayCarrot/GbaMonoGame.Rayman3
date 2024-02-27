namespace GbaMonoGame;

/// <summary>
/// Custom camera class for allow variable game and screen sizes.
/// </summary>
public class ScreenCamera : GfxCamera
{
    public ScreenCamera(GameViewPort gameViewPort) : base(gameViewPort) { }

    protected override Vector2 GetResolution(GameViewPort gameViewPort) => gameViewPort.GameResolution;
}