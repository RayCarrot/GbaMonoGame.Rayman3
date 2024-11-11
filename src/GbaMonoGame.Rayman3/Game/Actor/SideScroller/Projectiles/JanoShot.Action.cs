namespace GbaMonoGame.Rayman3;

public partial class JanoShot
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
        Hit = 2,
        Move_Down = 3,
    }
}