using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public abstract class Bar
{
    public int Mode { get; set; }

    public abstract void Init();
    public abstract void Load();
    public abstract void Draw(AnimationPlayer animationPlayer);
}