namespace OnyxCs.Gba.Rayman3;

public partial class Lums
{
    private new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    private enum Action
    {
        YellowLum = 0,
        RedLum = 1,
        GreenLum = 2,
        BlueLum = 3,
        WhiteLum = 4,
        // 5 doesn't exist
        BigYellowLum = 6, // Unused
        BigBlueLum = 7,
    }
}