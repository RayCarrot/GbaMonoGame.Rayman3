using OnyxCs.Gba.AnimEngine;

namespace OnyxCs.Gba.Rayman3;

public abstract class Bar
{
    public abstract void Init();
    public abstract void Load();
    public abstract void Draw(AnimationPlayer animationPlayer);
}