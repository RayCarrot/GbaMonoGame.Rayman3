namespace GbaMonoGame.Rayman3;

public partial class Arrive
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Idle = 0,
        EndingLevel = 1,
        EndedLevel = 2,
    }
}