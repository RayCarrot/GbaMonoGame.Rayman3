﻿namespace GbaMonoGame.Rayman3;

public partial class Rayman
{
    public new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    // TODO: Fill out remaining actions, 223 in total
    public enum Action
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
        Hang_EndMove_Right = 62,
        Hang_EndMove_Left = 63,

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
        BodyShot_Right = 78,
        BodyShot_Left = 79,
        Climb_Side_Right = 80,
        Climb_Side_Left = 81,
        Climb_Up_Right = 82,
        Climb_Up_Left = 83,
        Climb_Down_Right = 84,
        Climb_Down_Left = 85,
        PickUpObject_Right = 86,
        PickUpObject_Left = 87,
        CatchObject_Right = 88,
        CatchObject_Left = 89,
        ThrowObjectUp_Right = 90,
        ThrowObjectUp_Left = 91,
        ThrowObjectForward_Right = 92,
        ThrowObjectForward_Left = 93,
        CarryObject_Right = 94,
        CarryObject_Left = 95,
        WalkWithObject_Right = 96,
        WalkWithObject_Left = 97,

        Damage_Knockback_Right = 100,
        Damage_Knockback_Left = 101,
        WallJump_Fall = 102,
        WallJump_IdleStill = 103,
        WallJump_Jump = 104,
        WallJump_Move = 105,
        WallJump_Idle = 106,
        BeginBounce_Right = 107,
        BeginBounce_Left = 108,
        BouncyJump_Right = 109,
        BouncyJump_Left = 110,
        FlyForwardWithKeg_Right = 111,
        FlyForwardWithKeg_Left = 112,
        FlyBackwardsWithKeg_Right = 113,
        FlyBackwardsWithKeg_Left = 114,
        KnockbackBackwards_Right = 115,
        KnockbackBackwards_Left = 116,
        KnockbackForwards_Right = 117,
        KnockbackForwards_Left = 118,
        UnknownKnockback_Right = 119, // TODO: Unused?
        UnknownKnockback_Left = 120,
        Swing = 121,
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

        Idle_Yoyo_Right = 152,
        Idle_Yoyo_Left = 153,

        Idle_ChewingGum_Right = 156,
        Idle_ChewingGum_Left = 157,
        Dying_Right = 158,
        Dying_Left = 159,

        ChargeSuperFist_Right = 167,
        ChargeSuperFist_Left = 168,
        ChargeSecondSuperFist_Right = 169,
        ChargeSecondSuperFist_Left = 170,

        Idle_BasketBall_Right = 173,
        Idle_BasketBall_Left = 174,
        NewPower_Right = 175,
        NewPower_Left = 176,
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
        BouncyHelico_Right = 189,
        BouncyHelico_Left = 190,
        EnterCurtain_Right = 191,
        EnterCurtain_Left = 192,
        Idle_LookAround_Right = 193,
        Idle_LookAround_Left = 194,
        Walk_LookAround_Right = 195,
        Walk_LookAround_Left = 196,
        Idle_Shout_Right = 197,
        Idle_Shout_Left = 198,
        Idle_ReadyToFight_Right = 199,
        Idle_ReadyToFight_Left = 200,
        Idle_Determined_Right = 201,
        Idle_Determined_Left = 202,
        Idle_BeginCutscene_Right = 203,
        Idle_BeginCutscene_Left = 204,
        Idle_Cutscene_Right = 205,
        Idle_Cutscene_Left = 206,

        SmallKnockbackBackwards_Right = 209,
        SmallKnockbackBackwards_Left = 210,
        SmallKnockbackForwards_Right = 211,
        SmallKnockbackForwards_Left = 212,
        LookUp_Right = 213,
        LookUp_Left = 214,
        Damage_Shock_Right = 215,
        Damage_Shock_Left = 216,
        Spawn_Curtain_Right = 217,
        Spawn_Curtain_Left = 218,
        ReturnFromLevel_Right = 219,
        ReturnFromLevel_Left = 220,
        LockedLevelCurtain_Right = 221,
        LockedLevelCurtain_Left = 222,
    }
}