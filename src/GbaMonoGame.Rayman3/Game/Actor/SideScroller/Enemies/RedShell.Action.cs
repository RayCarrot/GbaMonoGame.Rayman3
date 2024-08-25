namespace GbaMonoGame.Rayman3;

public partial class RedShell
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        FlyIntoSpace_Right = 0, // ???
        FlyIntoSpace_Left = 1, // ???
        WaitingToCharge_Right = 2,
        WaitingToCharge_Left = 3,
        Sleep_Right = 4,
        Sleep_Left = 5,
        ChargeAttack_Right = 6,
        ChargeAttack_Left = 7,
        PrepareChargeAttack_Right = 8,
        PrepareChargeAttack_Left = 9,
        Walk_Right = 10,
        Walk_Left = 11,
        WakeUp_Right = 12,
        WakeUp_Left = 13,
        Action_14 = 14, // Unused
        Action_15 = 15, // Unused
    }
}