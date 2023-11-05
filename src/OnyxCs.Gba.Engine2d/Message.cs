namespace OnyxCs.Gba.Engine2d;

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
    LevelEnd = 1019,
    
    Murphy_NewMultiplayerLevel = 1049,
}