namespace OnyxCs.Gba.Engine2d;

// TODO: Since messages are implemented across different libraries maybe this should be constants instead of an enum?
public enum Message
{
    None = 0,

    // Object
    WakeUp = 100,
    Sleep = 101,
    Destroy = 102,
    Resurrect = 103,
    ResurrectWakeUp = 104,

    // Captor
    Captor_Trigger = 200,
    Captor_Trigger_Sound = 201,
    Captor_Trigger_None = 202,
    Captor_Trigger_SendMessageWithParam = 203,
    Captor_Trigger_SendMessageWithCaptorParam = 204,
    
    // Rayman 3
    CollectedYellowLum = 1009,
    CollectedRedLum = 1010,
    CollectedGreenLum = 1011,
    CollectedBlueLum = 1012,
    CollectedWhiteLum = 1013,
    CollectedUnusedLum = 1014, // Doesn't exist
    CollectedBigYellowLum = 1015,
    CollectedBigBlueLum = 1016,

    LevelEnd = 1019,

    Cam_1027 = 1027,

    LevelExit = 1031,

    Cam_1039 = 1039,
    Cam_1040 = 1040,
    
    Murphy_NewMultiplayerLevel = 1049,
}