namespace OnyxCs.Gba.Rayman3;

public partial class Rayman
{
    private new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    // TODO: Fill out remaining actions, 223 in total
    private enum Action
    {
        Idle_Right = 0,
        Idle_Left = 1,
        Walk_Right = 2,
        Walk_Left = 3,
        Idle_ThrowBody_Right = 4,
        Idle_ThrowBody_Left = 5,
        Crouch_Right = 6,
        Crouch_Left = 7,
        Crawl_Right = 8,
        Crawl_Left = 9,
        Jump_Right = 10,
        Jump_Left = 11,
        Fall_Right = 12,
        Fall_Left = 13,
        Hang_Right = 14,
        Hang_Left = 15,
        ChargeFist_Right = 16,
        ChargeFist_Left = 17,
        Land_Right = 18,
        Land_Left = 19,

        ChargeFistVariant_Right = 36,
        ChargeFistVariant_Left = 37,

        Victory_Right = 50,
        Victory_Left = 51,

        Sliding_Fast_Right = 66,
        Sliding_Fast_Left = 67,
        Sliding_Jump_Right = 68,
        Sliding_Jump_Left = 69,
        Sliding_Land_Right = 70,
        Sliding_Land_Left = 71,
        Spawn_Right = 72, // Unused
        Spawn_Left = 73,
        Sliding_Slow_Right = 74,
        Sliding_Slow_Left = 75,
        Sliding_Crouch_Right = 76,
        Sliding_Crouch_Left = 77,

        Damage_Knockback_Right = 100,
        Damage_Knockback_Left = 101,

        UnknownJump_Right = 109, // TODO: What is this?
        UnknownJump_Left = 110,

        CrouchDown_Right = 130,
        CrouchDown_Left = 131,

        Idle_SpinBody_Right = 142,
        Idle_SpinBody_Left = 143,

        Idle_Bored_Right = 146,
        Idle_Bored_Left = 147,

        Idle_BasketBall_Right = 173,
        Idle_BasketBall_Left = 174,

        Idle_Grimace_Right = 177,
        Idle_Grimace_Left = 178,
        Walk_Multiplayer_Right = 179,
        Walk_Multiplayer_Left = 180,

        Hidden_Right = 187,
        Hidden_Left = 188,

        EnterCurtain_Right = 193,
        EnterCurtain_Left = 194,
        Walk_LookAround_Right = 195,
        Walk_LookAround_Left = 196,
        Idle_Shout_Right = 197,
        Idle_Shout_Left = 198,
        Idle_ReadyToFight_Right = 199,
        Idle_ReadyToFight_Left = 200,
        Idle_Determined_Right = 201,
        Idle_Determined_Left = 202,

        LookUp_Right = 213,
        LookUp_Left = 214,
        Damage_Shock_Right = 215,
        Damage_Shock_Left = 216,
        Spawn_Curtain_Right = 217,
        Spawn_Curtain_Left = 218,
        ReturnFromLevel_Right = 219,
        ReturnFromLevel_Left = 220,
    }
}