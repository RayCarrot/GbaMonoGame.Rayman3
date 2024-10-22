using System.Collections.Generic;
using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.Engine2d;
using GbaMonoGame.TgxEngine;
using Microsoft.Xna.Framework.Input;
using Game = BinarySerializer.Ubisoft.GbaEngine.Game;

namespace GbaMonoGame.Rayman3;

public class Rayman3 : GbaGame
{
    #region Protected Properties

    protected override Game Game => Game.Rayman3;
    protected override string Title => "Rayman 3";
    protected override Dictionary<int, string> GbaSongTable => new()
    {
        [6] = "Sfx/OnoExpir",
        [8] = "Sfx/OnoInspi",
        [9] = "Sfx/OnoJump1",
        [14] = "Sfx/OnoThrow_Mix02",
        [16] = "Sfx/OnoWin_Mix02",
        [18] = "Sfx/OnoWinRM_Mix02",
        [27] = "Sfx/SkiLoop1",
        [61] = "Sfx/MenuMove",
        [62] = "Sfx/MinHP",
        [80] = "Sfx/RireMumu_Mix03",
        [89] = "Music/canopy",
        [90] = "Music/baddreams",
        [91] = "Music/precipice",
        [92] = "Music/lyrace",
        [98] = "Music/jano",
        [99] = "Music/mountain1",
        [100] = "Music/polokus",
        [101] = "Music/rockchase",
        [102] = "Music/spiderchase",
        [103] = "Music/bigplatform",
        [104] = "Music/bigtrees",
        [105] = "Music/boss1",
        [106] = "Music/boss34",
        [107] = "Music/fairyglades",
        [108] = "Music/helico",
        [109] = "Music/rocket",
        [110] = "Music/waterski",
        [111] = "Music/woodlight",
        [114] = "Sfx/Bounce01_Mix02",
        [115] = "Sfx/FishJaw1_Mix02",
        [116] = "Sfx/CageSnd1_Mix02",
        [118] = "Sfx/RootIn_Mix01",
        [134] = "Music/barrel",
        [135] = "Music/echocave",
        [136] = "Music/mountain2",
        [139] = "Sfx/SlideOut_Mix01",
        [141] = "Sfx/SldGreen_SkiLoop1",
        [147] = "Music/ancients",
        [148] = "Music/finalboss",
        [149] = "Music/firestone",
        [150] = "Music/happyslide",
        [151] = "Music/lyfree",
        [152] = "Music/raytheme",
        [153] = "Music/ship",
        [154] = "Music/tag",
        [161] = "Sfx/GumBlow_Mix01",
        [162] = "Sfx/CageSnd2_Mix02",
        [171] = "Sfx/Switch1_Mix03",
        [177] = "Music/enemy1",
        [178] = "Music/enemy2",
        [179] = "Music/sadslide",
        [180] = "Music/barbeslide",
        [181] = "Music/tizetre",
        [182] = "Music/win1",
        [183] = "Music/win2",
        [184] = "Music/win3",
        [185] = "Music/death",
        [186] = "Music/lyfreeVOX4",
        [199] = "Sfx/SlideIn_Mix02",
        [205] = "Sfx/MetlGate_Mix01",
        [214] = "Sfx/Combust1_Mix02",
        [219] = "Sfx/BarlLeaf_SkiWeed_Mix02",
        [222] = "Sfx/WallSlid_Mix02",
        [224] = "Sfx/Grenad01_Mix03",
        [226] = "Sfx/VibraFLW_Mix02",
        [249] = "Sfx/WWallBrk_PF2Fall_Mix03",
        [253] = "Sfx/OnoJump3_Mix01",
        [254] = "Sfx/OnoJump4_Mix01",
        [255] = "Sfx/OnoJump5_Mix01",
        [256] = "Sfx/OnoJump6_Mix01",
        [257] = "Sfx/RayFist_Mix02",
        [261] = "Sfx/SuprFist_Mix01",
        [262] = "Sfx/LumMauve_Mix02",
        [268] = "Sfx/HeliCut_Mix01",
        [269] = "Sfx/Valid01_Mix01",
        [270] = "Sfx/Back01_Mix01",
        [275] = "Sfx/Helico01_Mix10",
        [280] = "Sfx/Laser1_Mix02",
        [281] = "Sfx/Laser2_Mix02",
        [285] = "Sfx/OnoPeur1_Mix03",
        [286] = "Sfx/BodyAtk1_Mix01",
        [294] = "Sfx/OnoGO_Mix02",
        [298] = "Sfx/BangGen1_Mix07",
        [300] = "Sfx/BigSplsh_SplshGen_Mix04",
        [302] = "Sfx/OnoEfor1_Mix02",
        [304] = "Sfx/Boing_Mix02",
        [318] = "Music/LumBoost_Mix01GEN_P1",
        [319] = "Music/LumBoost_Mix01GEN_P2",
        [320] = "Music/LumBoost_Mix01GEN_P3",
        [335] = "Sfx/Grimace1_Mix04",
        [336] = "Sfx/OnoWoHoo_Mix01",
        [337] = "Music/barrel_BA",
        [338] = "Sfx/MachAtk1_Mix01",
        [341] = "Sfx/FishDead_Mix02",
        [342] = "Music/rocket_BA",
        [344] = "Sfx/SkulShak_Mix01",
        [346] = "Sfx/HandTap2_Mix03",
        [351] = "Sfx/HorseCry_Mix02",
        [352] = "Sfx/BigFoot1_Mix02",
        [358] = "Sfx/YoyoMove_Mix02",
        [361] = "Sfx/CagoDie2_Mix01",
        [365] = "Sfx/BallInit_Mix01",
        [366] = "Sfx/MachMotr_Mix01",
        [367] = "Sfx/BombFly_Mix03",
        [370] = "Sfx/Charge2_Mix04",
        [375] = "Sfx/RootOut_Mix04",
        [376] = "Sfx/PF2Crac_Mix02",
        [380] = "Music/RootOut_Pitch",
        [381] = "Sfx/LumGreen_Mix04",
        [382] = "Sfx/NoCombus_Mix04",
        [386] = "Sfx/Bounce00_Mix03",
        [388] = "Sfx/PinBall_Mix02",
        [389] = "Music/PinBall_Low",
        [394] = "Sfx/RaySpin_Mix06",
        [395] = "Sfx/JanoShot_Mix01",
        [397] = "Sfx/ScalDead_Mix02",
        [398] = "Sfx/CaCombus_NoCombus_Mix04",
        [399] = "Sfx/MachAtk2_Mix02",
        [400] = "Sfx/LumSwing_Mix03",
        [401] = "Sfx/BallSwng_LumSwing_Mix03",
        [402] = "Sfx/GenWoosh_LumSwing_Mix03",
        [403] = "Sfx/PiraJump_BigFoot1_Mix02",
        [404] = "Sfx/BallImp1_BigFoot1_Mix02",
        [405] = "Sfx/BarlFall_Mix04",
        [406] = "Sfx/PiraHit1_Mix02",
        [407] = "Sfx/PiraHit3_Mix03",
        [408] = "Sfx/CageHit_Mix07",
        [409] = "Sfx/GumChew_Mix02",
        [410] = "Sfx/HeliStop_Mix06",
        [411] = "Sfx/Horror_Mix08",
        [412] = "Sfx/LumBleu_Mix02",
        [413] = "Sfx/LumSlvr_Mix02",
        [414] = "Sfx/MumuHit_SkullHit_Mix02",
        [417] = "Sfx/MumuWake_LumHit_Mix03",
        [419] = "Sfx/CaFlyDie_Mix03",
        [420] = "Sfx/CagouRit_Mix03",
        [421] = "Sfx/MetlImp1_PiraHit3_Mix03",
        [423] = "Sfx/Thunder1_Mix04",
        [426] = "Sfx/LumTimer_Mix02",
        [427] = "Sfx/BBQ_Mix10",
        [428] = "Sfx/Tag_Mix02",
        [431] = "Sfx/Motor01_Mix12",
        [432] = "Sfx/Laser4_Mix01",
        [433] = "Sfx/MetlTrap_MetlGate_Mix01",
        [436] = "Sfx/LumRed_Mix03",
        [438] = "Sfx/LumOrag_Mix06",
        [439] = "Sfx/GlueFoot_PlumSnd2_Mix02",
        [440] = "Sfx/Cloche01_Mix01",
        [444] = "Sfx/Janogrrr_Mix03",
        [447] = "Sfx/CagoWlk1_Mix02",
        [449] = "Sfx/Fire02_Laser4_Mix01",
        [451] = "Sfx/SkulInit_Mix04",
        [452] = "Sfx/SkullEnd_Mix02",
        [453] = "Sfx/SkullHit_Mix02",
        [454] = "Sfx/Chain_Mix01",
        [455] = "Sfx/CageTrsh_Mix05",
        [458] = "Sfx/Store01_Mix01",
        [460] = "Sfx/Pannel_BigFoot1_Mix02",
        [461] = "Sfx/CagOno01_Mix01",
        [462] = "Sfx/RocktLeg_Mix03",
        [463] = "Sfx/Bounce02_Mix03",
        [464] = "Sfx/PF2Fall_Mix03",
        [467] = "Sfx/JanoSkul_Mix03",
        [468] = "Sfx/LavaBubl_Mix02",
        [469] = "Sfx/LavaStrt_Mix04",
        [470] = "Sfx/LumAtk01_Mix02",
        [471] = "Sfx/LumAtk02_Mix01",
        [472] = "Sfx/LumTotal_Mix02",
        [473] = "Sfx/MumuDead_Mix04",
        [475] = "Sfx/OnoEfor2_Mix03",
        [476] = "Sfx/OnoEquil_Mix03",
        [477] = "Sfx/PF1Fall_PF2Fall_Mix03",
        [478] = "Sfx/PlumSnd1_Mix03",
        [480] = "Sfx/RaDeath_Mix03",
        [481] = "Sfx/SkiWeed_Mix02",
        [482] = "Sfx/SpherImp_Mix02",
        [483] = "Sfx/SpidrAtk_Mix02",
        [484] = "Sfx/SplshGen_Mix04",
        [488] = "Sfx/Electric_Mix02",
        [494] = "Sfx/SocleFX1_Mix01",
        [495] = "Sfx/LyVO1_Mix01",
        [496] = "Sfx/CagouHit_Mix03",
        [497] = "Sfx/ArmSwing_LumSwing_Mix03",
        [498] = "Sfx/BlobFX02_Mix02",
        [500] = "Sfx/ScaMorf1_Mix02",
        [501] = "Sfx/ScaMorf2_Mix02",
        [502] = "Sfx/ScaHurt2_Mix02",
        [504] = "Sfx/BossVO01_Mix01",
        [505] = "Sfx/BossVO02_Mix01",
        [506] = "Sfx/Appear_SocleFX1_Mix01",
        [507] = "Sfx/MurfHeli_Mix01",
        [508] = "Sfx/Curtain_YoyoMove_Mix02",
        [509] = "Sfx/ScalBong_PinBall_Mix02",
        [511] = "Sfx/LumVioNP_SkulShak_Mix01",
        [512] = "Sfx/PiraHurt_Mix02",
        [515] = "Sfx/PiraAtk1_Mix01",
        [519] = "Sfx/LineFX01_Mix02_P1_",
        [520] = "Sfx/LineFX01_Mix02_P2_",
        [523] = "Sfx/Charge_Mix05",
        [524] = "Sfx/JanoGrwl_Mix03",
        [525] = "Sfx/PlumSnd2_Mix03",
        [526] = "Sfx/WoodImp_Mix03",
        [527] = "Sfx/WoodBrk1_Mix04",
        [528] = "Sfx/RireCago_RireMumu_Mix03",
        [529] = "Sfx/Store02_Mix02",
        [530] = "Sfx/ScalFlee_Mix02",
        [534] = "Sfx/BossVO03_Mix01",
        [537] = "Sfx/CagoTurn_Mix03",
        [538] = "Sfx/RyVOGlob_Mix02",
        [540] = "Sfx/GateClos_MetlGate_Mix01",
        [541] = "Sfx/LyMagic1_Mix01",
        [543] = "Sfx/Sparkles_Mix01",
        [544] = "Sfx/MurfyVO1A_Mix01",
        [545] = "Sfx/MurfyVO1B_Mix01",
        [548] = "Sfx/MurfyVO3A_Mix01",
        [550] = "Sfx/MurfyVO4A_Mix01",
        [557] = "Sfx/GhstDead_Mix05",
        [558] = "Sfx/OnoRcvH1_Mix04",
        [559] = "Sfx/PiraDead_Mix05",
        [560] = "Music/tizetre_Swing",
        [561] = "Sfx/RayFist2_Mix01",
        [563] = "Sfx/CagoAttk_Mix03",
        [564] = "Sfx/RoktSpin_Mix03",
        [565] = "Sfx/Whistle1_Mix01",
        [566] = "Sfx/TiztrVO1_Mix01",
        [567] = "Sfx/TiztrVO2_Mix01",
        [568] = "Sfx/TiztrVO3_Mix01",
        [569] = "Sfx/TiztrVO4_Mix01",
        [571] = "Sfx/LightFX1_Mix01",
        [578] = "Sfx/Missile1_Mix01",
        [579] = "Sfx/PannelUp_Mix01",
        [580] = "Sfx/PannelDw_Mix01",
        [581] = "Music/Win_BOSS",
        [582] = "Sfx/ScalUp_Mix03",
        [584] = "Sfx/GameOver_BeepFX01_Mix02",
        [585] = "Sfx/LyMagic2_Mix07",
        [586] = "Sfx/JanoRire_Mix01",
        [587] = "Sfx/ScalHurt_Mix02",
        [588] = "Sfx/LumAtk03_LumAtk01_Mix02",
        [589] = "Sfx/LumAtk04_LumAtk02_Mix01",
        [590] = "Sfx/CountDwn_Mix07_P1_",
        [591] = "Sfx/CountDwn_Mix07_P2_",
        [592] = "Sfx/CountDwn_Mix07_P3_",
        [593] = "Sfx/Globox_Mix04",
        [594] = "Sfx/PathFX_Mix01",
        [595] = "Sfx/BeepFX01_Mix02",
        [596] = "Sfx/Spirale_Mix01",
        [597] = "Sfx/RyVOLy_Mix01",
        [598] = "Sfx/RyVO1_Mix01",
        [599] = "Sfx/RyVO2_Mix01",
        [600] = "Sfx/RyVO3_Mix01",
        [602] = "Sfx/BossHurt_Mix02",
        [603] = "Sfx/Spark2_Sparkles_Mix01",
        [604] = "Sfx/Spark3_Sparkles_Mix01",
        [605] = "Sfx/LumPeek_Mix01",
        [607] = "Sfx/Spark4_Sparkles_Mix01",
        [608] = "Sfx/RaySigh_Mix01",
        [609] = "Sfx/SnoreFX1_Mix01",
        [610] = "Sfx/SnoreFX2_SnoreFX1_Mix01",
        [611] = "Sfx/ScalGrrr_Mix02",
        [613] = "Sfx/Laser3_Mix03",
        [614] = "Sfx/HandTap1_Mix04",
        [615] = "Sfx/NewPower_Mix06",
        [616] = "Sfx/Curtain2_YoyoMove_Mix02",
        [618] = "Sfx/Propulse_Combust1_Mix02",
    };
    protected override Dictionary<int, string> NGageSongTable => new()
    {
        [190] = "Sfx/unnamed0", // Unknown name due to it being N-Gage exclusive - used for event 108
        [191] = "Sfx/unnamed1", // Unknown name due to it being N-Gage exclusive - used for new event 512
        [192] = "Sfx/unnamed2", // Unknown name due to it being N-Gage exclusive - used for new event 513
        [193] = "Sfx/unnamed3", // Unknown name due to it being N-Gage exclusive - used for new event 514
        [194] = "Sfx/ABA1",
        [195] = "Sfx/ABD2",
        [196] = "Sfx/Back01_Mix01",
        [197] = "Sfx/BallInit_Mix01",
        [198] = "Sfx/BangGen1_Mix07",
        [199] = "Sfx/BarlFall_Mix04",
        [200] = "Sfx/BBQ_Mix10",
        [201] = "Sfx/BCC1",
        [202] = "Sfx/BCD1",
        [203] = "Sfx/BDA",
        [204] = "Sfx/BDC2",
        [205] = "Sfx/BeepFX01_Mix02",
        [206] = "Sfx/BigFoot1_Mix02",
        [207] = "Sfx/BlobFX02_Mix02",
        [208] = "Sfx/BodyAtk1_Mix01",
        [209] = "Sfx/Boing_Mix02",
        [210] = "Sfx/BombFly_Mix03",
        [211] = "Sfx/BossHurt_Mix02",
        [212] = "Sfx/BossVO01_Mix01",
        [213] = "Sfx/BossVO02_Mix01",
        [214] = "Sfx/BossVO03_Mix01",
        [215] = "Sfx/Bounce00_Mix03",
        [216] = "Sfx/Bounce01_Mix02",
        [217] = "Sfx/Bounce02_Mix03",
        [218] = "Sfx/BSB1",
        [219] = "Sfx/BSNAd2",
        [220] = "Sfx/CaFlyDie_Mix03",
        [221] = "Sfx/CageHit_Mix07",
        [222] = "Sfx/CageSnd1_Mix02",
        [223] = "Sfx/CageSnd2_Mix02",
        [224] = "Sfx/CageTrsh_Mix05",
        [225] = "Sfx/CagoAttk_Mix03",
        [226] = "Sfx/CagoDie2_Mix01",
        [227] = "Sfx/CagOno01_Mix01",
        [228] = "Sfx/CagoTurn_Mix03",
        [229] = "Sfx/CagouHit_Mix03",
        [230] = "Sfx/CagouRit_Mix03",
        [231] = "Sfx/CagoWlk1_Mix02",
        [232] = "Sfx/CGAA2",
        [233] = "Sfx/CGAF3",
        [234] = "Sfx/CGAF4",
        [235] = "Sfx/Chain_Mix01",
        [236] = "Sfx/Charge2_Mix04",
        [237] = "Sfx/Charge_Mix05",
        [238] = "Sfx/CLAF4",
        [239] = "Sfx/Cloche01_Mix01",
        [240] = "Sfx/Combust1_Mix02",
        [241] = "Sfx/CountDwn_Mix07", // Not on GBA
        [242] = "Sfx/CYC4",
        [243] = "Sfx/CYG4",
        [244] = "Sfx/Electric_Mix02",
        [245] = "Sfx/ETC3",
        [246] = "Sfx/FishDead_Mix02",
        [247] = "Sfx/FishJaw1_Mix02",
        [248] = "Sfx/GhstDead_Mix05",
        [249] = "Sfx/Globox_Mix04",
        [250] = "Sfx/Grenad01_Mix03",
        [251] = "Sfx/Grimace1_Mix04",
        [252] = "Sfx/GumBlow_Mix01",
        [253] = "Sfx/GumChew_Mix02",
        [254] = "Sfx/HandTap1_Mix04",
        [255] = "Sfx/HandTap2_Mix03",
        [256] = "Sfx/Helico01_Mix10",
        [257] = "Sfx/HeliCut_Mix01",
        [258] = "Sfx/HeliStop_Mix06",
        [259] = "Sfx/HHCd5",
        [260] = "Sfx/HHDd5",
        [261] = "Sfx/Horror_Mix08",
        [262] = "Sfx/HorseCry_Mix02",
        [263] = "Sfx/INPA3",
        [264] = "Sfx/INPAd3",
        [265] = "Sfx/INPE2",
        [266] = "Sfx/INPF3",
        [267] = "Sfx/INPG3",
        [268] = "Sfx/INPGd4",
        [269] = "Sfx/Janogrrr_Mix03",
        [270] = "Sfx/JanoGrwl_Mix03",
        [271] = "Sfx/JanoRire_Mix01",
        [272] = "Sfx/JanoShot_Mix01",
        [273] = "Sfx/JanoSkul_Mix03",
        [274] = "Sfx/Laser1_Mix02",
        [275] = "Sfx/Laser2_Mix02",
        [276] = "Sfx/Laser3_Mix03",
        [277] = "Sfx/Laser4_Mix01",
        [278] = "Sfx/LavaBubl_Mix02",
        [279] = "Sfx/LavaStrt_Mix04",
        [280] = "Sfx/LightFX1_Mix01",
        [281] = "Sfx/LineFX01_Mix02", // Not on GBA
        [282] = "Sfx/LumAtk01_Mix02",
        [283] = "Sfx/LumAtk02_Mix01",
        [284] = "Sfx/LumBleu_Mix02",
        [285] = "Sfx/LumBoost_Mix01", // Not on GBA
        [286] = "Sfx/LumGreen_Mix04",
        [287] = "Sfx/LumHit_Mix03", // Not on GBA
        [288] = "Sfx/LumMauve_Mix02",
        [289] = "Sfx/LumOrag_Mix06",
        [290] = "Sfx/LumPeek_Mix01",
        [291] = "Sfx/LumRed_Mix03",
        [292] = "Sfx/LumSlvr_Mix02",
        [293] = "Sfx/LumSwing_Mix03",
        [294] = "Sfx/LumTimer_Mix02",
        [295] = "Sfx/LumTotal_Mix02",
        [296] = "Sfx/LyMagic1_Mix01",
        [297] = "Sfx/LyMagic2_Mix07",
        [298] = "Sfx/LyVO1_Mix01",
        [299] = "Sfx/MAC3",
        [300] = "Sfx/MachAtk1_Mix01",
        [301] = "Sfx/MachAtk2_Mix02",
        [302] = "Sfx/MachMotr_Mix01",
        [303] = "Sfx/MenuMove",
        [304] = "Sfx/MetlGate_Mix01",
        [305] = "Sfx/MinHP",
        [306] = "Sfx/Missile1_Mix01",
        [307] = "Sfx/Motor01_Mix12",
        [308] = "Sfx/MPB3",
        [309] = "Sfx/MPC2",
        [310] = "Sfx/MPG2",
        [311] = "Sfx/MumuDead_Mix04",
        [312] = "Sfx/MurfHeli_Mix01",
        [313] = "Sfx/MurfyVO1A_Mix01",
        [314] = "Sfx/MurfyVO1B_Mix01",
        [315] = "Sfx/MurfyVO3A_Mix01",
        [316] = "Sfx/MurfyVO4A_Mix01",
        [317] = "Sfx/NewPower_Mix06",
        [318] = "Sfx/NoCombus_Mix04",
        [319] = "Sfx/OnoEfor1_Mix02",
        [320] = "Sfx/OnoEfor2_Mix03",
        [321] = "Sfx/OnoEquil_Mix03",
        [322] = "Sfx/OnoExpir",
        [323] = "Sfx/OnoGO_Mix02",
        [324] = "Sfx/OnoInspi",
        [325] = "Sfx/OnoJump1",
        [326] = "Sfx/OnoJump3_Mix01",
        [327] = "Sfx/OnoJump4_Mix01",
        [328] = "Sfx/OnoJump5_Mix01",
        [329] = "Sfx/OnoJump6_Mix01",
        [330] = "Sfx/OnoPeur1_Mix03",
        [331] = "Sfx/OnoRcvH1_Mix04",
        [332] = "Sfx/OnoThrow_Mix02",
        [333] = "Sfx/OnoWinRM_Mix02",
        [334] = "Sfx/OnoWin_Mix02",
        [335] = "Sfx/OnoWoHoo_Mix01",
        [336] = "Sfx/ORGC4",
        [337] = "Sfx/pada",
        [338] = "Sfx/PannelDw_Mix01",
        [339] = "Sfx/PannelUp_Mix01",
        [340] = "Sfx/PathFX_Mix01",
        [341] = "Sfx/PF2Crac_Mix02",
        [342] = "Sfx/PF2Fall_Mix03",
        [343] = "Sfx/PIAd4",
        [344] = "Sfx/PICd4",
        [345] = "Sfx/PIG3",
        [346] = "Sfx/PinBall_Mix02",
        [347] = "Sfx/PiraAtk1_Mix01",
        [348] = "Sfx/PiraDead_Mix05",
        [349] = "Sfx/PiraHit1_Mix02",
        [350] = "Sfx/PiraHit3_Mix03",
        [351] = "Sfx/PiraHurt_Mix02",
        [352] = "Sfx/PlumSnd1_Mix03",
        [353] = "Sfx/PlumSnd2_Mix03",
        [354] = "Sfx/RaDeath_Mix03",
        [355] = "Sfx/RayFist2_Mix01",
        [356] = "Sfx/RayFist_Mix02",
        [357] = "Sfx/RaySigh_Mix01",
        [358] = "Sfx/RaySpin_Mix06",
        [359] = "Sfx/RireMumu_Mix03",
        [360] = "Sfx/RocktLeg_Mix03",
        [361] = "Sfx/RoktSpin_Mix03",
        [362] = "Sfx/RootIn_Mix01",
        [363] = "Sfx/RootOut_Mix04",
        [364] = "Sfx/RyVO1_Mix01",
        [365] = "Sfx/RyVO2_Mix01",
        [366] = "Sfx/RyVO3_Mix01",
        [367] = "Sfx/RyVOGlob_Mix02",
        [368] = "Sfx/RyVOLy_Mix01",
        [369] = "Sfx/ScaHurt2_Mix02",
        [370] = "Sfx/ScalDead_Mix02",
        [371] = "Sfx/ScalFlee_Mix02",
        [372] = "Sfx/ScalGrrr_Mix02",
        [373] = "Sfx/ScalHurt_Mix02",
        [374] = "Sfx/ScalUp_Mix03",
        [375] = "Sfx/ScaMorf1_Mix02",
        [376] = "Sfx/ScaMorf2_Mix02",
        [377] = "Sfx/SHKCd5",
        [378] = "Sfx/SHKDd4",
        [379] = "Sfx/SkiLoop1",
        [380] = "Sfx/SkiWeed_Mix02",
        [381] = "Sfx/SkulInit_Mix04",
        [382] = "Sfx/SkullEnd_Mix02",
        [383] = "Sfx/SkullHit_Mix02",
        [384] = "Sfx/SkulShak_Mix01",
        [385] = "Sfx/SlideIn_Mix02",
        [386] = "Sfx/SlideOut_Mix01",
        [387] = "Sfx/SND1",
        [388] = "Sfx/SNF1",
        [389] = "Sfx/SnoreFX1_Mix01",
        [390] = "Sfx/SocleFX1_Mix01",
        [391] = "Sfx/Sparkles_Mix01",
        [392] = "Sfx/SpherImp_Mix02",
        [393] = "Sfx/SpidrAtk_Mix02",
        [394] = "Sfx/Spirale_Mix01",
        [395] = "Sfx/SplshGen_Mix04",
        [396] = "Sfx/Store01_Mix01",
        [397] = "Sfx/Store02_Mix02",
        [398] = "Sfx/STRDd3",
        [399] = "Sfx/SuprFist_Mix01",
        [400] = "Sfx/Switch1_Mix03",
        [401] = "Sfx/Tag_Mix02",
        [402] = "Sfx/TAMB",
        [403] = "Sfx/Thunder1_Mix04",
        [404] = "Sfx/TiztrVO1_Mix01",
        [405] = "Sfx/TiztrVO2_Mix01",
        [406] = "Sfx/TiztrVO3_Mix01",
        [407] = "Sfx/TiztrVO4_Mix01",
        [408] = "Sfx/Valid01_Mix01",
        [409] = "Sfx/VibraFLW_Mix02",
        [410] = "Sfx/VICd3",
        [411] = "Sfx/VIG3",
        [412] = "Sfx/WallSlid_Mix02",
        [413] = "Sfx/Whistle1_Mix01",
        [414] = "Sfx/WoodBrk1_Mix04",
        [415] = "Sfx/WoodImp_Mix03",
        [416] = "Sfx/YC7XFG1",
        [417] = "Sfx/YG1C1",
        [418] = "Sfx/YG1D2",
        [419] = "Sfx/YG1F2",
        [420] = "Sfx/YoyoMove_Mix02",
        [422] = "Music/ancients",
        [423] = "Music/baddreams",
        [424] = "Music/barbeslide",
        [425] = "Music/barrel",
        [426] = "Music/barrel_BA",
        [427] = "Music/bigplatform",
        [428] = "Music/bigtrees",
        [429] = "Music/boss1",
        [430] = "Music/boss34",
        [431] = "Music/canopy",
        [432] = "Music/death",
        [433] = "Music/echocave",
        [434] = "Music/enemy1",
        [435] = "Music/enemy2",
        [436] = "Music/fairyglades",
        [437] = "Music/finalboss",
        [438] = "Music/firestone",
        [439] = "Music/happyslide",
        [440] = "Music/helico",
        [441] = "Music/jano",
        [442] = "Music/lyfree",
        [443] = "Music/lyfreeVOX4",
        [444] = "Music/lyrace",
        [445] = "Music/mountain1",
        [446] = "Music/mountain2",
        [447] = "Music/polokus",
        [448] = "Music/precipice",
        [449] = "Music/raytheme",
        [450] = "Music/rockchase",
        [451] = "Music/rocket",
        [452] = "Music/rocket_BA",
        [453] = "Music/sadslide",
        [454] = "Music/ship",
        [455] = "Music/spiderchase",
        [456] = "Music/tag",
        [457] = "Music/tizetre",
        [458] = "Music/tizetre_Swing",
        [459] = "Music/waterski",
        [460] = "Music/win1",
        [461] = "Music/win2",
        [462] = "Music/win3",
        [463] = "Music/Win_BOSS",
        [464] = "Music/woodlight",
    };
    #endregion

