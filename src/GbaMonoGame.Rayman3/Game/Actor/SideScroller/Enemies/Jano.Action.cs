namespace GbaMonoGame.Rayman3;

public partial class Jano
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Idle_Left = 0,
        Attack_Left = 1,
        Grimace_Left = 2,
        Grimace_Right = 3,
        Complete = 4,
        CreateSkullPlatform2 = 5,
        TurnAroundSlow_Left = 6,
        TurnAroundFast_Left = 7,
        Action8 = 8, // Unused
        CreateSkullPlatform1 = 9,
        CreateSkullPlatform4 = 10,
        Attack_Right = 11,
        Move_Right = 12,
        Move_Left = 13,
        Idle_Right = 14,
        TurnAroundSlow_Right = 15,
        TurnAroundFast_Right = 16,
        CreateSkullPlatform3 = 17,
    }
}