namespace GbaMonoGame.Rayman3;

public partial class Machine
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        CogWheel1 = 0,
        CogWheel2 = 1,
        CogWheel3 = 2,
        CannonIdle1 = 3,
        CannonFire = 4,
        CannonBeginFire = 5,
        CannonIdle2 = 6, // Same as Idle1 - why a different action?
    }
}