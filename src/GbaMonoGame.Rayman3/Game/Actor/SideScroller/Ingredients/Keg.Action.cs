namespace GbaMonoGame.Rayman3;

public partial class Keg
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Idle = 0,
        Fall = 1,
        ThrownUp = 2,
        ThrownForward_Right = 3,
        ThrownForward_Left = 4,
        Ignite_Right = 5,
        Ignite_Left = 6,
        Flying_Right = 7,
        Flying_Left = 8,
        StopFlying_Right = 9,
        StopFlying_Left = 10,
        WaitToFall = 11,
        Respawn = 12,
        FallFromFlight_Right = 13,
        FallFromFlight_Left = 14,
        Action15 = 15, // Unused
        Drop = 16,
        EjectFromDispenser = 17,
        Bounce = 18,
    }
}