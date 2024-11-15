namespace GbaMonoGame.Rayman3;

public partial class SpikyBag
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Stationary = 0,
        Swing = 1,
        BeginSwing = 2,
    }
}