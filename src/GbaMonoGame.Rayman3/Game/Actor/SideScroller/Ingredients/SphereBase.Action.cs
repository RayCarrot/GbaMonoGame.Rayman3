namespace GbaMonoGame.Rayman3;

public partial class SphereBase
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Init_Yellow_Resurrect = 0,
        Init_Purple_Resurrect = 1,
        Init_Yellow_OpenGate = 2,
        Init_Purple_OpenGate = 3,
        Idle = 0,
        Activating = 4,
        Activated = 5,
    }
}