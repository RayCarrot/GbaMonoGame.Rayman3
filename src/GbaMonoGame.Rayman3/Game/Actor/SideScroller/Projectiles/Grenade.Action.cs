namespace GbaMonoGame.Rayman3;

public partial class Grenade
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        LongThrow_Left = 0,
        LongThrow_Right = 1,
        ShortThrow_Left = 2,
        ShortThrow_Right = 3,
        NormalThrow_Left = 4,
        NormalThrow_Right = 5,
    }
}