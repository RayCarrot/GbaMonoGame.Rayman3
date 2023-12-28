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

    // Rayman 3
    Enemy_Left = 34,
    Enemy_Right = 35,
    Enemy_Up = 36,
    Enemy_Down = 37,

    MovingPlatform_FullStop = 36,
    MovingPlatform_Stop = 37,
    MovingPlatform_Left = 38,
    MovingPlatform_Right = 39,
    MovingPlatform_Up = 40,
    MovingPlatform_Down = 41,
    MovingPlatform_DownLeft = 42,
    MovingPlatform_DownRight = 43,
    MovingPlatform_UpRight = 44,
    MovingPlatform_UpLeft = 45,

    Climb = 47,

    ClimbSpider1 = 51,
    ClimbSpider2 = 52,
    ClimbSpider3 = 53,
    ClimbSpider4 = 54,

    MovingPlatform_CounterClockwise45 = 81,
    MovingPlatform_CounterClockwise90 = 82,
    MovingPlatform_CounterClockwise135 = 83, // TODO: Add to collision tile set
    MovingPlatform_CounterClockwise180 = 84, // TODO: Add to collision tile set
    MovingPlatform_CounterClockwise225 = 85, // TODO: Add to collision tile set
    MovingPlatform_CounterClockwise270 = 86,
    MovingPlatform_CounterClockwise315 = 87,

    None = 0xFF,
}