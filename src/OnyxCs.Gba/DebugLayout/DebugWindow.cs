namespace OnyxCs.Gba;

public abstract class DebugWindow
{
    public abstract string Name { get; }
    public virtual bool CanClose => true;
    public bool IsOpen { get; set; } = true; // TODO: Save if window is open from last instance

    public abstract void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager);

    public virtual void DrawGame(GfxRenderer renderer) { }
}