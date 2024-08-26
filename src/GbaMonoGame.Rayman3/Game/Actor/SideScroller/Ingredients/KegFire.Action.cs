namespace GbaMonoGame.Rayman3;

public partial class KegFire
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Default_Right = 0,
        Default_Left = 1,
    }
}