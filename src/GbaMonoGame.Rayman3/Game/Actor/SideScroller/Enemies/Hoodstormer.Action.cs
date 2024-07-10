namespace GbaMonoGame.Rayman3;

public partial class Hoodstormer
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Idle_Left = 0,
        Idle_Right = 1,
        Fly_Left = 2,
        Fly_Right = 3,
        Taunt_Left = 4, // Unused
        Taunt_Right = 5, // Unused
        Shoot_Left = 6,
        Shoot_Right = 7,
        FlyAway_Left = 8,
        FlyAway_Right = 9,
        Dying_Left = 10,
        Dying_Right = 11,
    }
}