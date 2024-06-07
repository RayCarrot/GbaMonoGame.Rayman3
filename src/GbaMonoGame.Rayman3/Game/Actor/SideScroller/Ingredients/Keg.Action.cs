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
        // TODO: Name the remaining ones
        Idle = 0,
        Fall = 1,
        ThrownUp = 2,
        ThrownForward_Right = 3,
        ThrownForward_Left = 4,
        Action5 = 5,
        Action6 = 6,
        Action7 = 7,
        Action8 = 8,
        Action9 = 9,
        Action10 = 10,
        WaitToFall = 11,
        Respawn = 12,
        Action13 = 13,
        Action14 = 14,
        Action15 = 15,
        Drop = 16,
        Action17 = 17,
        Action18 = 18,
    }
}