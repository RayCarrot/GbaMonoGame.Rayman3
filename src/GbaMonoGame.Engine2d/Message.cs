namespace GbaMonoGame.Engine2d;

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
    RaymanBody_FinishedAttack = 1002,
    Main_LinkMovement = 1003,
    Main_UnlinkMovement = 1004,

    Main_CollectedYellowLum = 1009,
    Main_CollectedRedLum = 1010,
    Main_CollectedGreenLum = 1011,
    Main_CollectedBlueLum = 1012,
    Main_CollectedWhiteLum = 1013,
    Main_CollectedUnusedLum = 1014, // Doesn't exist
    Main_CollectedBigYellowLum = 1015,
    Main_CollectedBigBlueLum = 1016,

    Main_LevelEnd = 1019,

    Cam_1027 = 1027, // TODO: Name

    Main_LevelExit = 1031,

    Main_CollectedCage = 1033,
    FlowerFire_End = 1034,

    Cam_1039 = 1039, // TODO: Name
    Cam_1040 = 1040, // TODO: Name

    Main_BeginSwing = 1044,

    Main_AllowCoyoteJump = 1048,
    Murfy_Spawn = 1049,
    Hit = 1050,

    Main_Stop = 1057,

    Main_ExitCutscene = 1059,

    Main_EnterLevelCurtain = 1081,
    Main_BeginInFrontOfLevelCurtain = 1082,
    Main_EndInFrontOfLevelCurtain = 1083,

    Main_EnterCutscene = 1088,

    Main_LockedLevelCurtain = 1093,
}