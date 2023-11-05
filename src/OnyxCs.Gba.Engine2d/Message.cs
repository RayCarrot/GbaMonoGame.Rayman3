namespace OnyxCs.Gba.Engine2d;

public enum Message
{
    None = 0,

    // Object
    SetFlag1 = 100,
    ClearFlag1 = 101,
    Disable = 102,
    Enable = 103,
    Spawn = 104,

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