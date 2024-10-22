namespace GbaMonoGame.Rayman3;

public partial class Caterpillar
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Falling_Right = 0,
        Dying = 1,
        Move_Right = 2,
        Move_Left = 3,
        Move_Up = 4,
        Move_Down = 5,
        PickedUp = 6,
        ThrownUp = 7,
        ThrownForward_Right = 8,
        ThrownForward_Left = 9,
        Falling_Left = 10,
        KnockedDown_Right = 11,
        KnockedDown_Left = 12,
    }
}