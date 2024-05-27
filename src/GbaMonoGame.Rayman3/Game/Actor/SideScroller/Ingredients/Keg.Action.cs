namespace GbaMonoGame.Rayman3;

public partial class Keg
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        // TODO: Name the remaining ones
        Action0 = 0,
        Fall = 1,
        Action2 = 2,
        Action3 = 3,
        Action4 = 4,
        Action5 = 5,
        Action6 = 6,
        Action7 = 7,
        Action8 = 8,
        Action9 = 9,
        Action10 = 10,
        WaitToFall = 11,
        Action12 = 12,
        Action13 = 13,
        Action14 = 14,
        Action15 = 15,
        Action16 = 16,
        Action17 = 17,
        Action18 = 18,
    }
}