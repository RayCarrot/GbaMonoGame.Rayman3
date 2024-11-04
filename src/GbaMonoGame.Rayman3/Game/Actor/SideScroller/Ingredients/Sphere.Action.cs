namespace GbaMonoGame.Rayman3;

public partial class Sphere
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Init_Yellow = 0,
        Init_Purple = 1,
        Idle = 0,
        Respawn = 1,
        Drop = 2,
        ThrownForward_Right = 3,
        ThrownForward_Left = 4,
        ThrownUp = 5,
        Land_Right = 6,
        Land_Left = 7,
        Land = 8,
    }
}