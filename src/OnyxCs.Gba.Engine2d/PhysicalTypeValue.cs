namespace OnyxCs.Gba.Engine2d;

public enum PhysicalTypeValue : byte
{
    // Fully solid
    Solid = 0,
    Slippery = 1,
    Ledge = 2,
    SlipperyLedge = 3,
    Passthrough = 15,

    // Angled solid
    SolidAngleLeft1 = 18,
    SolidAngleLeft2 = 19,
    SolidAngleRight2 = 20,
    SolidAngleRight1 = 21,
    SlipperyAngleLeft1 = 22,
    SlipperyAngleLeft2 = 23,
    SlipperyAngleRight2 = 24,
    SlipperyAngleRight1 = 25,

    None = 0xFF,
}