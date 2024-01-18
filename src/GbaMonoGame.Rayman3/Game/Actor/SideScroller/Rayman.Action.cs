﻿namespace GbaMonoGame.Rayman3;

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
        HangOnEdge_Idle_Right = 14,
        HangOnEdge_Idle_Left = 15,
        ChargeFist_Right = 16,
        ChargeFist_Left = 17,
        Land_Right = 18,
        Land_Left = 19,
        ThrowFistInAir_Right = 20,
        ThrowFistInAir_Left = 21,
        HangOnEdge_Begin_Right = 22,
        HangOnEdge_Begin_Left = 23,
        EndChargeFist_Right = 24,
        EndChargeFist_Left = 25,
        BeginChargeFist_Right = 26,
        BeginChargeFist_Left = 27,
        Helico_Right = 28,
        Helico_Left = 29,
        BeginThrowFistInAir_Right = 30,
        BeginThrowFistInAir_Left = 31,
        BeginThrowSecondFistInAir_Right = 32,
        BeginThrowSecondFistInAir_Left = 33,
        BeginChargeSecondFist_Right = 34,
        BeginChargeSecondFist_Left = 35,
        ChargeSecondFist_Right = 36,
        ChargeSecondFist_Left = 37,
        EndChargeSecondFist_Right = 38,
        EndChargeSecondFist_Left = 39,
        Drown_Right = 40,
        Drown_Left = 41,
        NearEdgeBehind_Right = 42,
        NearEdgeBehind_Left = 43,
        NearEdgeFront_Right = 44,
        NearEdgeFront_Left = 45,
        Damage_Hit_Right = 46,
        Damage_Hit_Left = 47,

        Victory_Right = 50,
        Victory_Left = 51,
        Hang_Move_Right = 52,
        Hang_Move_Left = 53,
        Hang_Idle_Right = 54,
        Hang_Idle_Left = 55,
        Hang_ChargeAttack_Right = 56,
        Hang_ChargeAttack_Left = 57,
        Hang_Attack_Right = 58,
        Hang_Attack_Left = 59,
        Hang_BeginMove_Right = 60,
        Hang_BeginMove_Left = 61,
        Hang_BeginIdle_Right = 62,
        Hang_BeginIdle_Left = 63,

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

        Climb_Side_Right = 80,
        Climb_Side_Left = 81,
        Climb_Up_Right = 82,
        Climb_Up_Left = 83,
        Climb_Down_Right = 84,
        Climb_Down_Left = 85,

        Damage_Knockback_Right = 100,
        Damage_Knockback_Left = 101,

        UnknownJump_Right = 109, // TODO: What is this?
        UnknownJump_Left = 110,

        HangOnEdge_ChargeAttack_Right = 122,
        HangOnEdge_ChargeAttack_Left = 123,
        HangOnEdge_EndAttack_Right = 124,
        HangOnEdge_EndAttack_Left = 125,

        CrouchDown_Right = 130,
        CrouchDown_Left = 131,
        Climb_Idle_Right = 132,
        Climb_Idle_Left = 133,
        Climb_BeginIdle_Right = 134,
        Climb_BeginIdle_Left = 135,
        Climb_ChargeFist_Right = 136,
        Climb_ChargeFist_Left = 137,
        Climb_BeginChargeFist_Right = 138,
        Climb_BeginChargeFist_Left = 139,
        Climb_EndChargeFist_Right = 140,
        Climb_EndChargeFist_Left = 141,
        Idle_SpinBody_Right = 142,
        Idle_SpinBody_Left = 143,
        HangOnEdge_BeginAttack_Right = 144,
        HangOnEdge_BeginAttack_Left = 145,
        Idle_Bored_Right = 146,
        Idle_Bored_Left = 147,

        ChargeSuperFist_Right = 167,
        ChargeSuperFist_Left = 168,
        ChargeSecondSuperFist_Right = 169,
        ChargeSecondSuperFist_Left = 170,

        Idle_BasketBall_Right = 173,
        Idle_BasketBall_Left = 174,

        Idle_Grimace_Right = 177,
        Idle_Grimace_Left = 178,
        Walk_Multiplayer_Right = 179,
        Walk_Multiplayer_Left = 180,
        Climb_ChargeSuperFist_Right = 181,
        Climb_ChargeSuperFist_Left = 182,

        HelicoTimeout_Right = 185,
        HelicoTimeout_Left = 186,
        Hidden_Right = 187,
        Hidden_Left = 188,
        UnknownHelico_Right = 189, // TODO: What is this?
        UnknownHelico_Left = 190,

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