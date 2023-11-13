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
    SolidAngle90Right = 16, // Unused
    SolidAngle90Left = 17, // Unused
    SolidAngle30Right1 = 18,
    SolidAngle30Right2 = 19,
    SolidAngle30Left2 = 20,
    SolidAngle30Left1 = 21,
    SlipperyAngle30Right1 = 22,
    SlipperyAngle30Right2 = 23,
    SlipperyAngle30Left2 = 24,
    SlipperyAngle30Left1 = 25,

    None = 0xFF,
}