namespace GbaMonoGame.Rayman3;

public partial class MechanicalPlatform
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Idle_Right = 0,
        Idle_Left = 1,
        SoftHit_Right = 2,
        SoftHit_Left = 3,
        HardHit_Right = 4,
        HardHit_Left = 5,
    }
}