namespace GbaMonoGame.Rayman3;

public partial class SpikyFlyingBomb
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Move_Left = 0,
        Move_Right = 1,
        Move_Up = 2,
        Move_Down = 3,
        Action_4 = 4, // TODO: Name
        Stationary = 5,
        Action_6 = 6, // TODO: Name
        Action_7 = 7, // TODO: Name
    }
}