namespace GbaMonoGame.Rayman3;

public partial class Teensies
{
    private new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    private enum Action
    {
        Init_Victory_Left = 0,
        Init_Victory_Right = 1,
        Victory1_Left = 2,
        Victory1_Right = 3,
        Victory2_Left = 4,
        Victory2_Right = 5,

        // TODO: 6-17

        Init_Master_Left = 18,
        Init_Master_Right = 19,
        Master1_Left = 20,
        Master1_Right = 21,
        Master2_Left = 22,
        Master2_Right = 23,
        Master3_Left = 24,
        Master3_Right = 25,
        Master4_Left = 26,
        Master4_Right = 27,
        Master5_Left = 28,
        Master6_Right = 29,

        Init_World1_Left = 30,
        Init_World1_Right = 31,
        Init_World2_Left = 32,
        Init_World2_Right = 33,
        Init_World3_Left = 34,
        Init_World3_Right = 35,
        Init_World4_Left = 36,
        Init_World4_Right = 37,

        // TODO: 38-41
    }
}