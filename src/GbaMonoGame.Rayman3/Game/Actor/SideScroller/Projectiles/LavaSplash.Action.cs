namespace GbaMonoGame.Rayman3;

public partial class LavaSplash
{
    private new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    private enum Action
    {
        Idle = 0,
        PlumLandedSplash = 1,
        Moving = 2,
        MainActorDrownSplash = 3,
    }
}