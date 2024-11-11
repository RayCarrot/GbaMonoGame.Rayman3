namespace GbaMonoGame.Rayman3;

public partial class Jano
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    // TODO: Name
    public enum Action
    {
        Idle_Left = 0,
        Attack_Left = 1,
        Grimace_Left = 2,
        Grimace_Right = 3,
        Action4 = 4,
        Action5 = 5,
        TurnAroundSlow_Left = 6,
        TurnAroundFast_Left = 7,
        Action8 = 8,
        Action9 = 9,
        Action10 = 10,
        Attack_Right = 11,
        Move_Right = 12,
        Move_Left = 13,
        Idle_Right = 14,
        TurnAroundSlow_Right = 15,
        TurnAroundFast_Right = 16,
        Action17 = 17,
    }
}