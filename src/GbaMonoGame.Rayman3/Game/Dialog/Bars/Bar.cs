using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public abstract class Bar
{
    protected static readonly int[] BounceData =
    {
        0, -3, -5, -6, -5, -3,
        0, 2, 4, 5, 4, 2,
        0, -2, -3, -4, -3, -2,
        0, 2, 3, 2,
        0, -1, -2, -1,
        0, 1,
        0, -1,
        0, 1,
        0, -1,
        0,
        0,
    };

    protected BarState State { get; set; } = BarState.Wait;
    protected int Mode { get; set; }

    public abstract void Load();
    public abstract void Set();
    public abstract void Draw(AnimationPlayer animationPlayer);

    protected enum BarState
    {
        Hide,
        MoveIn,
        MoveOut,
        Bounce,
        Wait,
    }
}