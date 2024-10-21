namespace GbaMonoGame.Rayman3;

public partial class ZombieChicken
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        MoveDown_Right = 0,
        MoveDown_Left = 1,
        MoveUp_Right = 2,
        MoveUp_Left = 3,
        DyingBack_Right = 4,
        DyingBack_Left = 5,
        Attack_Right = 6,
        Attack_Left = 7,
        TurnAround_Right = 8,
        TurnAround_Left = 9,
        Idle_Right = 10,
        Idle_Left = 11,
        DyingFront_Right = 12,
        DyingFront_Left = 13,
    }
}