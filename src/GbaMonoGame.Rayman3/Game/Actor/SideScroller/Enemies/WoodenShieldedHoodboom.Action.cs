namespace GbaMonoGame.Rayman3;

public partial class WoodenShieldedHoodboom
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        ShieldedIdle_Left = 0,
        ShieldedIdle_Right = 1,
        Idle_Left = 2,
        Idle_Right = 3,
        Taunt_Left = 4,
        Taunt_Right = 5,
        Dying_Left = 6,
        Dying_Right = 7,
        ShieldedThrowGrenade_Left = 8,
        ShieldedThrowGrenade_Right = 9,
        ThrowGrenade_Left = 10,
        ThrowGrenade_Right = 11,
        ShieldedHitShield_Left = 12,
        ShieldedHitShield_Right = 13,
        ShieldedBreakShield_Left = 14,
        ShieldedBreakShield_Right = 15,
        Dizzy_Left = 16,
        Dizzy_Right = 17,
    }
}