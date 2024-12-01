namespace GbaMonoGame;

public abstract class ScreenEffect
{
    public GfxCamera Camera { get; set; } = Engine.ScreenCamera;

    public abstract void Draw(GfxRenderer renderer);
}