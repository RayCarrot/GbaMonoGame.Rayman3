namespace GbaMonoGame.Rayman3;

public partial class Switch
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Deactivated = 0,
        Activating = 1,
        Activated = 2,
        Deactivating = 3,
    }
}