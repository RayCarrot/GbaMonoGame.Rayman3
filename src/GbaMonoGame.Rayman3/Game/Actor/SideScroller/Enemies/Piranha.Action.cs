namespace GbaMonoGame.Rayman3;

public partial class Piranha
{
    private new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    private enum Action
    {
        Move_Right = 0,
        Move_Left = 1,
        Dying1_Right = 2,
        Dying1_Left = 3,
        Dying2_Right = 4,
        Dying2_Left = 5,
    }
}