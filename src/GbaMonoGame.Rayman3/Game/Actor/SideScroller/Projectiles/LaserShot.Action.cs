namespace GbaMonoGame.Rayman3;

public partial class LaserShot
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Shot1_Right = 0,
        Shot1_Left = 1,
        Shot2_Right = 2,
        Shot2_Left = 3,
        Shot3_Right = 4,
        Shot4_Left = 5,

        // TODO: Name
        Action_6 = 6,
        Action_7 = 7,

        Shot1Enemy_Right = 8,
        Shot1Enemy_Left = 9,
    }
}