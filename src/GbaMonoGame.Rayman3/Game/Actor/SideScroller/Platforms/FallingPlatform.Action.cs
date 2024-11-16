namespace GbaMonoGame.Rayman3;

public partial class FallingPlatform
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    public enum Action
    {
        Idle = 0,
        Shake = 1,
        BeginFall = 2,
        Action3 = 3,
        Action4 = 4, // Unused
    }
}