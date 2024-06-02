namespace GbaMonoGame.Rayman3;

public partial class Ly
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        IdleWaiting = 0,
        Leave = 1,
        Talk1 = 2,
        Talk2 = 3,
        Talk3 = 4,
        Talk4 = 5,
        IdleActive = 6,
        BeginTalk = 7,
        GivePower1 = 8,
        GivePower2 = 9,
        GivePower3 = 10,
        GivePower4 = 11,
        GivePower5 = 12,
    }
}