    #region Public Properties

    public override Dictionary<SoundType, string> SampleSongs => new()
    {
        [SoundType.Music] = "Music/raytheme",
        [SoundType.Sfx] = "Sfx/LumTotal_Mix02",
    };

    public override bool CanSkipCutscene => (Frame.Current as IHasScene)?.Scene.GetDialog<TextBoxDialog>()?.CanSkip() ?? false;

    #endregion

    #region Protected Methods

    protected override Frame CreateInitialFrame() => Engine.Settings.Platform switch
    {
        Platform.GBA => new Intro(),
        Platform.NGage => new NGageSplashScreensAct(),
        _ => throw new UnsupportedPlatformException()
    };

    protected override void LoadGame()
    {
        SoundEventsManager.SetCallBacks(new CallBackSet(
            getObjectPosition: x =>
            {
                if (x is not GameObject obj)
                    return Vector2.Zero;

                return new Vector2(obj.Position.X, 0);
            },
            getMikePosition: x =>
            {
                if (x is not GameObject obj)
                    return Vector2.Zero;

                TgxCamera cam = obj.Scene.Playfield.Camera;
                return new Vector2(cam.Position.X + cam.Resolution.X / 2, 0);
            },
            getSwitchIndex: () => 0));

        // TODO: Implement the rest
        ObjectFactory.Init(new Dictionary<ActorType, ObjectFactory.CreateActor>()
        {
            { ActorType.Rayman, (instanceId, scene, resource) => new Rayman(instanceId, scene, resource) },
            { ActorType.RaymanBody, (instanceId, scene, resource) => new RaymanBody(instanceId, scene, resource) },
            { ActorType.RedPirate, (instanceId, scene, resource) => new RedPirate(instanceId, scene, resource) },
            { ActorType.Piranha, (instanceId, scene, resource) => new Piranha(instanceId, scene, resource) },
            { ActorType.WaterSplash, (instanceId, scene, resource) => new WaterSplash(instanceId, scene, resource) },
            { ActorType.Explosion, (instanceId, scene, resource) => new Explosion(instanceId, scene, resource) },
            { ActorType.Missile, (instanceId, scene, resource) => new Missile(instanceId, scene, resource) },
            { ActorType.BouncyPlatform, (instanceId, scene, resource) => new BouncyPlatform(instanceId, scene, resource) },
            { ActorType.MovingFlowerPlatform, (instanceId, scene, resource) => new MovingPlatform(instanceId, scene, resource) },

            { ActorType.Switch, (instanceId, scene, resource) => new Switch(instanceId, scene, resource) },
            { ActorType.Gate, (instanceId, scene, resource) => new Gate(instanceId, scene, resource) },
            { ActorType.Lums, (instanceId, scene, resource) => new Lums(instanceId, scene, resource) },
            { ActorType.Cage, (instanceId, scene, resource) => new Cage(instanceId, scene, resource) },
            { ActorType.LevelCurtain, (instanceId, scene, resource) => new LevelCurtain(instanceId, scene, resource) },

            { ActorType.BreakableDoor, (instanceId, scene, resource) => new BreakableDoor(instanceId, scene, resource) },
            { ActorType.Keg, (instanceId, scene, resource) => new Keg(instanceId, scene, resource) },
            { ActorType.Barrel, (instanceId, scene, resource) => new Barrel(instanceId, scene, resource) },

            { ActorType.SilverPirate, (instanceId, scene, resource) => new SilverPirate(instanceId, scene, resource) },
            { ActorType.HelicopterBomb, (instanceId, scene, resource) => new FlyingBomb(instanceId, scene, resource) },
            { ActorType.ZombieChicken, (instanceId, scene, resource) => new ZombieChicken(instanceId, scene, resource) },

            { ActorType.BarrelSplash, (instanceId, scene, resource) => new BarrelSplash(instanceId, scene, resource) },
            { ActorType.Depart, (instanceId, scene, resource) => new Depart(instanceId, scene, resource) },
            { ActorType.RedShell, (instanceId, scene, resource) => new RedShell(instanceId, scene, resource) },
            { ActorType.KegFire, (instanceId, scene, resource) => new KegFire(instanceId, scene, resource) },

            { ActorType.Caterpillar, (instanceId, scene, resource) => new Caterpillar(instanceId, scene, resource) },

            { ActorType.Butterfly, (instanceId, scene, resource) => new Scenery(instanceId, scene, resource) },

            { ActorType.WoodenShieldedHoodboom, (instanceId, scene, resource) => new WoodenShieldedHoodboom(instanceId, scene, resource) },

            { ActorType.Slapdash, (instanceId, scene, resource) => new Slapdash(instanceId, scene, resource) },
            { ActorType.PurpleLum, (instanceId, scene, resource) => new PurpleLum(instanceId, scene, resource) },
            { ActorType.Grenade, (instanceId, scene, resource) => new Grenade(instanceId, scene, resource) },
            { ActorType.SwingSparkle, (instanceId, scene, resource) => new SwingSparkle(instanceId, scene, resource) },
            { ActorType.BreakableGround, (instanceId, scene, resource) => new BreakableGround(instanceId, scene, resource) },

            { ActorType.MovingWoodenPlatform, (instanceId, scene, resource) => new MovingPlatform(instanceId, scene, resource) },
            { ActorType.LavaSplash, (instanceId, scene, resource) => new LavaSplash(instanceId, scene, resource) },

            { ActorType.Hoodstormer, (instanceId, scene, resource) => new Hoodstormer(instanceId, scene, resource) },
            { ActorType.SpikyFlyingBomb, (instanceId, scene, resource) => new SpikyFlyingBomb(instanceId, scene, resource) },

            { ActorType.FlowerFire, (instanceId, scene, resource) => new FlowerFire(instanceId, scene, resource) },

            { ActorType.Murfy, (instanceId, scene, resource) => new Murfy(instanceId, scene, resource) },

            { ActorType.ChainedSparkles, (instanceId, scene, resource) => new ChainedSparkles(instanceId, scene, resource) },

            { ActorType.Bats, (instanceId, scene, resource) => new Bats(instanceId, scene, resource) },

            { ActorType.WoodenBar, (instanceId, scene, resource) => new WoodenBar(instanceId, scene, resource) },
            { ActorType.Ly, (instanceId, scene, resource) => new Ly(instanceId, scene, resource) },
            { ActorType.Flag, (instanceId, scene, resource) => new Scenery(instanceId, scene, resource) },

            { ActorType.KegDebris, (instanceId, scene, resource) => new KegDebris(instanceId, scene, resource) },

            { ActorType.Machine, (instanceId, scene, resource) => new Machine(instanceId, scene, resource) },

            { ActorType.Skull, (instanceId, scene, resource) => new Skull(instanceId, scene, resource) },
            
            { ActorType.MurfyStone, (instanceId, scene, resource) => new MurfyStone(instanceId, scene, resource) },

            { ActorType.Teensies, (instanceId, scene, resource) => new Teensies(instanceId, scene, resource) },

            { ActorType.Urchin, (instanceId, scene, resource) => new Urchin(instanceId, scene, resource) },
            { ActorType.Arrive, (instanceId, scene, resource) => new Arrive(instanceId, scene, resource) },
            { ActorType.SpikyBomb, (instanceId, scene, resource) => new FlyingBomb(instanceId, scene, resource) },

            { ActorType.MovingUnusedPlatform, (instanceId, scene, resource) => new MovingPlatform(instanceId, scene, resource) },
            { ActorType.RotatedHelicopterBomb, (instanceId, scene, resource) => new FlyingBomb(instanceId, scene, resource) },
            { ActorType.Leaf, (instanceId, scene, resource) => new Leaf(instanceId, scene, resource) },

            { ActorType.MetalShieldedHoodboom, (instanceId, scene, resource) => new MetalShieldedHoodboom(instanceId, scene, resource) },
        }, x => ((ActorType)x).ToString());
        Dictionary<MapId, LevelFactory.CreateLevel> levelCreations = new()
        {
            // World 1
            { MapId.WoodLight_M1, mapId => new WoodLight_M1(mapId) },
            { MapId.WoodLight_M2, mapId => new WoodLight_M2(mapId) },
            { MapId.FairyGlade_M1, mapId => new FairyGlade_M1(mapId) },
            { MapId.FairyGlade_M2, mapId => new FairyGlade_M2(mapId) },
            { MapId.MarshAwakening1, mapId => Engine.Settings.Platform != Platform.NGage ? new MarshAwakening1(mapId) : new NGageAscension(mapId) },
            { MapId.BossMachine, mapId => new BossMachine(mapId) },
            { MapId.SanctuaryOfBigTree_M1, mapId => new SanctuaryOfBigTree(mapId) },
            { MapId.SanctuaryOfBigTree_M2, mapId => new SanctuaryOfBigTree(mapId) },

            // World 2
            { MapId.MissileRace1, mapId => Engine.Settings.Platform != Platform.NGage ? new MissileRace1(mapId) : new FrameSideScroller(mapId) },
            { MapId.EchoingCaves_M1, mapId => new EchoingCaves_M1(mapId) },
            { MapId.EchoingCaves_M2, mapId => new EchoingCaves_M2(mapId) },
            { MapId.CavesOfBadDreams_M1, mapId => new CavesOfBadDreams(mapId) },
            { MapId.CavesOfBadDreams_M2, mapId => new CavesOfBadDreams(mapId) },
            { MapId.BossBadDreams, mapId => new BossBadDreams(mapId) },
            { MapId.MenhirHills_M1, mapId => new MenhirHills_M1(mapId) },
            { MapId.MenhirHills_M2, mapId => new FrameSideScroller(mapId) },
            { MapId.MarshAwakening2, mapId => Engine.Settings.Platform != Platform.NGage ? new MarshAwakening2(mapId) : new FrameSideScroller(mapId) },

            // World 3
            { MapId.SanctuaryOfStoneAndFire_M1, mapId => new SanctuaryOfStoneAndFire_M1(mapId) },
            { MapId.SanctuaryOfStoneAndFire_M2, mapId => new FrameSideScroller(mapId) },
            { MapId.SanctuaryOfStoneAndFire_M3, mapId => new FrameSideScroller(mapId) },
            { MapId.BeneathTheSanctuary_M1, mapId => new BeneathTheSanctuary_M1(mapId) },
            { MapId.BeneathTheSanctuary_M2, mapId => new FrameSideScroller(mapId) },
            { MapId.ThePrecipice_M1, mapId => new ThePrecipice_M1(mapId) },
            { MapId.ThePrecipice_M2, mapId => new ThePrecipice_M2(mapId) },
            { MapId.BossRockAndLava, mapId => new FrameSideScroller(mapId) },
            { MapId.TheCanopy_M1, mapId => new FrameSideScroller(mapId) },
            { MapId.TheCanopy_M2, mapId => new FrameSideScroller(mapId) },
            { MapId.SanctuaryOfRockAndLava_M1, mapId => new SanctuaryOfRockAndLava(mapId) },
            { MapId.SanctuaryOfRockAndLava_M2, mapId => new SanctuaryOfRockAndLava(mapId) },
            { MapId.SanctuaryOfRockAndLava_M3, mapId => new SanctuaryOfRockAndLava(mapId) },

            // World 4
            { MapId.TombOfTheAncients_M1, mapId => new FrameSideScroller(mapId) },
            { MapId.TombOfTheAncients_M2, mapId => new FrameSideScroller(mapId) },
            { MapId.BossScaleMan, mapId => new BossScaleMan(mapId) },
            { MapId.IronMountains_M1, mapId => new FrameSideScroller(mapId) },
            { MapId.IronMountains_M2, mapId => new FrameSideScroller(mapId) },
            { MapId.MissileRace2, mapId => Engine.Settings.Platform != Platform.NGage ? new MissileRace2(mapId) : new FrameSideScroller(mapId) },
            { MapId.PirateShip_M1, mapId => new FrameSideScroller(mapId) },
            { MapId.PirateShip_M2, mapId => new FrameSideScroller(mapId) },
            { MapId.BossFinal_M1, mapId => new BossFinal(mapId) },
            { MapId.BossFinal_M2, mapId => new BossFinal(mapId) },

            // Bonus
            { MapId.Bonus1, mapId => new FrameSideScroller(mapId) },
            { MapId.Bonus2, mapId => new FrameSideScroller(mapId) },
            { MapId.Bonus3, mapId => new FrameSideScroller(mapId) },
            { MapId.Bonus4, mapId => new FrameSideScroller(mapId) },
            { MapId._1000Lums, mapId => new FrameSideScroller(mapId) },

            // Ly's Challenge
            { MapId.ChallengeLy1, mapId => new ChallengeLy(mapId) },
            { MapId.ChallengeLy2, mapId => new ChallengeLy(mapId) },
            { MapId.ChallengeLyGCN, mapId => new ChallengeLy(mapId) },

            // Power
            { MapId.Power1, mapId => new FrameNewPower(mapId) },
            { MapId.Power2, mapId => new FrameNewPower(mapId) },
            { MapId.Power3, mapId => new FrameNewPower(mapId) },
            { MapId.Power4, mapId => new FrameNewPower(mapId) },
            { MapId.Power5, mapId => new FrameNewPower(mapId) },
            { MapId.Power6, mapId => new FrameNewPower(mapId) },

            // World
            { MapId.World1, mapId => new World1(mapId) },
            { MapId.World2, mapId => new World(mapId) },
            { MapId.World3, mapId => new World(mapId) },
            { MapId.World4, mapId => new World(mapId) },
            //{ MapId.WorldMap, mapId => new (mapId) }, // TODO: Create class for WorldMap
        };

        // TODO: Create classes for multiplayer maps
        switch (Engine.Settings.Platform)
        {
            case Platform.GBA:
                //levelCreations.Add(MapId.GbaMulti_MissileRace, mapId => new (mapId));
                //levelCreations.Add(MapId.GbaMulti_MissileArena, mapId => new (mapId));
                //levelCreations.Add(MapId.GbaMulti_RayTag1, mapId => new (mapId));
                //levelCreations.Add(MapId.GbaMulti_RayTag2, mapId => new (mapId));
                //levelCreations.Add(MapId.GbaMulti_CatAndMouse1, mapId => new (mapId));
                //levelCreations.Add(MapId.GbaMulti_CatAndMouse2, mapId => new (mapId));
                break;
            
            case Platform.NGage:
                //levelCreations.Add(MapId.NGageMulti_CaptureTheFlag1, mapId => new (mapId));
                //levelCreations.Add(MapId.NGageMulti_CaptureTheFlag2, mapId => new (mapId));
                //levelCreations.Add(MapId.NGageMulti_CaptureTheFlag3, mapId => new (mapId));
                //levelCreations.Add(MapId.NGageMulti_CaptureTheFlag4, mapId => new (mapId));
                //levelCreations.Add(MapId.NGageMulti_CaptureTheFlag5, mapId => new (mapId));
                //levelCreations.Add(MapId.NGageMulti_CaptureTheFlag6, mapId => new (mapId));
                //levelCreations.Add(MapId.NGageMulti_RayTag1, mapId => new (mapId));
                //levelCreations.Add(MapId.NGageMulti_RayTag2, mapId => new (mapId));
                //levelCreations.Add(MapId.NGageMulti_CatAndMouse1, mapId => new (mapId));
                //levelCreations.Add(MapId.NGageMulti_CatAndMouse2, mapId => new (mapId));
                break;

            default:
                throw new UnsupportedPlatformException();
        }

        LevelFactory.Init(levelCreations);

        // Set default language
        Localization.SetLanguage(0);
    }

    protected override void AddDebugWindowsAndMenus(DebugLayout debugLayout)
    {
        debugLayout.AddWindow(new SceneDebugWindow());
        debugLayout.AddWindow(new GameObjectDebugWindow());
        debugLayout.AddWindow(new PlayfieldDebugWindow());
        debugLayout.AddWindow(new GameInfoDebugWindow());
        debugLayout.AddMenu(new FramesDebugMenu());
        debugLayout.AddMenu(new GenerateDebugMenu());
    }

    protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
    {
        // Toggle showing debug collision screen
        if (InputManager.IsButtonJustPressed(Keys.T))
        {
            if (Frame.Current is IHasPlayfield { Playfield: { } playfield })
                playfield.PhysicalLayer.DebugScreen.IsEnabled = !playfield.PhysicalLayer.DebugScreen.IsEnabled;
        }

        base.Update(gameTime);
    }

    #endregion

    #region Public Methods

    public override void SkipCutscene()
    {
        (Frame.Current as IHasScene)?.Scene.GetDialog<TextBoxDialog>()?.Skip();
    }

    #endregion
}