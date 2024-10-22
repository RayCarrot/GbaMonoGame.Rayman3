namespace GbaMonoGame.Rayman3;

public partial class Caterpillar
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    // TODO: Name remaining actions
    public enum Action
    {
        Action0 = 0,
        Action1 = 1,
        Move_Right = 2,
        Move_Left = 3,
        Move_Up = 4,
        Move_Down = 5,
        Action6 = 6,
        Action7 = 7,
        Action8 = 8,
        Action9 = 9,
        Action10 = 10,
        Action11 = 11,
        Action12 = 12,
    }
}