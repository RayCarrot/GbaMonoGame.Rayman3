namespace GbaMonoGame.Rayman3;

public partial class Slapdash
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
        Walk_Left = 2,
        Walk_Right = 3,
        TurnAround_Left = 4,
        TurnAround_Right = 5,
        BeginChargeAttack_Left = 6,
        BeginChargeAttack_Right = 7,
        ChargeAttack_Left = 8,
        ChargeAttack_Right = 9,
        TurnAroundFromChargeAttack_Left = 10,
        TurnAroundFromChargeAttack_Right = 11,
        Hit_Left = 12,
        Hit_Right = 13,
        Dying_Left = 14,
        Dying_Right = 15,
    }
}