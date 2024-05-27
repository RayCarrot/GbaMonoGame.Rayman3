namespace GbaMonoGame.Rayman3;

public partial class Piranha
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Move_Right = 0,
        Move_Left = 1,
        Dying_Right = 2,
        Dying_Left = 3,
        Dead_Right = 4,
        Dead_Left = 5,
    }
}