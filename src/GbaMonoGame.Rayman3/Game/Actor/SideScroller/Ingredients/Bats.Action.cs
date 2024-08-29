namespace GbaMonoGame.Rayman3;

public partial class Bats
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Fly_HorizontallyStart1 = 0,
        Fly_VerticallyStart = 1,
        Fly_HorizontallyStart2 = 2,
        Stationary_Flap = 3,
        Fly_Wait = 4,
        Stationary_Wait = 5,
        Fly_Horizontally = 6,
        Fly_Vertically = 7,
    }
}