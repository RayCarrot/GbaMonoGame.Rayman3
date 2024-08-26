namespace GbaMonoGame.Rayman3;

public partial class SilverPirate
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
        Jump_Left = 2,
        Jump_Right = 3,
        ShootHigh_Left = 4,
        ShootHigh_Right = 5,
        ShootLow_Left = 6,
        ShootLow_Right = 7,
        Hit_Left = 8,
        Hit_Right = 9,
        HitBehind_Left = 10,
        HitBehind_Right = 11,
        Dying_Left = 12,
        Dying_Right = 13,
        DyingBehind_Left = 14,
        DyingBehind_Right = 15,
        HitKnockBack1_Left = 16,
        HitKnockBack1_Right = 17,
        Init_HasRedLum_Left = 18,
        Init_HasRedLum_Right = 19,
        HitKnockBack2_Left = 20,
        HitKnockBack2_Right = 21,
        Fall_Left = 22,
        Fall_Right = 23,
        Land_Left = 24,
        Land_Right = 25,
    }
}