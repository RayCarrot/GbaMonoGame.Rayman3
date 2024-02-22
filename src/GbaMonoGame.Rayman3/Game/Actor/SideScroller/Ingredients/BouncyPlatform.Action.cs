namespace GbaMonoGame.Rayman3;

public partial class BouncyPlatform
{
    private new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    private enum Action
    {
        Idle = 0,
        BeginTrap = 1,
        Bounce = 2,
        EndTrap = 3,
        Trap = 4,
    }
}