namespace GbaMonoGame.Rayman3;

public partial class BreakableDoor
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Idle_Right = 0,
        Idle_Left = 1,
        Break_Right = 2,
        Break_Left = 3,
        Shake_Right = 4,
        Shake_Left = 5,
    }
}