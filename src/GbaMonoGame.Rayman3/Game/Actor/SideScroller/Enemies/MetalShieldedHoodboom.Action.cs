namespace GbaMonoGame.Rayman3;

public partial class MetalShieldedHoodboom
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
        Attack_Left = 2,
        Attack_Right = 3,
        GuardAbove_Left = 4,
        GuardAbove_Right = 5,
        GuardBelow_Left = 6,
        GuardBelow_Right = 7,
        Dying_Left = 8,
        Dying_Right = 9,
    }
}