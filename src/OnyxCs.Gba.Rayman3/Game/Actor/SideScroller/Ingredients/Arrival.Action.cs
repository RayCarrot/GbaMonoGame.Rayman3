namespace OnyxCs.Gba.Rayman3;

public partial class Arrive
{
    private new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    private enum Action
    {
        Idle = 0,
        EndingLevel = 1,
        EndedLevel = 2,
    }
}