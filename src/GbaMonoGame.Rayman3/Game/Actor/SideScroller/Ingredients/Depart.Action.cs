namespace GbaMonoGame.Rayman3;

public partial class Depart
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        ExitLevel = 0,
        EndLevel = 1,
    }
}