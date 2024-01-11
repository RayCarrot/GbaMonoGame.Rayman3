namespace GbaMonoGame;

/// <summary>
/// Custom camera class for allow variable game and screen sizes.
/// </summary>
public class ScreenCamera : GfxCamera
{
    public ScreenCamera(GameWindow gameWindow) : base(gameWindow) { }

    protected override Vector2 GetResolution(GameWindow gameWindow) => gameWindow.GameResolution;
}