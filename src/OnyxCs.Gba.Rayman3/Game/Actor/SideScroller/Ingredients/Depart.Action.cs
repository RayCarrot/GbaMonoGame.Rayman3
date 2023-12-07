namespace OnyxCs.Gba.Rayman3;

public partial class Depart
{
    private new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    private enum Action
    {
        ExitLevel = 0,
        EndLevel = 1,
    }
}