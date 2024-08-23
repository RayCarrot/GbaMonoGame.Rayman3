namespace GbaMonoGame.Rayman3;

public partial class Gate
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Closed_Left = 0,
        Closed_Right = 1,
        Open_Left = 2,
        Open_Right = 3,
        Opening_Left = 4,
        Opening_Right = 5,
        Init_4Switches_Left = 6,
        Init_4Switches_Right = 7,
        Init_3Switches_Left = 8,
        Init_3Switches_Right = 9,
        Closing_Left = 10,
    }
}