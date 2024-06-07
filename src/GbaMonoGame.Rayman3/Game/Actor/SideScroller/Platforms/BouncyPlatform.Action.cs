namespace GbaMonoGame.Rayman3;

public partial class BouncyPlatform
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Idle = 0,
        BeginTrap = 1,
        Bounce = 2,
        EndTrap = 3,
        Trap = 4,
    }
}